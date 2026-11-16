using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using Weavers.Core.Extensions;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.AssetArchive {
  // ─────────────────────────────────────────────────────────────────────────────
  // SyncAssetArchive: walk an AssetArchive's RootFolder and bring the catalog
  // (MediaAssets + ArchiveAssets) in line with what's on disk.
  //
  //   unchanged (same RelPath + size + mtime)  -> touch LastSeenUtc, no read
  //   new / changed                            -> SHA-256 + metadata probe, upsert
  //   cloud placeholder (iCloud "online only") -> skipped unless HydrateCloudFiles
  //   Present in DB but not seen this scan     -> Status = Missing
  //
  // Root not reachable -> archive marked Available=False, catalog left untouched.
  // That guard matters: an unplugged USB drive must never flip 200k rows to Missing.
  // ─────────────────────────────────────────────────────────────────────────────

  public enum ArchiveAssetStatus : byte { Present = 0, Missing = 1, Quarantined = 2, Offloaded = 3 }

  public sealed record SyncAssetArchiveCommand(int ArchiveItemId) : IRequest<SyncAssetArchiveResult> {
    /// <summary>Open (and so download) cloud placeholder files. Off by default: a 150 GB iCloud folder would hydrate.</summary>
    public bool HydrateCloudFiles { get; init; } = false;
    /// <summary>Concurrent hash/probe workers. Keep low for spinning disks, 4-8 for NVMe.</summary>
    public int HashParallelism { get; init; } = 2;
    public IProgress<SyncAssetArchiveProgress>? Progress { get; init; }
  }

  public sealed record SyncAssetArchiveProgress(int Scanned, int Unchanged, int Hashed, int Errors);

  public sealed class SyncAssetArchiveResult {
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int Scanned { get; set; }
    public int Unchanged { get; set; }
    public int Added { get; set; }
    public int Changed { get; set; }
    public int NewAssets { get; set; }              // content never seen before anywhere
    public int MatchedExistingContent { get; set; } // hash already catalogued (dup or copy elsewhere)
    public int Placeholders { get; set; }
    public int Missing { get; set; }
    public int Errors { get; set; }
    public List<string> ErrorSamples { get; } = new();
    public TimeSpan Elapsed { get; set; }
  }

  public sealed class SyncAssetArchiveHandler(FabricDbContext db, 
    IAssetArchiveSettingsStore settings,      
    IAppSessionService appSessionService ) : IRequestHandler<SyncAssetArchiveCommand, SyncAssetArchiveResult> {
    private readonly IAppSessionService _appSessionService = appSessionService;
    const int ChunkSize = 200;          // files hashed + saved per SaveChanges
    const int TouchFlushSize = 2000;    // unchanged ids per LastSeen update
    const int MaxErrorSamples = 50;

    // No ReparsePoint skip: Windows cloud-sync files AND folders (iCloud, OneDrive) are
    // reparse points, so skipping them would hide the whole iCloud tree. Depth cap guards
    // against junction loops instead.
    static readonly EnumerationOptions EnumOptions = new() {
      RecurseSubdirectories = true,
      IgnoreInaccessible = true,
      AttributesToSkip = FileAttributes.System,
      MaxRecursionDepth = 32,
    };

    const FileAttributes RecallOnDataAccess = (FileAttributes)0x00400000;
    const FileAttributes RecallOnOpen = (FileAttributes)0x00040000;

    sealed record KnownRow(int Id, string RelPath, DateTime FileMtimeUtc, long FileSize);
    sealed record WorkItem(string FullPath, string RelPath, DateTime MtimeUtc, int? ExistingId);

    public async Task<SyncAssetArchiveResult> Handle(SyncAssetArchiveCommand req, CancellationToken ct) {
      var sw = Stopwatch.StartNew();
      var result = new SyncAssetArchiveResult();

      var archive = await settings.LoadAsync(req.ArchiveItemId, ct);
      if (archive is null)
        return Fail(result, $"Item {req.ArchiveItemId} is not an AssetArchiveModel.");
      if (archive.ServiceHostId != _appSessionService.HarnessId)
        return Fail(result, $"Archive is hosted by harness {archive.ServiceHostId}; this instance is on {_appSessionService.HarnessId}.");
      if (string.IsNullOrWhiteSpace(archive.RootFolder))
        return Fail(result, "RootFolder is not set.");

      var root = Path.GetFullPath(archive.RootFolder);
      var available = Directory.Exists(root);
      if (available != archive.Available)
        await settings.SetAvailableAsync(archive.ItemId, available, ct);
      if (!available)
        return Fail(result, $"'{root}' is not reachable. Archive marked unavailable; catalog left untouched.");

      // One sync per archive across ALL instances/machines. Session-owned app lock lives
      // as long as this connection is open; pooled-connection reset also releases it.
      await db.Database.OpenConnectionAsync(ct);
      try {
        if (!await TryGetSyncLockAsync(archive.ItemId, ct))
          return Fail(result, "Another instance is already syncing this archive.");
        try { 
          // LastSeenUtc is datetime2(0) and SQL rounds; back off a second so rows touched
          // during this scan can never compare as older than the scan start.
          var scanStartUtc = DateTime.UtcNow.AddSeconds(-1);

          // 1. What this archive already knows (~100 bytes/row; 300k rows ≈ 30 MB).
          var known = await db.ArchiveAssets.AsNoTracking()
            .Where(a => a.ArchiveId == archive.ItemId)
            .Select(a => new KnownRow(a.Id, a.RelPath, a.FileMtimeUtc, a.Asset.FileSize))
            .ToDictionaryAsync(k => k.RelPath, StringComparer.OrdinalIgnoreCase, ct);

          // 2. Global content index: hash -> MediaAssetId (dedup is across every archive).
          var hashIndex = new Dictionary<byte[], int>(ByteArrayComparer.Instance);
          await foreach (var m in db.MediaAssets.AsNoTracking()
            .Select(m => new { m.Id, m.FileHash }).AsAsyncEnumerable().WithCancellation(ct))
            hashIndex[m.FileHash] = m.Id;

          var touch = new List<int>(TouchFlushSize);
          var work = new List<WorkItem>(ChunkSize);

          // 3. Walk the disk.
          foreach (var path in Directory.EnumerateFiles(root, "*", EnumOptions)) {
            ct.ThrowIfCancellationRequested();
            if (!MediaProbe.IsMediaExtension(path)) continue;
            result.Scanned++;

            FileInfo fi;
            try { 
              fi = new FileInfo(path); 
              _ = fi.Length; 
            } catch (Exception ex) { 
              AddError(result, $"{path}: {ex.Message}"); continue; 
            }

            var rel = Path.GetRelativePath(root, path);
            known.TryGetValue(rel, out var row);
            var mtime = fi.LastWriteTimeUtc;

            var unchanged = row is not null && row.FileSize == fi.Length && row.FileMtimeUtc.Ticks == mtime.Ticks;
            var placeholder = (fi.Attributes & (RecallOnDataAccess | RecallOnOpen | FileAttributes.Offline)) != 0;

            if (unchanged || (placeholder && !req.HydrateCloudFiles)) {
              if (unchanged) result.Unchanged++; else result.Placeholders++;
              if (row is not null) {
                touch.Add(row.Id);
                if (touch.Count >= TouchFlushSize) { await FlushTouchAsync(touch, ct); Report(req, result); }
              }
              continue;
            }

            work.Add(new WorkItem(path, rel, mtime, row?.Id));
            if (work.Count >= ChunkSize) {
              await ProcessChunkAsync(work, archive.ItemId, hashIndex, req, result, ct);
              work.Clear();
              Report(req, result);
            }
          }

          if (work.Count > 0) await ProcessChunkAsync(work, archive.ItemId, hashIndex, req, result, ct);
          await FlushTouchAsync(touch, ct);

          // 4. Present in the catalog but not seen on this walk -> Missing.
          //    Only reached on a full, uncancelled walk.
          result.Missing = await db.ArchiveAssets
            .Where(a => a.ArchiveId == archive.ItemId
                     && a.Status == (byte)ArchiveAssetStatus.Present
                     && a.LastSeenUtc < scanStartUtc)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, (byte)ArchiveAssetStatus.Missing), ct);

        
        } finally {
          await ReleaseSyncLockAsync(archive.ItemId);
        }
      } finally {
        await db.Database.CloseConnectionAsync();
      }

      result.Success = true;
      result.Elapsed = sw.Elapsed;
      result.Message = $"Scanned {result.Scanned:N0}: {result.Unchanged:N0} unchanged, {result.Added:N0} added, " +
                       $"{result.Changed:N0} changed, {result.Missing:N0} missing, {result.Placeholders:N0} cloud-only, " +
                       $"{result.Errors:N0} errors in {sw.Elapsed:hh\\:mm\\:ss}.";
      Report(req, result);
      return result;
    }

    async Task ProcessChunkAsync(List<WorkItem> items, int archiveId, Dictionary<byte[], int> hashIndex,
                                 SyncAssetArchiveCommand req, SyncAssetArchiveResult result, CancellationToken ct) {
      // Hash + probe in parallel (IO/CPU); apply to the DbContext single-threaded.
      var probes = new ProbeResult[items.Count];
      await Parallel.ForAsync(0, items.Count,
        new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, req.HashParallelism), CancellationToken = ct },
        async (i, token) => probes[i] = await MediaProbe.ProbeAsync(items[i].FullPath, token));

      var changedIds = items.Where(w => w.ExistingId is not null).Select(w => w.ExistingId!.Value).ToList();
      var changedRows = changedIds.Count == 0
        ? new Dictionary<int, ArchiveAsset>()
        : await db.ArchiveAssets.Where(a => changedIds.Contains(a.Id)).ToDictionaryAsync(a => a.Id, ct);

      var pending = new Dictionary<byte[], MediaAsset>(ByteArrayComparer.Instance);
      var now = DateTime.UtcNow;

      for (var i = 0; i < items.Count; i++) {
        var w = items[i];
        var p = probes[i];
        if (p.Asset is null) { AddError(result, $"{w.RelPath}: {p.Error}"); continue; }

        var hash = p.Asset.FileHash;
        var existingAssetId = 0;
        MediaAsset? newAsset = null;

        if (hashIndex.TryGetValue(hash, out existingAssetId) || pending.TryGetValue(hash, out newAsset)) {
          result.MatchedExistingContent++;
        } else {
          newAsset = p.Asset;
          db.MediaAssets.Add(newAsset);
          pending[hash] = newAsset;
          result.NewAssets++;
        }

        ArchiveAsset row;
        if (w.ExistingId is int rid && changedRows.TryGetValue(rid, out var tracked)) {
          row = tracked;              // content at this path changed (old MediaAsset may now be orphaned)
          result.Changed++;
        } else {
          row = new ArchiveAsset { ArchiveId = archiveId, RelPath = w.RelPath };
          db.ArchiveAssets.Add(row);
          result.Added++;
        }

        row.FileMtimeUtc = w.MtimeUtc;
        row.LastSeenUtc = now;
        row.LastVerifiedUtc = now;    // we just hashed it
        row.Status = (byte)ArchiveAssetStatus.Present;
        if (newAsset is not null) row.Asset = newAsset;
        else row.MediaAssetId = existingAssetId;
      }

      try {
        await db.SaveChangesAsync(ct);
      } catch (DbUpdateException ex) {
        // Most likely: another archive's sync inserted the same new hash a moment ago
        // (UX_MediaAssets_FileHash). Rerunning the sync picks it up as existing content.
        db.ChangeTracker.Clear();
        throw new InvalidOperationException("Catalog write conflict; rerun sync to continue. " + ex.InnerException?.Message, ex);
      }

      foreach (var (h, m) in pending) hashIndex[h] = m.Id;
      db.ChangeTracker.Clear();       // keep the tracker from growing across 300k files
    }

    async Task FlushTouchAsync(List<int> ids, CancellationToken ct) {
      if (ids.Count == 0) return;
      var now = DateTime.UtcNow;
      const byte missing = (byte)ArchiveAssetStatus.Missing, present = (byte)ArchiveAssetStatus.Present;
      await db.ArchiveAssets
        .Where(a => ids.Contains(a.Id))
        .ExecuteUpdateAsync(s => s
          .SetProperty(a => a.LastSeenUtc, now)
          .SetProperty(a => a.Status, a => a.Status == missing ? present : a.Status), ct);   // came back
      ids.Clear();
    }

    async Task<bool> TryGetSyncLockAsync(int archiveId, CancellationToken ct) {
      await using var cmd = db.Database.GetDbConnection().CreateCommand();
      cmd.CommandText = "DECLARE @r int; EXEC @r = sp_getapplock @Resource = @res, @LockMode = 'Exclusive', " +
                        "@LockOwner = 'Session', @LockTimeout = 0; SELECT @r;";
      var p = cmd.CreateParameter(); p.ParameterName = "@res"; p.Value = $"AssetArchiveSync:{archiveId}";
      cmd.Parameters.Add(p);
      return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) >= 0;
    }

    async Task ReleaseSyncLockAsync(int archiveId) {
      await using var cmd = db.Database.GetDbConnection().CreateCommand();
      cmd.CommandText = "EXEC sp_releaseapplock @Resource = @res, @LockOwner = 'Session';";
      var p = cmd.CreateParameter(); p.ParameterName = "@res"; p.Value = $"AssetArchiveSync:{archiveId}";
      cmd.Parameters.Add(p);
      await cmd.ExecuteNonQueryAsync();
    }

    static void Report(SyncAssetArchiveCommand req, SyncAssetArchiveResult r) =>
      req.Progress?.Report(new SyncAssetArchiveProgress(r.Scanned, r.Unchanged, r.Added + r.Changed, r.Errors));

    static void AddError(SyncAssetArchiveResult r, string msg) {
      r.Errors++;
      if (r.ErrorSamples.Count < MaxErrorSamples) r.ErrorSamples.Add(msg);
    }

    static SyncAssetArchiveResult Fail(SyncAssetArchiveResult r, string msg) {
      r.Success = false; r.Message = msg; return r;
    }
  }



}
