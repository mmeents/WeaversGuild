using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.DepItems {

  public record ProcessPropertyUpdateCommand(ItemDto EntityItem, ItemDto PropertyItem) : IRequest<bool>;
  public class ProcessPropertyUpdateCommandHandler : IRequestHandler<ProcessPropertyUpdateCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    private readonly ConcurrentDictionary<int, ItemDto> _cache = new ConcurrentDictionary<int, ItemDto>();

    public ProcessPropertyUpdateCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<bool> Handle(ProcessPropertyUpdateCommand request, CancellationToken cancellationToken) {
      // Implement the logic for processing the property update here
      // current WeItemType enums
      /*
       *EntityClassModel = 600,
         EntityClassImportModel = 605,
         EntityPropertyModel = 610,      <- property is imput. if nav bool is true, then add or verify nav, if false remove nav if exists.
         EntityNavigationModel = 614,    <- found by only child of prop, user picks a class in ItPropertyClassType property of the nav item. that is the class this nav points to. if nav bool is false, remove this item if exists.  
         EntityInboundNavigationModel = 616,  <- gets added when a nav is added to point back to the entity from the target of the nav. gets removed when nav is removed.
        EntityConfigurationModel = 620,  <-  only one per cass, nothing changes here. but used to hold the configs below as children.
         EntityPropertyConfigurationModel = 622,  <- below have parameter class type pointing to the property.
         EntityNavigationConfigurationModel = 624,
         EntityInboundNavConfigurationModel = 626,
       */
      // 1. Validate the input data (EntityItem and PropertyItem)
      // 2. load both items from the database using their IDs 
      // 3. Check property states of the property to see if it's a nav bool is true.
      // 4. If the property is a nav, add or verify the EntityNavigationModel exists for the property.
      // 5. If the property is not a nav, verify remove nav
      // 6. If the property is a nav, update the navigation Configuration related items to match. (that is check for propertyConfigModel and remove, confirm or add navigationConfigModel )
      // 7. If the property is not a nav, add remove verify the property config models match the property state in nav config.
      // 8. All good, return true.

      if (request.EntityItem == null || request.PropertyItem == null) {        
        return false;

      }
      var propertyItem = await _context.GetItemDtoById(  request.PropertyItem.Id, cancellationToken);  // maybe fresh copies.
      var entityItem = await _context.GetItemDtoById(  request.EntityItem.Id, cancellationToken);
      if (propertyItem == null || entityItem == null) {        
        return false;
      }
      if (propertyItem.ItemTypeId != (int)WeItemType.EntityPropertyModel) { return false; }
      if (entityItem.ItemTypeId != (int)WeItemType.EntityClassModel) { return false; }      

      // should only be one config.
      var configItemId = entityItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityConfigurationModel)?.RelatedItemId ?? 0;
      ItemDto? entityConfigItem = null;
      if (configItemId > 0) { 
        entityConfigItem = await GetCachedItemById(configItemId, cancellationToken);
      }
      if (entityConfigItem == null) { return false; }

                  
      var isNavProp = propertyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation)?.Value.AsBoolean() ?? false;
      var entityNavItem = await GetEntityNavForProperty(propertyItem, cancellationToken);
      bool propIsNullable = propertyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? false;

      if (isNavProp) { // is a nav
        
        if (entityNavItem == null) {  // add the navigation if not there as child.
          entityNavItem = await _mediator.Send(new CreateRelatedItemCommand(propertyItem.Id, 
            (int)WeRelationTypes.Contains, (int)WeItemType.EntityNavigationModel, propertyItem.Name, "", "{}"));
          if (entityNavItem != null) {                         
            var nullableProp = entityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
            if (nullableProp != null && nullableProp.Value.AsBoolean() != propIsNullable) {
              var entity = await _context.ItemProperties.FindAsync(nullableProp.Id, cancellationToken);
              if (entity != null) { entity.Value = propIsNullable.ToString(); }
            }
          }
        } 

        // add config for nav
        var navConfig = await GetNavConfig(entityConfigItem, propertyItem, cancellationToken);
        if (navConfig == null) {
          navConfig = await _mediator.Send(new CreateRelatedItemCommand(entityConfigItem.Id,
            (int)WeRelationTypes.Contains, (int)WeItemType.EntityNavigationConfigurationModel, propertyItem.Name, "", "{}"));
          if (navConfig != null) {
            await SyncNullableAndType(navConfig, propIsNullable, propertyItem?.Id, cancellationToken);      
          }
        } else {
          if (navConfig.Name != propertyItem.Name) {
            navConfig.Name = propertyItem.Name;
            await _mediator.Send(new UpdateItemCommand(navConfig.Id, navConfig.ItemTypeId, navConfig.Name, navConfig.Description, navConfig.Data, navConfig.IsActive));
          }
        } 

        if (entityNavItem != null) { 
          var remoteClassEntityId = entityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType)?.Value.AsInt32();           
          if (remoteClassEntityId.HasValue && remoteClassEntityId.Value >0) {
            var cmd3 = new AddRemoveEntityInboundNavCommand(remoteClassEntityId.Value, propertyItem.Id, entityItem.Id, true);
            await _mediator.Send(cmd3, cancellationToken);
          }
        }

        // remove config for prop.
        var propConfig = await GetPropConfig(entityConfigItem, propertyItem, cancellationToken);
        if (propConfig != null) {
          await _mediator.Send(new DeleteItemCommand(propConfig.Id));
        }

      } else {   // is not a nav but a prop.

        if (entityNavItem != null) {
          var remoteEntityId = entityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType)?.Value.AsInt32();
          if (remoteEntityId.HasValue) {
            var cmd3 = new AddRemoveEntityInboundNavCommand(remoteEntityId.Value, propertyItem.Id, entityItem.Id, false);
            await _mediator.Send(cmd3, cancellationToken);
          }
        }

        var navConfig = await GetNavConfig(entityConfigItem, propertyItem, cancellationToken);
        if (entityNavItem != null&&navConfig != null) {          
          await _mediator.Send(new DeleteItemCommand(navConfig.Id));
        }

        // remove config for nav
        if (entityNavItem != null) { // remove the nav item.
          await _mediator.Send(new DeleteItemCommand(entityNavItem.Id));
        }

        // add config for prop
        var propConfig = await GetPropConfig(entityConfigItem, propertyItem, cancellationToken);
        if (propConfig == null) {
          propConfig = await _mediator.Send(new CreateRelatedItemCommand(entityConfigItem.Id,
            (int)WeRelationTypes.Contains, (int)WeItemType.EntityPropertyConfigurationModel, propertyItem.Name, "", "{}"));
          if (propConfig != null) {                        
            await SyncNullableAndType(propConfig, propIsNullable, propertyItem?.Id, cancellationToken);
          }
        } else { 
          if( propConfig.Name != propertyItem.Name ) {
            propConfig.Name = propertyItem.Name;
            await _mediator.Send(new UpdateItemCommand(propConfig.Id, propConfig.ItemTypeId, propConfig.Name, 
              propConfig.Description, propConfig.Data, propConfig.IsActive));
          }
        }      
      

      }

      return true;
    }

    private async Task SyncNullableAndType(ItemDto targetItem, bool isNullable, int? propTypeRefId, CancellationToken ct) {
      var dirty = false;

      var nullableProp = targetItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
      if (nullableProp != null && nullableProp.Value.AsBoolean() != isNullable) {
        var entity = await _context.ItemProperties.FindAsync(nullableProp.Id, ct);
        if (entity != null) { entity.Value = isNullable.ToString(); dirty = true; }
      }

      if (propTypeRefId.HasValue) {
        var typeProp = targetItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType);
        if (typeProp != null) {
          var entity = await _context.ItemProperties.FindAsync(typeProp.Id, ct);
          if (entity != null) { entity.Value = propTypeRefId.Value.ToString(); dirty = true; }
        }
      }

      if (dirty) await _context.SaveChangesAsync(ct);
    }

    private async Task<ItemDto?> GetNavConfig(ItemDto entityConfig, ItemDto entityProp, CancellationToken cancellationToken) {
      var targetId = entityProp.Id.ToString();
      foreach (var importRel in entityConfig.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationConfigurationModel )) {
        if (importRel.RelatedItemId != null) {
          var importItem = await GetCachedItemById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            var importProp = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType && p.Value == targetId);
            if (importProp != null) {
              return importItem;
            }
          }
        }
      }
      return null;    
    }

    private async Task<ItemDto?> GetPropConfig(ItemDto entityConfig, ItemDto entityProp, CancellationToken cancellationToken) {
        var targetId = entityProp.Id.ToString();
      foreach (var importRel in entityConfig.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityPropertyConfigurationModel)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await GetCachedItemById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            var importProp = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType && p.Value == targetId);
            if (importProp != null) {
              return importItem;
            }
          }
        }
      }
      return null;
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


    //below is namespace and class end. no need to change it.
  }
}
