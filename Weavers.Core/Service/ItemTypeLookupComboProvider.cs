using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.ItemTypes;
using Weavers.Core.Handlers.Templates;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;

namespace Weavers.Core.Service {
  public class ItemTypeLookupComboProvider : IItemTypeLookupComboProvider {
    private readonly IServiceScopeFactory _scopeFactory;
    public ItemTypeLookupComboProvider(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
    }

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
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var iType = Task.Run( async () => await mediator.Send(new GetItemByIdQuery(itemTypeId)).ConfigureAwait(false)).GetAwaiter().GetResult();
             if (iType != null) { 
              return iType.Description;
             }
          } catch (Exception) { }  // didnt' work, probably a reference to an item.          
        
        }
      }

      return $"unknown key {value}";
    }

    private List<ItemLookup>? _typesCache;

    public async Task<IEnumerable<ItemLookup>> GetTypesAsync() {
      if (_typesCache != null) return _typesCache;
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      return _typesCache =
        await mediator.Send(new GetItemsByItemTypeQuery((int)WeItemType.ActiveItemTypes));
    }

    public async Task<IEnumerable<ItemLookup>> GetValuesAsync(int? itemTypeId) {
      if (itemTypeId is null or 0) return Enumerable.Empty<ItemLookup>();
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      return await mediator.Send(new GetItemsByItemTypeQuery(itemTypeId.Value));
    }

    public async Task<ItemDto?> GetItemByIdAsync(int id) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var result = await mediator.Send(new GetItemByIdQuery(id));
      return result;
    }

    public bool IsValidValue(object? value) {
      throw new NotImplementedException();
    }

    public async Task<string> RenderTemplate(ItemPropertyDto Field, CancellationToken cancellationToken) {
      if (Field == null ||  Field.Value == null) return "";      
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new RenderFieldTemplate(Field), cancellationToken);
        return result;        
      } catch (Exception) { 
        return $"error rendering template for key {Field.Value}";
      }        
    } 
  }

  public interface IItemTypeLookupComboProvider : IComboBoxDataProvider {

  }
}
