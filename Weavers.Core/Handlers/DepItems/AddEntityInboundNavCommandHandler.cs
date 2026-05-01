using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;



namespace Weavers.Core.Handlers.DepItems {
  public record AddRemoveEntityInboundNavCommand(int TargetEntityItemId, int AddPropertyItemId, int FromEntityItemId, bool IsAdd) : IRequest<bool>;
  public class AddEntityInboundNavCommandHandler : IRequestHandler<AddRemoveEntityInboundNavCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    private readonly ConcurrentDictionary<int, ItemDto> _cache = new ConcurrentDictionary<int, ItemDto>();

    public AddEntityInboundNavCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<bool> Handle(AddRemoveEntityInboundNavCommand request, CancellationToken cancellationToken) {

      var targetEntityItem = await _context.GetItemDtoById(request.TargetEntityItemId, cancellationToken);
      if (targetEntityItem == null) return false;
      var targetEntityConfigRelation = targetEntityItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityConfigurationModel).FirstOrDefault();
      if (targetEntityConfigRelation == null) { return false; }
      var targetEntityConfigRelationId = targetEntityConfigRelation.RelatedItemId;
      if (targetEntityConfigRelationId == null) { return false; }
      var targetEntityConfigItem = await _context.GetItemDtoById(targetEntityConfigRelationId.Value, cancellationToken);
      if (targetEntityConfigItem == null) { return false;}
      


      var fromEntityItem = await _context.GetItemDtoById(request.FromEntityItemId, cancellationToken);
      var propertyItem = await _context.GetItemDtoById(request.AddPropertyItemId, cancellationToken);      
      if (targetEntityItem == null || fromEntityItem == null || propertyItem == null) { return false; }


      var inboundEntityNavItem = await FindEntityInboundNavForProperty(targetEntityItem, propertyItem, cancellationToken);
      if (inboundEntityNavItem == null && request.IsAdd) {
        var cmd1 = new CreateRelatedItemCommand(targetEntityItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.EntityInboundNavigationModel, propertyItem.Name, "", "{}");
        inboundEntityNavItem = await _mediator.Send(cmd1, cancellationToken);

        if (inboundEntityNavItem != null) { 
          var propClassTypeProp = inboundEntityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType);
          if (propClassTypeProp != null) {
            var propClass = _context.ItemProperties.Where(p => p.Id == propClassTypeProp.Id).FirstOrDefault();
            if (propClass != null) {
              propClass.Value = targetEntityItem.Id.ToString();
              _context.ItemProperties.Update(propClass);
              await _context.SaveChangesAsync(cancellationToken);
            }
          }

          var propFkProp = inboundEntityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItForeignKey);
          if (propFkProp != null) {
            var propFk = _context.ItemProperties.Where(p => p.Id == propFkProp.Id).FirstOrDefault();
            if (propFk != null) {
              propFk.Value = propertyItem.Id.ToString();
              _context.ItemProperties.Update(propFk);
              await _context.SaveChangesAsync(cancellationToken);
            }
          }


          var propIsNullableProp = inboundEntityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
          if (propIsNullableProp != null) {
            var propIsNullable = _context.ItemProperties.Where(p => p.Id == propIsNullableProp.Id).FirstOrDefault();
            if (propIsNullable != null) {
              var isNullable = propertyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? false;
              propIsNullable.Value = isNullable.ToString();
              _context.ItemProperties.Update(propIsNullable);
              await _context.SaveChangesAsync(cancellationToken);
            }
          }

        }


      } else if (inboundEntityNavItem != null && !request.IsAdd) {
        var inboundConfigItemToDelete = await FindEntityInboundNavConfig(targetEntityConfigItem, propertyItem, cancellationToken);
        if (inboundConfigItemToDelete != null) {
          await _mediator.Send(new DeleteItemCommand(inboundConfigItemToDelete.Id), cancellationToken);
        }
        await _mediator.Send(new DeleteItemCommand(inboundEntityNavItem.Id), cancellationToken);        
      }



      if (inboundEntityNavItem != null && request.IsAdd) {            



        var inboundConfigItem = await FindEntityInboundNavConfig(targetEntityConfigItem, propertyItem, cancellationToken);
        if (inboundConfigItem == null) {         
          var cmd2 = new CreateRelatedItemCommand(targetEntityConfigItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.EntityInboundNavConfigurationModel, propertyItem.Name, "", "{}");
          inboundConfigItem = await _mediator.Send(cmd2, cancellationToken);
        }
        if (inboundConfigItem != null) {
          var paramClassTypeProp = inboundConfigItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType);
          if (paramClassTypeProp != null) {
            var propEntity = await _context.ItemProperties.FindAsync(paramClassTypeProp.Id, cancellationToken);
            if (propEntity != null) {
              propEntity.Value = propertyItem.Id.ToString();
              _context.ItemProperties.Update(propEntity);
              await _context.SaveChangesAsync(cancellationToken);
            }
          }
        

          var paramIsNullableProp = inboundConfigItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
          if (paramIsNullableProp != null) {
            var propEntity = await _context.ItemProperties.FindAsync(paramIsNullableProp.Id, cancellationToken);
            if (propEntity != null) {
              var isNullable = propertyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? false;
              propEntity.Value = isNullable.ToString();
              _context.ItemProperties.Update(propEntity);
              await _context.SaveChangesAsync(cancellationToken);
            }
          }                    
        }   
      } 
      return true;
    }

    
    private async Task<ItemDto?> FindEntityInboundNavForProperty(ItemDto entityItem, ItemDto propItem, CancellationToken cancellationToken) {
      var targetId = propItem.Id.ToString();
      foreach (var importRel in entityItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityInboundNavigationModel)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await GetCachedItemById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            var importProp =  importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItForeignKey && p.Value == targetId);
            if (importProp != null) { 
                return importItem;
            }
          }
        }
      }
      return null;
    }

    private async Task<ItemDto?> FindEntityInboundNavConfig(ItemDto entityConfigItem, ItemDto propItem, CancellationToken cancellationToken) {      
      if (entityConfigItem == null) { return null; }
      var targetId = propItem.Id.ToString();
      foreach (var importRel in entityConfigItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityInboundNavConfigurationModel)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await GetCachedItemById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            var importProp =  importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType && p.Value == targetId);
            if (importProp != null) { 
                return importItem;
            }
          }
        }
      }
      return null;
    }

    private async Task<ItemDto> GetCachedItemById(int ItemId, CancellationToken cancellationToken) {
      if (_cache.Keys.Contains(ItemId)) {
        var item = _cache[ItemId];
        return item;
      } else {
        var item = await _context.GetItemDtoById(ItemId, cancellationToken);
        _cache[item.Id] = item;
        return item;
      }
    }


  }
}