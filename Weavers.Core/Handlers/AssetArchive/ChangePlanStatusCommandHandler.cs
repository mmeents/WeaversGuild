using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using Weavers.Core.Handlers.Pipeline;

namespace Weavers.Core.Handlers.AssetArchive {

  // ─────────────────────────────────────────────────────────────────────────────
  // All plan status changes go through here, so the transition rules live in one place.
  //
  //   Approve : Draft                        -> Approved   (empty plan -> Done)
  //   Pause   : Approved | Running           -> Paused
  //   Resume  : Paused                       -> Approved   (first worker claim moves it to Running)
  //   Cancel  : Draft | Approved | Running | Paused -> Cancelled
  //   Delete  : Draft | Done | Failed | Cancelled   -> row removed (commands cascade)
  //
  // Every write is conditional on the status the rules expect (WHERE Status IN ...), so two
  // instances clicking at once, or a worker closing the plan mid-click, can't clobber each other.
  // Workers already ignore plans that aren't Approved/Running, so Pause/Cancel take effect at the
  // next claim; commands already claimed finish normally.
  // ─────────────────────────────────────────────────────────────────────────────

  public enum PlanAction : byte { Approve = 1, Pause = 2, Resume = 3, Cancel = 4, Delete = 5 }

  public record ChangePlanStatusCommand(int PlanId, PlanAction Action, bool Force = false) : IMcpRequest, IRequest<PlanActionResult>;

  public class PlanActionResult {
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    /// <summary>Approve refused because an archive was synced after the plan was built. Resend with Force or rebuild.</summary>
    public bool IsStale { get; set; }
    public int PolicyItemId { get; set; }          // so the UI can offer "Rebuild" without another lookup
    public AssetPlanStatus? NewStatus { get; set; } // null after Delete
  }

  public class ChangePlanStatusCommandHandler(FabricDbContext db) : IRequestHandler<ChangePlanStatusCommand, PlanActionResult> {
    private static readonly Dictionary<PlanAction, (AssetPlanStatus[] From, AssetPlanStatus? To)> Rules = new() {      
      [PlanAction.Approve] = (new[] { AssetPlanStatus.Draft, AssetPlanStatus.Cancelled }, AssetPlanStatus.Approved),
      [PlanAction.Pause] = (new[] { AssetPlanStatus.Approved, AssetPlanStatus.Running }, AssetPlanStatus.Paused),
      [PlanAction.Resume] = (new[] { AssetPlanStatus.Paused }, AssetPlanStatus.Approved),
      [PlanAction.Cancel] = (new[] { AssetPlanStatus.Draft, AssetPlanStatus.Approved, AssetPlanStatus.Running, AssetPlanStatus.Paused }, AssetPlanStatus.Cancelled),
      [PlanAction.Delete] = (new[] { AssetPlanStatus.Draft, AssetPlanStatus.Done, AssetPlanStatus.Failed, AssetPlanStatus.Cancelled }, null),
    };

