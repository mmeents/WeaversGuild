using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Entities;

namespace Weavers.Core.Handlers.AssetArchive {
  public record GetPlanBoardQuery(byte[] PlanStatuses, int PerPlanLimit = 200) : IRequest<PlanBoard>;

  public sealed class PlanBoard {
    public List<PlanRow> Plans { get; set; } = new();
    public Dictionary<int, List<CommandRow>> CommandsByPlan { get; set; } = new();
  }

  public sealed class PlanRow {
    public int Id { get; set; }
    public int PolicyItemId { get; set; }
    public string PolicyName { get; set; } = "";
    public byte Kind { get; set; }
    public byte Status { get; set; }
    public int CommandCount { get; set; }
    public long Bytes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? ApprovedUtc { get; set; }
    public DateTime? StartedUtc { get; set; }
    public DateTime? DoneUtc { get; set; }
    public string? PlanSummary { get; set; }
    public string? ResultSummary { get; set; }
    // live counts
    public int PendingCnt { get; set; }
    public int ClaimedCnt { get; set; }
    public int DoneCnt { get; set; }
    public int FailedCnt { get; set; }
    public int SkippedCnt { get; set; }
    public long DoneBytes { get; set; }
  }

  public sealed class CommandRow {
    public int Id { get; set; }
    public int AssetPlanId { get; set; }
    public byte Status { get; set; }
    public long Bytes { get; set; }
    public string SourceRelPath { get; set; } = "";
    public string TargetRelPath { get; set; } = "";
    public int Attempts { get; set; }
    public string? ClaimedBy { get; set; }
    public DateTime? NotBeforeUtc { get; set; }
    public string? LastError { get; set; }
  }

  public sealed class GetPlanBoardQueryHandler(FabricDbContext db) : IRequestHandler<GetPlanBoardQuery, PlanBoard> {
    public async Task<PlanBoard> Handle(GetPlanBoardQuery q, CancellationToken ct) {
      var statuses = q.PlanStatuses;

      var plans = await db.AssetPlans.AsNoTracking()
        .Where(p => statuses.Contains(p.Status))
        .OrderByDescending(p => p.Id)
        .Select(p => new PlanRow {
          Id = p.Id,
          PolicyItemId = p.PolicyItemId,
          PolicyName = p.PolicyItem.Name,
          Kind = p.Kind,
          Status = p.Status,
          CommandCount = p.CommandCount,
          Bytes = p.Bytes,
          CreatedUtc = p.CreatedUtc,
          ApprovedUtc = p.ApprovedUtc,
          StartedUtc = p.StartedUtc,
          DoneUtc = p.DoneUtc,
          PlanSummary = p.PlanSummary,
          ResultSummary = p.ResultSummary,
        })
        .ToListAsync(ct);
      if (plans.Count == 0) return new PlanBoard();

      var ids = plans.Select(p => p.Id).ToList();

      // Live counts per plan (IX_AssetCommands_Plan_Status covers this).
      var counts = await db.AssetCommands.AsNoTracking()
        .Where(c => ids.Contains(c.AssetPlanId))
        .GroupBy(c => new { c.AssetPlanId, c.Status })
        .Select(g => new { g.Key.AssetPlanId, g.Key.Status, Cnt = g.Count(), Bytes = g.Sum(c => c.Bytes) })
        .ToListAsync(ct);

      foreach (var p in plans) {
        foreach (var c in counts.Where(c => c.AssetPlanId == p.Id)) {
          switch ((AssetCommandStatus)c.Status) {
            case AssetCommandStatus.Pending: p.PendingCnt = c.Cnt; break;
            case AssetCommandStatus.Claimed: p.ClaimedCnt = c.Cnt; break;
            case AssetCommandStatus.Done: p.DoneCnt = c.Cnt; p.DoneBytes = c.Bytes; break;
            case AssetCommandStatus.Failed: p.FailedCnt = c.Cnt; break;
            case AssetCommandStatus.Skipped: p.SkippedCnt = c.Cnt; break;
          }
        }
      }

      // Capped window per plan: Failed, Claimed, Pending, then Done/Skipped; Id order within.
      var idList = string.Join(",", ids);   // ints only, safe to inline
      var sql = $@"
      WITH w AS (
        SELECT c.Id, c.AssetPlanId, c.Status, c.Bytes, s.RelPath AS SourceRelPath, c.TargetRelPath,
          c.Attempts, c.ClaimedBy, c.NotBeforeUtc, c.LastError,
          ROW_NUMBER() OVER (PARTITION BY c.AssetPlanId
            ORDER BY CASE c.Status WHEN 3 THEN 0 WHEN 1 THEN 1 WHEN 0 THEN 2 ELSE 3 END, c.Id) AS rn
        FROM dbo.AssetCommands c
        JOIN dbo.ArchiveAssets s ON s.Id = c.SourceArchiveAssetId
        WHERE c.AssetPlanId IN ({idList}))
      SELECT Id, AssetPlanId, Status, Bytes, SourceRelPath, TargetRelPath, Attempts, ClaimedBy, NotBeforeUtc, LastError
      FROM w WHERE rn <= @lim
      ORDER BY AssetPlanId DESC, rn;";

      var rows = await db.Database.SqlQueryRaw<CommandRow>(sql, new SqlParameter("@lim", q.PerPlanLimit))
        .ToListAsync(ct);

      return new PlanBoard {
        Plans = plans,
        CommandsByPlan = rows.GroupBy(r => r.AssetPlanId).ToDictionary(g => g.Key, g => g.ToList()),
      };
    }
  }
}
