using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
      // todo list
      // 1. Validate the input data (EntityItem and PropertyItem)
      // 2. load both items from the database using their IDs  -- nevermind, items in request have it.
      // 3. Check property states of the property to see if it's a nav bool is true.
      // 4. If the property is a nav, add or verify the EntityNavigationModel exists for the property.
      // 5. If the property is not a nav, verify remove nav
      // 6. If the property is a nav, update the navigation Configuration related items to match. (that is check for propertyConfigModel and remove. confirm or add navigationConfigModel )
      // 7. If the property is not a nav, add remove verify the property config models match the property state.
      // 8. All good, return true.

      if (request.EntityItem == null || request.PropertyItem == null) {        
        return false;

      }
      var propertyItem = await _context.GetItemDtoById(  request.PropertyItem.Id, cancellationToken);  // maybe fresh copies.
      var entityItem = await _context.GetItemDtoById(  request.EntityItem.Id, cancellationToken);

      bool propIsNullable = propertyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? false;

      // should only be one config.
      var configItemId = entityItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityConfigurationModel)?.RelatedItemId ?? 0;
      ItemDto? entityConfigItem = null;
      if (configItemId > 0) { 
        entityConfigItem = await GetCachedItemById(configItemId, cancellationToken);
      }
      if (entityConfigItem == null) { return false; }

      
      var isNavProp = propertyItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation)?.Value.AsBoolean() ?? false;
      var entityNavItem = await GetEntityNavForProperty(entityItem, propertyItem, cancellationToken);
      if (isNavProp) { // is a nav
        
        if (entityNavItem == null) {  // add the navigtion if not there.
          entityNavItem = await _mediator.Send(new CreateRelatedItemCommand(propertyItem.Id, 
            (int)WeRelationTypes.Contains, (int)WeItemType.EntityNavigationModel, propertyItem.Name, "", "{}"));
          if (entityNavItem != null) { 

            var isNullableProp = entityNavItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
            if (isNullableProp != null) { 
              if (isNullableProp.Value.AsBoolean() != propIsNullable) {                
                var propEntity = await _context.ItemProperties.FindAsync(isNullableProp.Id, cancellationToken);
                if (propEntity != null) {
                  propEntity.Value = propIsNullable.ToString();
                  _context.ItemProperties.Update(propEntity);
                  await _context.SaveChangesAsync(cancellationToken);
                }
              }
            }          

          }
        }

        // add config for nav
        var navConfig = await GetNavConfig(entityConfigItem, propertyItem, cancellationToken);
        if (navConfig == null) {
          navConfig = await _mediator.Send(new CreateRelatedItemCommand(entityConfigItem.Id,
            (int)WeRelationTypes.Contains, (int)WeItemType.EntityNavigationConfigurationModel, propertyItem.Name, "", "{}"));
          if (navConfig != null) {
            await SyncNullableAndType(navConfig, propIsNullable, entityNavItem?.Id, cancellationToken);      
          }
        }

        // remove config for prop.
        var propConfig = await GetPropConfig(entityConfigItem, propertyItem, cancellationToken);
        if (propConfig != null) {
          await _mediator.Send(new DeleteItemCommand(propConfig.Id));
        }

      } else {   // is not a nav but a prop.

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
        }

        // remove config for nav
        var navConfig = await GetNavConfig(entityConfigItem, propertyItem, cancellationToken);
        if (navConfig != null) {
          await _mediator.Send(new DeleteItemCommand(navConfig.Id));
        }

      }

      return true;
    }

    private async Task SyncNullableAndType(ItemDto targetItem, bool isNullable, int? classTypeRefId, CancellationToken ct) {
      var dirty = false;

      var nullableProp = targetItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
      if (nullableProp != null && nullableProp.Value.AsBoolean() != isNullable) {
        var entity = await _context.ItemProperties.FindAsync(nullableProp.Id, ct);
        if (entity != null) { entity.Value = isNullable.ToString(); dirty = true; }
      }

      if (classTypeRefId.HasValue) {
        var typeProp = targetItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType);
        if (typeProp != null) {
          var entity = await _context.ItemProperties.FindAsync(typeProp.Id, ct);
          if (entity != null) { entity.Value = classTypeRefId.Value.ToString(); dirty = true; }
        }
      }

      if (dirty) await _context.SaveChangesAsync(ct);
    }

    private async Task<ItemDto?> GetNavConfig(ItemDto entityConfig, ItemDto entityProp, CancellationToken cancellationToken) {
      foreach (var importRel in entityConfig.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationConfigurationModel && r.RelatedItemName == entityProp.Name)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await GetCachedItemById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            return importItem;
          }
        }
      }
      return null;    
    }

    private async Task<ItemDto?> GetPropConfig(ItemDto entityConfig, ItemDto entityProp, CancellationToken cancellationToken) {
      foreach (var importRel in entityConfig.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityPropertyConfigurationModel  && r.RelatedItemName == entityProp.Name)) {
        if (importRel.RelatedItemId != null) {
          var importItem = await GetCachedItemById(importRel.RelatedItemId.Value, cancellationToken);
          if (importItem != null) {
            return importItem;
          }
        }
      }
      return null;
    } 

    private async Task<ItemDto?> GetEntityNavForProperty(ItemDto entityItem, ItemDto propItem, CancellationToken cancellationToken) {
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
