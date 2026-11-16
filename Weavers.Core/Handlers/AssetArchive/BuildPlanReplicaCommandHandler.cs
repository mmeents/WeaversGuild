using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.AssetArchive {
  public record BuildPlanReplicaCommand(int ReplicaPolicyId) : IMcpRequest, IRequest<BuildPlanResult>;

  public class BuildPlanResult {
    public bool Success { get; set; }
    public int? NewAssetPlanId { get; set; }
    public List<string> PlanSummary { get; set; } = new List<string>();
    public string Message { get; set; } = string.Empty;
  }

  // Replica plan = set difference by CONTENT: every MediaAsset Present in the source archive
  // that is not Present in the target archive gets one Replicate command. Because it is keyed
  // on content, A->B and B->A policies converge instead of ping-ponging.
  public class BuildPlanReplicaCommandHandler : IRequestHandler<BuildPlanReplicaCommand, BuildPlanResult> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;

    private const int InsertChunk = 5000;
    private const int SampleLines = 10;

    public BuildPlanReplicaCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    private sealed class Candidate {
      public int ArchiveAssetId;
      public int MediaAssetId;
      public string RelPath = "";
      public long FileSize;
      public DateTime? CapturedLocal;
      public byte[] FileHash = Array.Empty<byte>();
      public string TargetRelPath = "";
    }

    public async Task<BuildPlanResult> Handle(BuildPlanReplicaCommand request, CancellationToken cancellationToken) {
      // ── 1. Policy ────────────────────────────────────────────────────────────
      var replicaPolicyItem = await _context.GetItemDtoById(request.ReplicaPolicyId);
      if (replicaPolicyItem == null) {
        return request.FailsWith($"Replica policy item with ID {request.ReplicaPolicyId} not found.");
      }
      if (replicaPolicyItem.ItemTypeId != (int)WeItemType.ReplicaPolicyModel) {
        return request.FailsWith($"Item with ID {request.ReplicaPolicyId} is not of type ReplicaPolicyModel.");
      }

      var replicaArchiveIdP = replicaPolicyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReplicaArchive)?.Value.AsInt32();
      if (replicaArchiveIdP is null or 0) {
        return request.FailsWith($"Replica archive ID not set on replica policy {request.ReplicaPolicyId}.");
      }
      int targetArchiveId = replicaArchiveIdP.Value;

      int sourceArchiveId;
      try {
        sourceArchiveId = replicaPolicyItem.GetParentId();
      } catch (Exception ex) {
        return request.FailsWith($"Error resolving the parent archive of policy {request.ReplicaPolicyId}: {ex.Message}");
      }
      if (sourceArchiveId == targetArchiveId) {
        return request.FailsWith("Replica archive must be different from the policy's parent archive.");
      }

      // ── 2. Both archives ────────────────────────────────────────────────────
      var archiveItems = await _mediator.Send(new GetItemsByIdsQuery(new List<int> { targetArchiveId, sourceArchiveId }), cancellationToken);
      var sourceArchiveItem = archiveItems.FirstOrDefault(i => i.Id == sourceArchiveId);
      var targetArchiveItem = archiveItems.FirstOrDefault(i => i.Id == targetArchiveId);
      if (sourceArchiveItem == null) {
        return request.FailsWith($"Source archive item with ID {sourceArchiveId} not found.");
      }
      if (targetArchiveItem == null) {
        return request.FailsWith($"Replica archive item with ID {targetArchiveId} not found.");
      }
      if (sourceArchiveItem.ItemTypeId != (int)WeItemType.AssetArchiveModel || targetArchiveItem.ItemTypeId != (int)WeItemType.AssetArchiveModel) {
        return request.FailsWith("Both ends of a replica policy must be AssetArchiveModel items.");
      }

      var sourceHarnessId = sourceArchiveItem.Properties.FirstOrDefault(p => p.Name == Cx.ItServiceHost)?.Value.AsInt32();
      var targetHarnessId = targetArchiveItem.Properties.FirstOrDefault(p => p.Name == Cx.ItServiceHost)?.Value.AsInt32();
      var targetRootFolder = targetArchiveItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value;
      var targetArchiveKind = targetArchiveItem.Properties.FirstOrDefault(p => p.Name == Cx.ItArchiveKind)?.Value.AsInt32();
      var targetArchivePlan = targetArchiveItem.Properties.FirstOrDefault(p => p.Name == Cx.ItArchivePlan)?.Value.AsInt32()
                              ?? (int)WeItemType.AsmMirror;

      // ArchiveKinds 330: AkICloudSync = 331, AkArchive = 332, AkReplica = 333, AkRemovable = 334
      // ArchiveStorageModels 340: AsmMirror = 341, AsmByDate = 342
      if (sourceHarnessId is null or 0) return request.FailsWith($"Source archive {sourceArchiveId} has no ServiceHost.");
      if (targetHarnessId is null or 0) return request.FailsWith($"Replica archive {targetArchiveId} has no ServiceHost.");
      if (string.IsNullOrWhiteSpace(targetRootFolder)) return request.FailsWith($"Replica archive {targetArchiveId} has no RootFolder.");
      if (targetArchiveKind == (int)WeItemType.AkICloudSync) {
        // Writing into an iCloud sync folder uploads to the account we're trying to drain.
        return request.FailsWith("An iCloud sync archive can't be a replica target.");
      }
      if (targetArchivePlan != (int)WeItemType.AsmMirror && targetArchivePlan != (int)WeItemType.AsmByDate) {
        return request.FailsWith($"Replica archive {targetArchiveId} has an unknown ArchivePlan ({targetArchivePlan}).");
      }

      // ── 3. One open plan per policy ─────────────────────────────────────────
      var openPlans = await _context.AssetPlans
        .Where(p => p.PolicyItemId == request.ReplicaPolicyId
                 && (p.Status == (byte)AssetPlanStatus.Draft
                  || p.Status == (byte)AssetPlanStatus.Approved
                  || p.Status == (byte)AssetPlanStatus.Running
                  || p.Status == (byte)AssetPlanStatus.Paused))
        .Select(p => new { p.Id, p.Status })
        .ToListAsync(cancellationToken);
      if (openPlans.Any(p => p.Status != (byte)AssetPlanStatus.Draft)) {
        return request.FailsWith($"Policy {request.ReplicaPolicyId} already has an approved or running plan ({openPlans.First(p => p.Status != (byte)AssetPlanStatus.Draft).Id}). Finish or cancel it first.");
      }

      // ── 4. Catalog freshness (both sides must have been scanned) ───────────
      const byte present = (byte)ArchiveAssetStatus.Present;
      var sourceLastSeen = await _context.ArchiveAssets.Where(a => a.ArchiveId == sourceArchiveId)
        .MaxAsync(a => (DateTime?)a.LastSeenUtc, cancellationToken);
      if (sourceLastSeen == null) {
        return request.FailsWith($"Source archive {sourceArchiveId} has never been synced. Sync it first.");
      }
      var targetLastSeen = await _context.ArchiveAssets.Where(a => a.ArchiveId == targetArchiveId)
        .MaxAsync(a => (DateTime?)a.LastSeenUtc, cancellationToken);

      // ── 5. Set difference: source content not present in target ───────────
      var targetMediaIds = _context.ArchiveAssets
        .Where(t => t.ArchiveId == targetArchiveId && t.Status == present)
        .Select(t => t.MediaAssetId);

      var rows = await _context.ArchiveAssets.AsNoTracking()
        .Where(s => s.ArchiveId == sourceArchiveId && s.Status == present && !targetMediaIds.Contains(s.MediaAssetId))
        .Select(s => new Candidate {
          ArchiveAssetId = s.Id,
          MediaAssetId = s.MediaAssetId,
          RelPath = s.RelPath,
          FileSize = s.Asset.FileSize,
          CapturedLocal = s.Asset.CapturedLocal,
          FileHash = s.Asset.FileHash,
        })
        .ToListAsync(cancellationToken);

      // Dups inside the source collapse to one copy (lowest ArchiveAssetId = stable choice).
      var candidates = rows
        .GroupBy(r => r.MediaAssetId)
        .Select(g => g.OrderBy(r => r.ArchiveAssetId).First())
        .OrderBy(c => c.ArchiveAssetId)
        .ToList();
      int sourceDupsCollapsed = rows.Count - candidates.Count;

      // ── 6. Target paths + collision resolution ─────────────────────────────
      foreach (var c in candidates) {
        c.TargetRelPath = targetArchivePlan == (int)WeItemType.AsmByDate
          ? ByDatePath(c.RelPath, c.CapturedLocal)
          : c.RelPath;
      }

      // Paths already used in the target (by content that is NOT being copied, by definition).
      var takenInTarget = new HashSet<string>(
        await _context.ArchiveAssets.AsNoTracking()
          .Where(t => t.ArchiveId == targetArchiveId && t.Status == present)
          .Select(t => t.RelPath)
          .ToListAsync(cancellationToken),
        StringComparer.OrdinalIgnoreCase);

      // First claimant of a path keeps the plain name; later ones (and anything hitting an
      // existing target file) get a short-hash suffix: IMG_0001~3fa9c2d1.JPG
      var claimed = new HashSet<string>(takenInTarget, StringComparer.OrdinalIgnoreCase);
      int renamed = 0;
      foreach (var c in candidates) {
        if (claimed.Add(c.TargetRelPath)) continue;
        var suffixed = WithHashSuffix(c.TargetRelPath, c.FileHash, 8);
        if (!claimed.Add(suffixed)) {
          suffixed = WithHashSuffix(c.TargetRelPath, c.FileHash, 16);
          claimed.Add(suffixed);
        }
        c.TargetRelPath = suffixed;
        renamed++;
      }

      long totalBytes = candidates.Sum(c => c.FileSize);

      // ── 7. Write plan + commands in one transaction ────────────────────────
      _context.ChangeTracker.AutoDetectChangesEnabled = false;
      int planId;
      try {
        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        // Rebuilding replaces any previous Draft for this policy (commands cascade).
        foreach (var draft in openPlans) {
          await _context.AssetPlans.Where(p => p.Id == draft.Id).ExecuteDeleteAsync(cancellationToken);
        }

        var plan = new AssetPlan {
          PolicyItemId = request.ReplicaPolicyId,
          Kind = (byte)AssetCommandKind.Replicate,
          Status = (byte)AssetPlanStatus.Draft,
          CommandCount = candidates.Count,
          Bytes = totalBytes,
          CreatedUtc = DateTime.UtcNow,
        };
        _context.AssetPlans.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);
        planId = plan.Id;
        _context.ChangeTracker.Clear();

        var now = DateTime.UtcNow;
        for (int i = 0; i < candidates.Count; i += InsertChunk) {
          foreach (var c in candidates.Skip(i).Take(InsertChunk)) {
            _context.AssetCommands.Add(new AssetCommand {
              AssetPlanId = planId,
              Kind = (byte)AssetCommandKind.Replicate,
              Status = (byte)AssetCommandStatus.Pending,
              Step = (byte)AssetCommandStep.None,
              MediaAssetId = c.MediaAssetId,
              Bytes = c.FileSize,
              SourceHarnessId = sourceHarnessId.Value,
              SourceArchiveId = sourceArchiveId,
              SourceArchiveAssetId = c.ArchiveAssetId,
              TargetHarnessId = targetHarnessId.Value,
              TargetArchiveId = targetArchiveId,
              TargetRelPath = c.TargetRelPath,
              CreatedUtc = now,
            });
          }
          await _context.SaveChangesAsync(cancellationToken);
          _context.ChangeTracker.Clear();
        }

        await tx.CommitAsync(cancellationToken);
      } finally {
        _context.ChangeTracker.AutoDetectChangesEnabled = true;
        _context.ChangeTracker.Clear();
      }

      // ── 8. Summary for review / approval ───────────────────────────────────
      var summary = new List<string> {
        $"Plan {planId} (Draft): {sourceArchiveItem.Name} -> {targetArchiveItem.Name}, layout {(targetArchivePlan == (int)WeItemType.AsmByDate ? "ByDate" : "Mirror")}",
        $"{candidates.Count:N0} files, {FormatBytes(totalBytes)} to copy",
        $"{sourceDupsCollapsed:N0} duplicate copies inside the source collapsed",
        $"{renamed:N0} target names suffixed to avoid collisions",
        $"Source last synced {sourceLastSeen:yyyy-MM-dd HH:mm} UTC; target " +
          (targetLastSeen is null ? "never synced (treated as empty)" : $"last synced {targetLastSeen:yyyy-MM-dd HH:mm} UTC"),
      };
      if (sourceHarnessId != targetHarnessId) {
        summary.Add($"Cross-machine plan: executed by harness {targetHarnessId} pulling from harness {sourceHarnessId}.");
      }
      summary.AddRange(candidates.Take(SampleLines).Select(c => $"  {c.RelPath}  ->  {c.TargetRelPath}"));
      if (candidates.Count > SampleLines) summary.Add($"  ... and {candidates.Count - SampleLines:N0} more");

      return new BuildPlanResult {
        Success = true,
        NewAssetPlanId = planId,
        PlanSummary = summary,
        Message = candidates.Count == 0
          ? $"Plan {planId} created with nothing to copy: {targetArchiveItem.Name} already holds everything in {sourceArchiveItem.Name}."
          : $"Plan {planId} created as Draft: {candidates.Count:N0} files, {FormatBytes(totalBytes)}. Review and approve to run.",
      };
    }

    // yyyy/MM/dd/name.ext. Undated -> _undated/<original rel path>.
    private static string ByDatePath(string relPath, DateTime? captured) {
      var fileName = Path.GetFileName(relPath);
      if (captured is null ) {
        return Path.Combine("_undated", relPath);
      }
      var datePart = Path.Combine(captured.Value.ToString("yyyy"), captured.Value.ToString("MM"), captured.Value.ToString("dd"));
      return Path.Combine(datePart, fileName);
    }

    private static string WithHashSuffix(string relPath, byte[] hash, int hexChars) {
      var dir = Path.GetDirectoryName(relPath) ?? "";
      var name = Path.GetFileNameWithoutExtension(relPath);
      var ext = Path.GetExtension(relPath);
      var tag = Convert.ToHexString(hash).Substring(0, Math.Min(hexChars, hash.Length * 2)).ToLowerInvariant();
      return Path.Combine(dir, $"{name}~{tag}{ext}");
    }

    private static string FormatBytes(long b) =>
      b >= 1L << 30 ? $"{b / (double)(1L << 30):N1} GB" :
      b >= 1L << 20 ? $"{b / (double)(1L << 20):N1} MB" :
      $"{b / 1024.0:N0} KB";
  }

  public static class BuildPlanResultExts {
    public static BuildPlanResult FailsWith(this BuildPlanReplicaCommand request, string message) {
      return new BuildPlanResult {
        Success = false,
        NewAssetPlanId = null,
        PlanSummary = new List<string>(),
        Message = message
      };
    }
  }
}