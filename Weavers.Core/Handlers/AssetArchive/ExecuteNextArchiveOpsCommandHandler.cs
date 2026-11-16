using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;

namespace Weavers.Core.Handlers.AssetArchive {
  // ─────────────────────────────────────────────────────────────────────────────
  // One unit of work for the ArchiveOps line:
  //   1. claim up to BatchSize commands for this machine (Approved/Running plans only)
  //   2. flip any Approved plan touched to Running (+ StartedUtc)
  //   3. execute each: copy -> hash -> verify -> rename -> record ArchiveAsset -> Done
  //   4. try to close every plan touched; the instance that wins writes ResultSummary
  //
  // Same-machine only for now: claims require SourceHarnessId == TargetHarnessId == me.
  // Cross-machine commands simply wait in the queue until UNC / HTTP pull exists.
  //
  // Safe to kill at any point: an unfinished copy is only ever a *.part file; an expired
  // lease hands the command to another instance; every step re-checks before acting.
  // ─────────────────────────────────────────────────────────────────────────────

  public enum ArchiveOpOutcome : byte { Worked = 1, Idle = 2 }

  public record ExecuteNextArchiveOpsCommand(int HarnessId, string InstanceId, int BatchSize = 10)
    : IMcpRequest, IRequest<ArchiveOpResult> {
    public IProgress<string>? Progress { get; init; }   // "copying 2019/07/04/IMG_1234.HEIC (4.2 MB)"
  }

  public class ArchiveOpResult {
    public ArchiveOpOutcome Outcome { get; set; }
    public int Done { get; set; }
    public int Retrying { get; set; }
    public int Failed { get; set; }
    public long Bytes { get; set; }
    public List<int> PlansClosed { get; set; } = new();
    public string Message { get; set; } = "";
  }

  public class ExecuteNextArchiveOpsCommandHandler(FabricDbContext db, IMediator mediator)
    : IRequestHandler<ExecuteNextArchiveOpsCommand, ArchiveOpResult> {

    private const int LeaseSeconds = 120;
    private const int HeartbeatSeconds = 30;
    private const int MaxAttempts = 5;
    private const int CopyBufferSize = 1 << 20;
    private static readonly TimeSpan[] Backoff = {
      TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), TimeSpan.FromHours(2),
    };

    private sealed class Work {
      public int Id { get; set; }
      public int AssetPlanId { get; set; }
      public int Attempts { get; set; }
      public int MediaAssetId { get; set; }
      public long Bytes { get; set; }
      public int SourceArchiveId { get; set; }
      public int TargetArchiveId { get; set; }
      public string TargetRelPath { get; set; } = "";
      public string SourceRelPath { get; set; } = "";
      public byte[] FileHash { get; set; } = Array.Empty<byte>();
    }

    private readonly Dictionary<int, string> _roots = new();
    private string _instance = "";
    private Stopwatch _sinceHeartbeat = Stopwatch.StartNew();

