using KB.Core.Entities;
using KB.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.Items {
  public record CreateRelatedItemCommand(
     int ParentItemId,
     int RelationTypeId,
     int ItemTypeId,
     string ItemName,
     string ItemDescription,
     string ItemData
   ) : IRequest<ItemDto?>;

  public class CreateRelatedItemCommandHandler(FabricDbContext context) : IRequestHandler<CreateRelatedItemCommand, ItemDto?> {
    private readonly FabricDbContext _context = context;
    public async Task<ItemDto?> Handle(CreateRelatedItemCommand request, CancellationToken cancellationToken) {

      if (request.ParentItemId <= 0) return null;

      var parentExists = await _context.Items.AnyAsync(i => i.Id == request.ParentItemId && i.IsActive, cancellationToken);
      if (!parentExists) throw new Exception($"Parent item with id {request.ParentItemId} not found");


      var itemType = await _context.ItemTypes.FindAsync(new object[] { request.ItemTypeId }, cancellationToken);
      if (itemType == null) {
        throw new Exception($"ItemType with id {request.ItemTypeId} not found");
      }

      var newRelatedItem = new Item {
        Name = request.ItemName,
        ItemTypeId = itemType.Id,
        Description = request.ItemDescription,
        Data = request.ItemData,
        IsActive = true
      };

      using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
      try {
        _context.Items.Add(newRelatedItem);
        await _context.SaveChangesAsync(cancellationToken);

        var nextRank = await _context.ItemRelations
          .Where(ir => ir.ItemId == request.ParentItemId)
          .CountAsync(cancellationToken) + 1;

        _context.ItemRelations.Add(new ItemRelation {
          ItemId = request.ParentItemId,
          RelationTypeId = request.RelationTypeId,
          RelatedItemId = newRelatedItem.Id,
          Rank = nextRank
        });
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
      } catch {
        await transaction.RollbackAsync(cancellationToken);
        throw;
      }

      return await _context.Items
        .AsNoTracking()
        .Where(i => i.Id == newRelatedItem.Id && i.IsActive)
        .Include(i => i.ItemType)
        .Include(i => i.Relations)
            .ThenInclude(r => r.RelatedItem)
        .Include(i => i.Relations)
            .ThenInclude(r => r.RelationType)
        .Include(i => i.IncomingRelations)
            .ThenInclude(r => r.Item)
        .Include(i => i.IncomingRelations)
            .ThenInclude(r => r.RelationType)
        .FirstOrDefaultAsync(cancellationToken)
        .ContinueWith(t => t.Result?.ToDto(true), cancellationToken);
    }
  }
}
