using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record UpdateItemCommand(
     int Id,
     int ItemTypeId,
     string Name,
     string Description,
     string Data,
     bool IsActive
   ) : IRequest<ItemDto?>;


  public class UpdateItemCommandHandler(
    FabricDbContext context, 
    IMediator mediator,
    ILogger<UpdateItemCommandHandler> logger
  ) : IRequestHandler<UpdateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context = context;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<UpdateItemCommandHandler> _logger = logger;

    public async Task<ItemDto?> Handle(UpdateItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.Items.FindAsync(request.Id);
      var nameWas = item?.Name;

      if (item == null) {
        var error = new KeyNotFoundException($"Item with id {request.Id} not found");
        _logger.LogError(error, "Failed to update item with id {ItemId}", request.Id);
        throw error;
      }

      item.Name = request.Name;
      item.Description = request.Description;
      item.Data = request.Data;
      item.ItemTypeId = request.ItemTypeId;
      item.IsActive = request.IsActive;

      _context.Items.Update(item);
      await _context.SaveChangesAsync(cancellationToken);

      var itemDto = await _context.GetItemDtoById(item.Id, cancellationToken);

      if (itemDto.ItemTypeId == (int)WeItemType.EntityClassModel && nameWas != itemDto.Name) {
        // EntityClassModel has dependent EntityConfigurationModel and DbContextEntityImportModel names that also need to change.
        var libraryItem = await _mediator.Send(new GetLibDiModelCommand(itemDto.Id), cancellationToken);
        if (libraryItem != null) { 
          var dbContextItem = await _context.ResolveDbContextFromLib(libraryItem, cancellationToken);
          if (dbContextItem != null) {
            var dbContextEntityItem = await _context.FindRelatedDbContextEntity(dbContextItem, itemDto, cancellationToken);
            if (dbContextEntityItem != null) {
             var entity = await _context.Items.FindAsync(dbContextEntityItem.Id);
              if (entity != null) {
                entity.Name = item.Name;
                _context.Items.Update(entity);
                await _context.SaveChangesAsync(cancellationToken);
              }
            }
          }
        }

        var entityConfigItemRef = itemDto.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityConfigurationModel);
        if (entityConfigItemRef != null) {
          await DoRename(entityConfigItemRef.RelatedItemId, request.Name.Trim()+"Config", cancellationToken);
        }

      } else if (itemDto.ItemTypeId == (int)WeItemType.EntityPropertyModel) {  /*----------------------  property model ------*/
        var propItem = itemDto;
        var rels = propItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationModel);
        if (rels != null && rels.RelatedItemId != null) { 
          await DoRename(rels.RelatedItemId, request.Name, cancellationToken); // child nav
        }
        var entityClassId = propItem.IncomingRelations.FirstOrDefault(r => r.ItemId != propItem.Id)?.ItemId;
        if (entityClassId != null) { 
          var entityClass = await _context.GetItemDtoById(entityClassId.Value, cancellationToken);
          var entConfigItemRef = entityClass.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityConfigurationModel);
          if (entConfigItemRef != null && entConfigItemRef.RelatedItemId != null) {
            var entConfigItem = await _context.GetItemDtoById(entConfigItemRef.RelatedItemId.Value, cancellationToken);
            if (entConfigItem != null) { 
              var configProp = await GetPropConfig(entConfigItem, itemDto, cancellationToken);
              if (configProp != null) {  // update props runs later and re adds with correct name.
                await _mediator.Send(new DeleteItemCommand(configProp.Id), cancellationToken);
              }
            }      
          }
        }        

      } 
        else if (itemDto.ItemTypeId == (int)WeItemType.EntityNavigationModel && nameWas != itemDto.Name) {
        var entityPropId = itemDto.IncomingRelations.FirstOrDefault(r => r.ItemId != itemDto.Id)?.ItemId;
        if (entityPropId == null) { return null; }
        var entityClass = await _mediator.Send(new GetEntityClassFromChildrenCommand(entityPropId.Value), cancellationToken);
        var remoteEntityId = itemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType)?.Value.AsInt32();
        if (remoteEntityId.HasValue && remoteEntityId.Value > 0 && entityClass != null) {
          var cmd3 = new AddRemoveEntityInboundNavCommand(remoteEntityId.Value, entityPropId.Value, entityClass.Id, false);
          await _mediator.Send(cmd3, cancellationToken);
        }
      }

      return itemDto;

    }

    private async Task<bool> DoRename(int? itemId, string newName, CancellationToken cancellationToken) {
      int theId = 0;
      if (itemId != null) { 
        theId = itemId.Value;
      }
      if (theId != 0) { 
        var entityConfig = await _context.Items.FindAsync(theId);
        if (entityConfig != null) {
          entityConfig.Name = newName;
          _context.Items.Update(entityConfig);
          await _context.SaveChangesAsync(cancellationToken);
        }
      } else return false;
      return true;
    }

    private async Task<ItemDto?> GetPropConfig(ItemDto entityConfig, ItemDto entityProp, CancellationToken cancellationToken) {
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
