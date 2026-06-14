using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.Todo {

  public record GetTodoByStatusReadyCommand(WeItemType TodoStatusFilter, bool ReadyFilter) : IRequest<IReadOnlyList<ReadyTodoRow>>;

  public class GetTodoByStatusReadyCommandHandler(FabricDbContext context) : IRequestHandler<GetTodoByStatusReadyCommand, IReadOnlyList<ReadyTodoRow>> {
    private readonly FabricDbContext _context = context;

    public async Task<IReadOnlyList<ReadyTodoRow>> Handle(GetTodoByStatusReadyCommand request, CancellationToken cancellationToken) {


      const int TodoType = (int)WeItemType.TodoModel;       // 1050
      const int DeskType = (int)WeItemType.DeskModel;       // 1045
      int statusFilter = (int)WeItemType.TodoNotStarted; // 221
      if (request.TodoStatusFilter >= WeItemType.TodoNotStarted && request.TodoStatusFilter <= WeItemType.TodoFailedForward) {
        statusFilter = (int)request.TodoStatusFilter;
      }      
      bool isReady = request.ReadyFilter;

      var rows = await _context.Set<ReadyTodoRow>().FromSqlInterpolated($@"
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
        JOIN dbo.ItemProperties itR  ON itR.ItemId  = it.Id AND itR.Name = 'Ready'
          AND CAST(itR.Value AS bit) = {isReady}
        WHERE it.ItemTypeId = {TodoType}
          AND CAST(itPS.Value AS int) = {statusFilter}
          AND it.IsActive = 1
        ORDER BY CAST(itPD.Value AS int) DESC, it.Established ASC")
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
