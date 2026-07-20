using MediatR;
using Weavers.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.Todo {

  public record GetTodoByStatusReadyCommand(int HarnessId, WeItemType TodoStatusFilter, bool ReadyFilter) : IRequest<IReadOnlyList<ReadyTodoRow>>;

  public class GetTodoByStatusReadyCommandHandler(
    FabricDbContext context
    ) : IRequestHandler<GetTodoByStatusReadyCommand, IReadOnlyList<ReadyTodoRow>> {
    private readonly FabricDbContext _context = context;

    public async Task<IReadOnlyList<ReadyTodoRow>> Handle(GetTodoByStatusReadyCommand request, CancellationToken cancellationToken) {
      var harnessId = request.HarnessId;
      const int HarnessType = (int)WeItemType.HarnessAppModel;   // 1010
      const int PresenceLmStudioGatewayModel = (int)WeItemType.PresenceLmStudioGatewayModel;   // LM Studio instance details. enough to query the models.
      const int PresenceClaudeGatewayModel = (int)WeItemType.PresenceClaudeGatewayModel;   // Claude instance details. enough to query the models.
      const int PresModelLmStudioModel = (int)WeItemType.PresModelLmStudioModel;     // LmStudio model for each model found.
      const int PresModelClaudeModel = (int)WeItemType.PresModelClaudeModel;     // Claude model for each model found.

      const int TodoType = (int)WeItemType.TodoModel;       // 1050
      const int DeskType = (int)WeItemType.DeskModel;       // 1045
      int statusFilter = (int)WeItemType.TodoNotStarted; // 221
      if (request.TodoStatusFilter >= WeItemType.TodoNotStarted && request.TodoStatusFilter <= WeItemType.TodoFailedForward) {
        statusFilter = (int)request.TodoStatusFilter;
      }      
      string isReady = request.ReadyFilter ? "1" : "0";
      string gwlWhereClause = $"where itHarness.ItemTypeId = {HarnessType}";
      if (isReady == "1" && statusFilter == (int)WeItemType.TodoNotStarted) {
        gwlWhereClause += $" and itHarness.Id = {harnessId}";
      }

      var sql = $@"
        with gwl (Id) as (
          select 
            rel2.RelatedItemId GateWayId
          from dbo.Items itHarness  
            inner join dbo.Relations rel on rel.ItemId = itHarness.Id
            inner join dbo.Items itGateways on itGateways.Id = rel.RelatedItemId
            inner join dbo.Relations rel2 on rel2.ItemId = itGateways.Id
            inner join dbo.Items itGateWay on itGateWay.Id = rel2.RelatedItemId and (itGateWay.ItemTypeId = {PresenceLmStudioGatewayModel} or itGateWay.ItemTypeId = {PresenceClaudeGatewayModel})
          {gwlWhereClause}
        )
        SELECT distinct it.Id, 
          it.ItemTypeId, 
          it.Name,
          desk.Name AS DeskName, 
          desk.ItemTypeId AS DeskItemTypeId,
          it.Established,
          CAST(itPD.Value AS int) AS TodoDepth
        FROM dbo.Items it
        JOIN dbo.Relations pr      ON pr.RelatedItemId = it.Id
        JOIN dbo.Items desk        ON pr.ItemId = desk.Id AND desk.ItemTypeId = {DeskType}
        JOIN dbo.ItemProperties itPD ON itPD.ItemId = it.Id AND itPD.Name = 'TodoDepth'
        JOIN dbo.ItemProperties itPS ON itPS.ItemId = it.Id AND itPS.Name = 'Status'
        JOIN dbo.ItemProperties itR  ON itR.ItemId  = it.Id AND itR.Name = 'Ready' AND CAST(itR.Value AS bit) = {isReady}

        Join dbo.ItemProperties itO ON desk.Id = itO.ItemId and ItO.Name = 'Operator'
        join dbo.Items itOm on itOm.Id = cast( itO.Value as int)
        join dbo.ItemProperties itOmp on itOmp.ItemId = itOm.Id and ItOmp.Name ='Presence'
        join dbo.Items itPr on itPr.Id = cast( itOmp.Value as int) 
          and (itPr.ItemTypeId = {PresModelLmStudioModel} or itPr.ItemTypeId = {PresModelClaudeModel})
        join dbo.Relations hr on hr.RelatedItemId = itPr.Id 
        join dbo.Items itGw on itGw.Id = hr.ItemId 
          and (itGw.ItemTypeId={PresenceLmStudioGatewayModel} or itGw.ItemTypeId={PresenceClaudeGatewayModel})

        WHERE it.ItemTypeId = {TodoType}
          AND CAST(itPS.Value AS int) = {statusFilter}
          AND it.IsActive = 1
          and itGw.Id in (select id from gwl)
        ORDER BY it.Id ASC";

      var rows = await _context.Set<ReadyTodoRow>().FromSqlRaw(sql)
      .AsNoTracking()
      .ToListAsync(cancellationToken);

      return rows;
    }
  }


  public record ReadyTodoRow {
    public int Id { get; init; }
    public int ItemTypeId { get; init; }
    public string Name { get; init; } = "";
    public string DeskName { get; init; } = "";
    public int DeskItemTypeId { get; init; }
    public DateTime Established { get; init; }
    public int TodoDepth { get; init; }   // pulled into the projection so the UI can show/sort it
  }
}
