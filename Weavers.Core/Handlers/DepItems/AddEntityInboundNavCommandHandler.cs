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
using Weavers.Core.Service;



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

      var targetEntityItem = await _context.GetItemDtoById(request.TargetEntityItemId, cancellationToken);  // entity class
      if (targetEntityItem == null || targetEntityItem.Relations == null) return false;

      var parentItemId = targetEntityItem?.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains)?.ItemId; // look up parent.     

      var fromEntityItem = await _context.GetItemDtoById(request.FromEntityItemId, cancellationToken);  // from entity class.      
      var entityNameProp = fromEntityItem?.Properties.FirstOrDefault(p => p.Name == Cx.ItDbTableName);
      var entityTableDisplayName = entityNameProp != null ? entityNameProp.Value : fromEntityItem?.Name ?? "UnknownEntity";      

      var propertyItem = await _context.GetItemDtoById(request.AddPropertyItemId, cancellationToken);   // property added  
      var originalNavItem = await GetEntityNavForProperty(propertyItem, cancellationToken);             // propertys navigation item.
      if (targetEntityItem == null || fromEntityItem == null || propertyItem == null || parentItemId == null|| originalNavItem == null) { return false; }


      var origalNavTypeProp = originalNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation);
      WeItemType navType = WeItemType.NavHasOneToOne;
      if (origalNavTypeProp != null) { 
        var navTypeValue = int.TryParse(origalNavTypeProp.Value, out var result) ? result : (int?)navType;
        if (navTypeValue != null) { 
          navType = (WeItemType)navTypeValue.Value;          
        }
      }

      var inboundEntityNavItem = await FindEntityInboundNavForProperty(targetEntityItem, propertyItem, cancellationToken);
      if ( request.IsAdd) {
        if (inboundEntityNavItem == null) {
          var cmd1 = new CreateRelatedItemCommand(targetEntityItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.EntityInboundNavigationModel, propertyItem.Name, "", "{}");
          inboundEntityNavItem = await _mediator.Send(cmd1, cancellationToken);
        } 
        
        var dirty = false;
        if (inboundEntityNavItem != null) {

          var propClassTypeProp = inboundEntityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType);
          if (propClassTypeProp != null) {
            var propClass = _context.ItemProperties.Where(p => p.Id == propClassTypeProp.Id).FirstOrDefault();
            if (propClass != null) {
              propClass.Value = fromEntityItem.Id.ToString();
              _context.ItemProperties.Update(propClass);
              dirty = true;
            }
          }

          var propFkProp = inboundEntityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItForeignKey);
          if (propFkProp != null) {
            var propFk = _context.ItemProperties.FirstOrDefault(p => p.Id == propFkProp.Id);
            if (propFk != null) {
              string fkValueToSet = propertyItem.Id.ToString();
              if (propFk.Value != fkValueToSet) {
                propFk.Value = fkValueToSet;
                _context.ItemProperties.Update(propFk);
                dirty = true;
              }
            }
          }
          
        

          var propIsNullableProp = inboundEntityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
          if (propIsNullableProp != null) {
            var propIsNullable = _context.ItemProperties.Where(p => p.Id == propIsNullableProp.Id).FirstOrDefault();
            if (propIsNullable != null) {
              var isNullable = propertyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? false;
              propIsNullable.Value = isNullable.ToString();
              _context.ItemProperties.Update(propIsNullable);
              dirty = true;
            }
          }

          var propNavTypeProp = inboundEntityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation);
          WeItemType inboundNavType = navType;
          if (propNavTypeProp != null) {
            var propNavType = _context.ItemProperties.Where(p => p.Id == propNavTypeProp.Id).FirstOrDefault();
            if (propNavType != null) {

              inboundNavType = navType switch {
                WeItemType.NavHasOneToMany => WeItemType.NavHasManyToOne,   // Collection on one side → Reference on the other
                WeItemType.NavHasManyToOne => WeItemType.NavHasOneToMany,   // Reference on one side → Collection on the other
                WeItemType.NavHasOneToOne => WeItemType.NavHasOneToOne,
                WeItemType.NavHasManyToMany => WeItemType.NavHasManyToMany,
                _ => navType
              };

              propNavType.Value = ((int)inboundNavType).ToString();
              _context.ItemProperties.Update(propNavType);
              dirty = true;
            }
          }

          var inverseProp = inboundEntityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItInverseNavigation);
          if (inverseProp != null && originalNavItem != null) {
            var invPropEntity = _context.ItemProperties.Where(p => p.Id == inverseProp.Id).FirstOrDefault();
            if (invPropEntity != null) {
              // The name the user actually typed on the other side is usually the best inverse name
              string inverseName = originalNavItem.Name;        // e.g. "Orders" or "Customer"
              if (inboundNavType == WeItemType.NavHasManyToOne || inboundNavType == WeItemType.NavHasOneToOne) { 
                inverseName = entityTableDisplayName;
              } 
                
              invPropEntity.Value = inverseName;
              _context.ItemProperties.Update(invPropEntity);
              dirty = true;
            }
          }



        }

        if (dirty) {
          await _context.SaveChangesAsync(cancellationToken);
        }

      } else if (inboundEntityNavItem != null && !request.IsAdd) {  // its a remove, we need to remove both the inbound nav and its config if exists
        await _mediator.Send(new DeleteItemCommand(inboundEntityNavItem.Id), cancellationToken);
      }


      if (parentItemId != null) { // notify system that entity changed and should refresh
        await _mediator.Publish(new ItemUpdatedNotification(parentItemId.Value), cancellationToken);
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

    private async Task<ItemDto?> GetEntityNavForProperty(ItemDto propItem, CancellationToken cancellationToken) {
      foreach (var importRel in propItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationModel)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await GetCachedItemById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            return importItem;
          }
        }
      }
      return null;
    }



  }
}