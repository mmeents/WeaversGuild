using MediatR;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.ItemTypes;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Service {
  public class ItemTypeLookupComboProvider(IMediator mediator) : IItemTypeLookupComboProvider {
    private IMediator _mediator = mediator;
    public string GetDisplayText(object? value) {
      if (value == null) { return ""; }
      if (value is string strValue) {
        if (int.TryParse(strValue, out int itemTypeId)) {
          try { 
            var iType = ((WeItemType)itemTypeId).Description();
            if (iType != null) { 
              return iType;
            }
          } catch (Exception) { }  // didnt' work, probably a reference to an item. 

          try {
             var iType = Task.Run( async () => await _mediator.Send(new GetItemByIdQuery(itemTypeId)).ConfigureAwait(false)).GetAwaiter().GetResult();
             if (iType != null) { 
              return iType.Description;
             }
          } catch (Exception) { }  // didnt' work, probably a reference to an item.          
        
        }
      }

      return $"unknown key {value}";
    }

    public async Task<IEnumerable<ItemLookup>> GetItemsAsync(ItemPropertyDto? field = null) {
      if (field == null) {
        throw new ArgumentNullException(nameof(field), "Field parameter cannot be null");
      }
      var itemTypeId = field.ReferenceItemTypeId;
      if (itemTypeId == null) {
        return Enumerable.Empty<ItemLookup>();
      } else { 
        return await _mediator.Send(new GetItemsByItemTypeQuery(itemTypeId.Value));
      }     
    }

    public async Task<ItemDto?> GetItemByIdAsync(int id) {
      var result = await _mediator.Send(new GetItemByIdQuery(id));
      return result;
    }

    public bool IsValidValue(object? value) {
      throw new NotImplementedException();
    }
  }

  public interface IItemTypeLookupComboProvider : IComboBoxDataProvider;
}