    public async Task<PlanActionResult> Handle(ChangePlanStatusCommand request, CancellationToken ct) {
      var plan = await db.AssetPlans.AsNoTracking()
        .Where(p => p.Id == request.PlanId)
        .Select(p => new { p.Id, p.Status, p.PolicyItemId, p.CommandCount, p.CreatedUtc })
        .FirstOrDefaultAsync(ct);
      if (plan is null) return Fail($"Plan {request.PlanId} not found.");
      if (!Rules.TryGetValue(request.Action, out var rule)) return Fail($"Unknown action {request.Action}.");

      var current = (AssetPlanStatus)plan.Status;
      if (!rule.From.Contains(current)) {
        return Fail($"Can't {request.Action} plan {plan.Id} while it is {current}." +
          (request.Action == PlanAction.Delete ? " Cancel it first." : ""), plan.PolicyItemId);
      }
      var fromBytes = rule.From.Select(s => (byte)s).ToArray();

      // ── Delete ─────────────────────────────────────────────────────────────
      if (request.Action == PlanAction.Delete) {
        var deleted = await db.AssetPlans
          .Where(p => p.Id == plan.Id && fromBytes.Contains(p.Status))
          .ExecuteDeleteAsync(ct);                         // commands cascade
        return deleted == 1
          ? new PlanActionResult { Success = true, Message = $"Plan {plan.Id} deleted.", PolicyItemId = plan.PolicyItemId }
          : Fail($"Plan {plan.Id} changed status before it could be deleted; refresh and try again.", plan.PolicyItemId);
      }

      // ── Approve: staleness check + empty-plan shortcut ─────────────────────
      if (request.Action == PlanAction.Approve) {
        if (plan.CommandCount == 0) {
          var now0 = DateTime.UtcNow;
          var n0 = await db.AssetPlans
            .Where(p => p.Id == plan.Id && p.Status == (byte)AssetPlanStatus.Draft)
            .ExecuteUpdateAsync(s => s
              .SetProperty(p => p.Status, (byte)AssetPlanStatus.Done)
              .SetProperty(p => p.ApprovedUtc, now0)
              .SetProperty(p => p.StartedUtc, now0)
              .SetProperty(p => p.DoneUtc, now0)
              .SetProperty(p => p.ResultSummary, "Nothing to copy; target already held everything."), ct);
          return n0 == 1
            ? Ok(plan.Id, AssetPlanStatus.Done, plan.PolicyItemId, $"Plan {plan.Id} had nothing to copy and was closed.")
            : Fail($"Plan {plan.Id} changed status; refresh and try again.", plan.PolicyItemId);
        }

        if (!request.Force) {
          var stale = await SyncedSinceAsync(plan.Id, plan.CreatedUtc, ct);
          if (stale is not null) {
            return new PlanActionResult {
              Success = false,
              IsStale = true,
              PolicyItemId = plan.PolicyItemId,
              Message = $"Archive {stale} was synced after plan {plan.Id} was built ({plan.CreatedUtc.ToLocalTime():g}). " +
                        "Rebuild the plan, or approve anyway.",
            };
          }
        }
      }

      // ── Status flip (Approve / Pause / Resume / Cancel) ────────────────────
      var to = (byte)rule.To!.Value;
      var now = DateTime.UtcNow;
      int n = request.Action switch {
        PlanAction.Approve => await db.AssetPlans
          .Where(p => p.Id == plan.Id && fromBytes.Contains(p.Status))
          .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, to).SetProperty(p => p.ApprovedUtc, now), ct),

        PlanAction.Cancel => await db.AssetPlans
          .Where(p => p.Id == plan.Id && fromBytes.Contains(p.Status))
          .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, to).SetProperty(p => p.DoneUtc, now), ct),

        _ => await db.AssetPlans
          .Where(p => p.Id == plan.Id && fromBytes.Contains(p.Status))
          .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, to), ct),
      };

      return n == 1
        ? Ok(plan.Id, rule.To.Value, plan.PolicyItemId, $"Plan {plan.Id}: {current} -> {rule.To.Value}.")
        : Fail($"Plan {plan.Id} changed status before the update; refresh and try again.", plan.PolicyItemId);
    }

    /// <summary>Returns the id of an archive (source or target) synced after the plan was built, else null.</summary>
    private async Task<int?> SyncedSinceAsync(int planId, DateTime planCreatedUtc, CancellationToken ct) {
      var ends = await db.AssetCommands.AsNoTracking()
        .Where(c => c.AssetPlanId == planId)
        .Select(c => new { c.SourceArchiveId, c.TargetArchiveId })
        .FirstOrDefaultAsync(ct);
      if (ends is null) return null;

      // CreatedUtc and LastSeenUtc are both datetime2(0); 2s of slack covers rounding.
      var cutoff = planCreatedUtc.AddSeconds(2);
      foreach (var archiveId in new[] { ends.SourceArchiveId, ends.TargetArchiveId }) {
        var touched = await db.ArchiveAssets
          .AnyAsync(a => a.ArchiveId == archiveId && a.LastSeenUtc > cutoff, ct);
        if (touched) return archiveId;
      }
      return null;
    }

    private static PlanActionResult Ok(int planId, AssetPlanStatus status, int policyId, string msg) =>
      new() { Success = true, NewStatus = status, PolicyItemId = policyId, Message = msg };

    private static PlanActionResult Fail(string msg, int policyId = 0) =>
      new() { Success = false, Message = msg, PolicyItemId = policyId };
  }
}
