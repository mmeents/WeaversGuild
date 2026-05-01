using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Entities;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record DeleteItemCommand(int Id) : IRequest<bool>;

  public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public DeleteItemCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.Items
        .Include(i => i.Relations)
        .Include(i => i.IncomingRelations)
        .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken)
        ?? throw new KeyNotFoundException("Item not found");

      var propItemDto = await _context.GetItemDtoById(request.Id, cancellationToken);


      if (propItemDto.ItemTypeId == (int)WeItemType.EntityClassModel) {
        var libraryItem = await _mediator.Send(new GetLibDiModelCommand(item.Id), cancellationToken);
        if (libraryItem != null) {
          await _mediator.Send(new AddRemoveEntityToDbContextCommand(item.Id, false), cancellationToken);
        }
      } else if (propItemDto.ItemTypeId == (int)WeItemType.EntityPropertyModel) { 
        var propParentClassId = propItemDto.IncomingRelations.FirstOrDefault(r => r.ItemId != propItemDto.Id)?.ItemId;
        if (propParentClassId != null) {
          var entityItem = await _context.GetItemDtoById(propParentClassId.Value, cancellationToken);

          if (entityItem != null) {          
            var entityConfigRelation = entityItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityConfigurationModel).FirstOrDefault();
            if (entityConfigRelation == null) { return false; }
            var entityConfigRelationId = entityConfigRelation.RelatedItemId;
            if (entityConfigRelationId == null) { return false; }
            var entityConfigItem = await _context.GetItemDtoById(entityConfigRelationId.Value, cancellationToken);
            if (entityConfigItem == null) { return false; }
            var configProp = await GetPropConfig(entityConfigItem, item, cancellationToken);
            if (configProp != null) { 
              await _mediator.Send(new DeleteItemCommand(configProp.Id), cancellationToken);
            }
            var navItem = propItemDto.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationModel).FirstOrDefault();
            if (navItem != null && navItem.RelatedItemId.HasValue) { 
               await _mediator.Send(new DeleteItemCommand(navItem.RelatedItemId.Value), cancellationToken);
            }
          }
        }

      } else if (propItemDto.ItemTypeId == (int)WeItemType.EntityNavigationModel) {

        var entityPropParentId = propItemDto.IncomingRelations.FirstOrDefault(r => r.ItemId != propItemDto.Id)?.ItemId;
        if (entityPropParentId == null) { return false; }
        var entityClass = await _context.GetItemDtoById(entityPropParentId.Value, cancellationToken);
        var remoteEntityPropId = propItemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType)?.Value.AsInt32();
        if (remoteEntityPropId.HasValue && remoteEntityPropId.Value > 0 && entityClass != null) {
          var remoteEntityPropItem = await _context.GetItemDtoById(remoteEntityPropId.Value, cancellationToken);
          if (remoteEntityPropItem != null) { 
            var remoteClassId = remoteEntityPropItem.IncomingRelations.FirstOrDefault(r => r.ItemId != remoteEntityPropId.Value)?.ItemId;
            if (remoteClassId != null && remoteClassId.Value > 0) { 
             var cmd3 = new AddRemoveEntityInboundNavCommand(remoteClassId.Value, propItemDto.Id, entityClass.Id, false);
            await _mediator.Send(cmd3, cancellationToken);
            }
            
          }
        }


      }

      // Remove inbound relations (other items pointing here)
      _context.Relations.RemoveRange(item.IncomingRelations);

      // Walk outbound relations, cascade delete orphaned targets
      foreach (var relation in item.Relations.ToList()) {
        _context.Relations.Remove(relation);
        if (relation.RelatedItemId.HasValue)
          await CascadeIfOrphan(relation.RelatedItemId.Value, cancellationToken);
      }

      _context.Items.Remove(item);
      await _context.SaveChangesAsync(cancellationToken);
      return true;
    }

    private async Task CascadeIfOrphan(int itemId, CancellationToken cancellationToken) {
      var related = await _context.Items
          .Include(i => i.Relations)
          .Include(i => i.IncomingRelations)
          .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);
      if (related == null) return;

      // Only cascade if no remaining connections after this delete
      var remainingIncoming = related.IncomingRelations.Count(r => r.ItemId != itemId);

      if (remainingIncoming == 0 && !related.Relations.Any()) {
        _context.Items.Remove(related);
      }
    }

    private async Task<ItemDto?> GetPropConfig(ItemDto entityConfig, Item entityProp, CancellationToken cancellationToken) {
      var targetId = entityProp.Id.ToString();
      foreach (var importRel in entityConfig.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityPropertyConfigurationModel)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await _context.GetItemDtoById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            var importProp = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType && p.Value == targetId);
            if (importProp != null) {
              return importItem;
            }
          }
        }
      }
      return null;
    }

  }

}