    public async Task<ArchiveOpResult> Handle(ExecuteNextArchiveOpsCommand request, CancellationToken ct) {
      _instance = request.InstanceId;
      var result = new ArchiveOpResult();

      // ── 1. Claim ───────────────────────────────────────────────────────────
      var claimedIds = await ClaimAsync(request.HarnessId, request.BatchSize, ct);
      if (claimedIds.Count == 0) {
        result.Outcome = ArchiveOpOutcome.Idle;
        result.Message = "Nothing to do.";
        return result;
      }
      result.Outcome = ArchiveOpOutcome.Worked;
      _sinceHeartbeat.Restart();

      // Everything the executor needs, in one query.
      var work = await (
        from c in db.AssetCommands.AsNoTracking()
        join s in db.ArchiveAssets on c.SourceArchiveAssetId equals s.Id
        join m in db.MediaAssets on c.MediaAssetId equals m.Id
        where claimedIds.Contains(c.Id)
        orderby c.Id
        select new Work {
          Id = c.Id,
          AssetPlanId = c.AssetPlanId,
          Attempts = c.Attempts,
          MediaAssetId = c.MediaAssetId,
          Bytes = c.Bytes,
          SourceArchiveId = c.SourceArchiveId,
          TargetArchiveId = c.TargetArchiveId,
          TargetRelPath = c.TargetRelPath,
          SourceRelPath = s.RelPath,
          FileHash = m.FileHash,
        }).ToListAsync(ct);

      // ── 2. Approved -> Running for the plans we just touched ──────────────
      var planIds = work.Select(w => w.AssetPlanId).Distinct().ToList();
      var startedUtc = DateTime.UtcNow;
      await db.AssetPlans
        .Where(p => planIds.Contains(p.Id) && p.Status == (byte)AssetPlanStatus.Approved)
        .ExecuteUpdateAsync(s => s
          .SetProperty(p => p.Status, (byte)AssetPlanStatus.Running)
          .SetProperty(p => p.StartedUtc, p => p.StartedUtc ?? startedUtc), ct);

      await LoadRootsAsync(work.SelectMany(w => new[] { w.SourceArchiveId, w.TargetArchiveId }).Distinct(), ct);

      // ── 3. Execute ─────────────────────────────────────────────────────────
      for (var i = 0; i < work.Count; i++) {
        var w = work[i];
        if (ct.IsCancellationRequested) {
          await ReleaseAsync(work.Skip(i).Select(x => x.Id).ToList());   // hand back the rest untouched
          break;
        }
        try {
          request.Progress?.Report($"Copying {w.TargetRelPath} ({FormatBytes(w.Bytes)})");
          var targetAaId = await ExecuteOneAsync(w, ct);
          await MarkDoneAsync(w.Id, targetAaId);
          result.Done++;
          result.Bytes += w.Bytes;
        } catch (OperationCanceledException) when (ct.IsCancellationRequested) {
          await ReleaseAsync(work.Skip(i).Select(x => x.Id).ToList());
          break;
        } catch (Exception ex) {
          if (await MarkFailedAsync(w, ex.Message)) result.Failed++; else result.Retrying++;
        }
      }

      // ── 4. Close finished plans; the winner writes ResultSummary ───────────
      foreach (var planId in planIds) {
        if (await TryClosePlanAsync(planId)) result.PlansClosed.Add(planId);
      }

      result.Message = $"{result.Done} copied ({FormatBytes(result.Bytes)}), {result.Retrying} retrying, {result.Failed} failed" +
                       (result.PlansClosed.Count > 0 ? $"; closed plan(s) {string.Join(", ", result.PlansClosed)}" : "");
      return result;
    }

