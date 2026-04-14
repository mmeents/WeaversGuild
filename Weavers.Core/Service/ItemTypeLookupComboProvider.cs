using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.ItemTypes;

namespace Weavers.Core.Service {
  public class ItemTypeLookupComboProvider(IMediator mediator) : IItemTypeLookupComboProvider {
    private IMediator _mediator = mediator;
    public string GetDisplayText(object? value) {
      throw new NotImplementedException();
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
