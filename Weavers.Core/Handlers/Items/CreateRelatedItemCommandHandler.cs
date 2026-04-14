using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Entities;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Microsoft.Extensions.Logging;


namespace Weavers.Core.Handlers.Items {
  public record CreateRelatedItemCommand(
     int ParentItemId,
     int RelationTypeId,
     int ItemTypeId,
     string ItemName,
     string ItemDescription,
     string ItemData
   ) : IRequest<ItemDto?>;

  public class CreateRelatedItemCommandHandler(
    FabricDbContext context,
    ILogger<CreateRelatedItemCommandHandler> logger
  ) : IRequestHandler<CreateRelatedItemCommand, ItemDto?> {
    private readonly FabricDbContext _context = context;
    private readonly ILogger<CreateRelatedItemCommandHandler> _logger = logger;
    public async Task<ItemDto?> Handle(CreateRelatedItemCommand request, CancellationToken cancellationToken) {

      if (request.ParentItemId <= 0) return null;

      var parentExists = await _context.Items.AnyAsync(i => i.Id == request.ParentItemId && i.IsActive, cancellationToken);
      if (!parentExists) {
        _logger.LogError("Parent item with id {ParentItemId} not found", request.ParentItemId);
        throw new Exception($"Parent item with id {request.ParentItemId} not found");
      }

      var itemType = await _context.ItemTypes.FindAsync([request.ItemTypeId], cancellationToken);
      if (itemType == null) {
        _logger.LogError("ItemType with id {ItemTypeId} not found", request.ItemTypeId);
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
        await _context.SyncDefaultsByModelIdAsync(newRelatedItem.Id, newRelatedItem.ItemTypeId, cancellationToken);
        var nextRank = await _context.GetItemsNextRankId(request.ParentItemId, cancellationToken);

        _context.Relations.Add(new Relation {
          ItemId = request.ParentItemId,
          RelationTypeId = request.RelationTypeId,
          RelatedItemId = newRelatedItem.Id,
          Rank = nextRank
        });

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
      } catch {
        await transaction.RollbackAsync(cancellationToken);
        _logger.LogError("Failed to create related item for parent item {ParentItemId} with relation type {RelationTypeId}", request.ParentItemId, request.RelationTypeId);
        throw;
      }

      try {
        return await _context.GetItemDtoById(newRelatedItem.Id, cancellationToken);
      } catch (Exception ex) {
        _logger.LogError(ex, "Failed to retrieve newly created related item with id {ItemId}", newRelatedItem.Id);
        throw new Exception($"Failed to retrieve newly created related item with id {newRelatedItem.Id}");
      }

    }
  }
}