    // ── Claim ────────────────────────────────────────────────────────────────
    private async Task<List<int>> ClaimAsync(int harnessId, int batch, CancellationToken ct) {
      const string sql = @"
        UPDATE TOP (@batch) c WITH (ROWLOCK, UPDLOCK, READPAST)
        SET c.Status = 1, c.ClaimedBy = @me,
            c.LeaseUntilUtc = DATEADD(second, @lease, SYSUTCDATETIME()),
            c.Attempts = c.Attempts + 1
        OUTPUT inserted.Id
        FROM dbo.AssetCommands c
        JOIN dbo.AssetPlans p ON p.Id = c.AssetPlanId
        WHERE c.TargetHarnessId = @harness
          AND c.SourceHarnessId = @harness                 -- same-machine only, for now
          AND p.Status IN (1, 2)                           -- Approved, Running
          AND (c.Status = 0 OR (c.Status = 1 AND c.LeaseUntilUtc < SYSUTCDATETIME()))
          AND (c.NotBeforeUtc IS NULL OR c.NotBeforeUtc <= SYSUTCDATETIME())
          AND c.Attempts < @maxAttempts;";

      var ids = new List<int>();
      var conn = db.Database.GetDbConnection();
      var openedHere = conn.State != System.Data.ConnectionState.Open;
      if (openedHere) await conn.OpenAsync(ct);
      try {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqlParameter("@batch", batch));
        cmd.Parameters.Add(new SqlParameter("@me", _instance));
        cmd.Parameters.Add(new SqlParameter("@lease", LeaseSeconds));
        cmd.Parameters.Add(new SqlParameter("@harness", harnessId));
        cmd.Parameters.Add(new SqlParameter("@maxAttempts", MaxAttempts));
        await using var r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct)) ids.Add(r.GetInt32(0));
      } finally {
        if (openedHere) await conn.CloseAsync();
      }
      return ids;
    }

    /// <summary>Extends the lease on everything this instance holds. Called inline from the copy loop.</summary>
    private async Task HeartbeatAsync() {
      if (_sinceHeartbeat.Elapsed.TotalSeconds < HeartbeatSeconds) return;
      var until = DateTime.UtcNow.AddSeconds(LeaseSeconds);
      await db.AssetCommands
        .Where(c => c.ClaimedBy == _instance && c.Status == (byte)AssetCommandStatus.Claimed)
        .ExecuteUpdateAsync(s => s.SetProperty(c => c.LeaseUntilUtc, until));
      _sinceHeartbeat.Restart();
    }

    // ── One command ──────────────────────────────────────────────────────────
    /// <returns>The target ArchiveAsset Id the content now lives at.</returns>
    private async Task<int> ExecuteOneAsync(Work w, CancellationToken ct) {
      const byte present = (byte)ArchiveAssetStatus.Present;

      // a) Already there? (another plan, or the target was synced since the plan was built)
      var existing = await db.ArchiveAssets.AsNoTracking()
        .Where(a => a.ArchiveId == w.TargetArchiveId && a.MediaAssetId == w.MediaAssetId && a.Status == present)
        .Select(a => (int?)a.Id).FirstOrDefaultAsync(ct);
      if (existing is not null) return existing.Value;

      var srcPath = Path.Combine(_roots[w.SourceArchiveId], w.SourceRelPath);
      var targetRoot = _roots[w.TargetArchiveId];
      var relPath = w.TargetRelPath;
      var dstPath = Path.Combine(targetRoot, relPath);

      if (!File.Exists(srcPath)) throw new FileNotFoundException($"Source missing: {srcPath} (resync the source archive)");

      // b) Something already at the intended path?
      if (File.Exists(dstPath)) {
        if (await HashMatchesAsync(dstPath, w.FileHash, ct)) {
          return await RecordAsync(w, relPath, dstPath);                 // right content, just not catalogued
        }
        relPath = WithHashSuffix(relPath, w.FileHash);                   // different content: sidestep it
        dstPath = Path.Combine(targetRoot, relPath);
      }

      // c) Copy to .part while hashing, verify, then atomic rename.
      Directory.CreateDirectory(Path.GetDirectoryName(dstPath)!);
      var partPath = dstPath + ".part";
      var srcInfo = new FileInfo(srcPath);
      try {
        byte[] copiedHash;
        await using (var src = new FileStream(srcPath, FileMode.Open, FileAccess.Read, FileShare.Read,
                                              CopyBufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan))
        await using (var dst = new FileStream(partPath, FileMode.Create, FileAccess.Write, FileShare.None,
                                              CopyBufferSize, FileOptions.Asynchronous)) {
          using var hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
          var buffer = new byte[CopyBufferSize];
          int read;
          while ((read = await src.ReadAsync(buffer, ct)) > 0) {
            hasher.AppendData(buffer, 0, read);
            await dst.WriteAsync(buffer.AsMemory(0, read), ct);
            await HeartbeatAsync();                                      // long videos keep their lease
          }
          await dst.FlushAsync(ct);
          copiedHash = hasher.GetHashAndReset();
        }

        if (!copiedHash.AsSpan().SequenceEqual(w.FileHash)) {
          throw new InvalidOperationException("Source content changed since it was catalogued (hash mismatch); resync the source archive.");
        }

        // Same mtime as the source so the target archive's next sync sees it as unchanged.
        File.SetLastWriteTimeUtc(partPath, srcInfo.LastWriteTimeUtc);
        File.SetCreationTimeUtc(partPath, srcInfo.CreationTimeUtc);

        try {
          File.Move(partPath, dstPath, overwrite: false);
        } catch (IOException) when (File.Exists(dstPath)) {
          relPath = WithHashSuffix(relPath, w.FileHash, 16);             // raced another writer
          dstPath = Path.Combine(targetRoot, relPath);
          File.Move(partPath, dstPath, overwrite: false);
        }
      } finally {
        if (File.Exists(partPath)) {
          try { File.Delete(partPath); } catch { /* next attempt overwrites it */ }
        }
      }

      return await RecordAsync(w, relPath, dstPath);
    }

    private async Task<int> RecordAsync(Work w, string relPath, string fullPath) {
      var now = DateTime.UtcNow;
      var row = new ArchiveAsset {
        ArchiveId = w.TargetArchiveId,
        MediaAssetId = w.MediaAssetId,
        RelPath = relPath,
        FileMtimeUtc = File.GetLastWriteTimeUtc(fullPath),
        LastSeenUtc = now,
        LastVerifiedUtc = now,
        Status = (byte)ArchiveAssetStatus.Present,
      };
      db.ArchiveAssets.Add(row);
      await db.SaveChangesAsync();
      db.ChangeTracker.Clear();
      return row.Id;
    }

    private static async Task<bool> HashMatchesAsync(string path, byte[] expected, CancellationToken ct) {
      await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                                          CopyBufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
      if (fs.Length == 0 && expected.Length > 0) return false;
      var h = await SHA256.HashDataAsync(fs, ct);
      return h.AsSpan().SequenceEqual(expected);
    }

    // ── Command outcomes (no CancellationToken: these must land even while stopping) ──
    private Task MarkDoneAsync(int commandId, int targetArchiveAssetId) {
      var now = DateTime.UtcNow;
      return db.AssetCommands.Where(c => c.Id == commandId && c.ClaimedBy == _instance)
        .ExecuteUpdateAsync(s => s
          .SetProperty(c => c.Status, (byte)AssetCommandStatus.Done)
          .SetProperty(c => c.Step, (byte)AssetCommandStep.Verified)
          .SetProperty(c => c.TargetArchiveAssetId, targetArchiveAssetId)
          .SetProperty(c => c.DoneUtc, now)
          .SetProperty(c => c.LeaseUntilUtc, (DateTime?)null)
          .SetProperty(c => c.LastError, (string?)null));
    }

    /// <returns>true if the command is now permanently Failed; false if it will retry.</returns>
    private async Task<bool> MarkFailedAsync(Work w, string error) {
      var final = w.Attempts >= MaxAttempts;
      var notBefore = final ? (DateTime?)null : DateTime.UtcNow + Backoff[Math.Min(w.Attempts - 1, Backoff.Length - 1)];
      var msg = error.Length > 1000 ? error[..1000] : error;
      await db.AssetCommands.Where(c => c.Id == w.Id && c.ClaimedBy == _instance)
        .ExecuteUpdateAsync(s => s
          .SetProperty(c => c.Status, final ? (byte)AssetCommandStatus.Failed : (byte)AssetCommandStatus.Pending)
          .SetProperty(c => c.ClaimedBy, (string?)null)
          .SetProperty(c => c.LeaseUntilUtc, (DateTime?)null)
          .SetProperty(c => c.NotBeforeUtc, notBefore)
          .SetProperty(c => c.LastError, msg));
      return final;
    }

    /// <summary>Stop pressed: give unstarted / interrupted commands back without burning an attempt.</summary>
    private Task ReleaseAsync(List<int> ids) {
      if (ids.Count == 0) return Task.CompletedTask;
      return db.AssetCommands
        .Where(c => ids.Contains(c.Id) && c.ClaimedBy == _instance && c.Status == (byte)AssetCommandStatus.Claimed)
        .ExecuteUpdateAsync(s => s
          .SetProperty(c => c.Status, (byte)AssetCommandStatus.Pending)
          .SetProperty(c => c.ClaimedBy, (string?)null)
          .SetProperty(c => c.LeaseUntilUtc, (DateTime?)null)
          .SetProperty(c => c.Attempts, c => c.Attempts - 1));
    }

    // ── Plan close ───────────────────────────────────────────────────────────
    private async Task<bool> TryClosePlanAsync(int planId) {
      var now = DateTime.UtcNow;
      var closed = await db.AssetPlans
        .Where(p => p.Id == planId && p.Status == (byte)AssetPlanStatus.Running
                 && !db.AssetCommands.Any(c => c.AssetPlanId == planId
                      && (c.Status == (byte)AssetCommandStatus.Pending || c.Status == (byte)AssetCommandStatus.Claimed)))
        .ExecuteUpdateAsync(s => s
          .SetProperty(p => p.Status, (byte)AssetPlanStatus.Done)
          .SetProperty(p => p.DoneUtc, now));
      if (closed != 1) return false;

      // This instance won the close: write the result summary.
      var stats = await db.AssetCommands.AsNoTracking()
        .Where(c => c.AssetPlanId == planId)
        .GroupBy(c => c.Status)
        .Select(g => new { Status = g.Key, Cnt = g.Count(), Bytes = g.Sum(c => c.Bytes) })
        .ToListAsync();
      var plan = await db.AssetPlans.AsNoTracking()
        .Where(p => p.Id == planId).Select(p => new { p.StartedUtc, p.DoneUtc }).FirstAsync();

      int Cnt(AssetCommandStatus s) => stats.FirstOrDefault(x => x.Status == (byte)s)?.Cnt ?? 0;
      long Bytes(AssetCommandStatus s) => stats.FirstOrDefault(x => x.Status == (byte)s)?.Bytes ?? 0;
      var elapsed = plan.StartedUtc is null || plan.DoneUtc is null ? TimeSpan.Zero : plan.DoneUtc.Value - plan.StartedUtc.Value;
      var rate = elapsed.TotalSeconds > 1 ? $", {FormatBytes((long)(Bytes(AssetCommandStatus.Done) / elapsed.TotalSeconds))}/s" : "";

      var failedSamples = await db.AssetCommands.AsNoTracking()
        .Where(c => c.AssetPlanId == planId && c.Status == (byte)AssetCommandStatus.Failed)
        .OrderBy(c => c.Id).Take(10)
        .Select(c => c.TargetRelPath + ": " + c.LastError)
        .ToListAsync();

      var lines = new List<string> {
        $"Plan {planId} done in {elapsed:hh\\:mm\\:ss}{rate}",
        $"{Cnt(AssetCommandStatus.Done):N0} copied ({FormatBytes(Bytes(AssetCommandStatus.Done))})",
        $"{Cnt(AssetCommandStatus.Failed):N0} failed, {Cnt(AssetCommandStatus.Skipped):N0} skipped",
      };
      if (failedSamples.Count > 0) {
        lines.Add("Failures:");
        lines.AddRange(failedSamples.Select(f => "  " + f));
      }

      var summary = string.Join(Environment.NewLine, lines);
      await db.AssetPlans.Where(p => p.Id == planId)
        .ExecuteUpdateAsync(s => s.SetProperty(p => p.ResultSummary, summary));
      return true;
    }

    // ── Archive roots ────────────────────────────────────────────────────────
    private async Task LoadRootsAsync(IEnumerable<int> archiveIds, CancellationToken ct) {
      var missing = archiveIds.Where(id => !_roots.ContainsKey(id)).ToList();
      if (missing.Count == 0) return;
      var items = await mediator.Send(new GetItemsByIdsQuery(missing), ct);
      foreach (var item in items) {
        var root = item.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value;
        if (string.IsNullOrWhiteSpace(root)) throw new InvalidOperationException($"Archive {item.Id} has no RootFolder.");
        _roots[item.Id] = Path.GetFullPath(root);
        // Later: ArchiveAccessPath lookup for (archive, this harness) goes here for UNC roots.
      }
    }

    private static string WithHashSuffix(string relPath, byte[] hash, int hexChars = 8) {
      var dir = Path.GetDirectoryName(relPath) ?? "";
      var tag = Convert.ToHexString(hash)[..Math.Min(hexChars, hash.Length * 2)].ToLowerInvariant();
      return Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(relPath)}~{tag}{Path.GetExtension(relPath)}");
    }

    private static string FormatBytes(long b) =>
      b >= 1L << 30 ? $"{b / (double)(1L << 30):N1} GB" :
      b >= 1L << 20 ? $"{b / (double)(1L << 20):N1} MB" :
      $"{b / 1024.0:N0} KB";
  }
}

