using MediatR;
using System.Security.Cryptography;
using System.Text;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemPropsExt {


    public static void AddOrUpdateProperty(this ItemDto item, ItemPropertyDto property) {
      var existingProperty = item.Properties.FirstOrDefault(p => p.Name == property.Name);
      if (existingProperty != null) {
        existingProperty.Value = property.Value;
        existingProperty.ValueHash = property.Value.ComputeHash();
      } else {
        property.ValueHash = property.Value.ComputeHash();
        item.Properties.Add(property);
      }
    }

    public static async Task SaveProp(this ItemPropertyDto property, ItemDto item, IMediator mediator) {
      var updatedProp = await mediator.Send(new AddUpdateItemPropertyCommand(
        property.Id,
        property.ItemId,
        property.Name,
        property.Value,        
        property.ValueDataTypeId,
        property.EditorTypeId,
        property.ReferenceItemTypeId
      ));
      if (updatedProp != null) {        
        if (item != null) {
          item.AddOrUpdateProperty(updatedProp);
        }
      }
    }

    public static async Task<ItemDto> UpdateItemProp(this IMediator mediator, ItemDto importItem, ItemPropertyDto itemProp) {
      if (itemProp == null) {
        throw new ArgumentNullException(nameof(itemProp));
      }
      var command = new AddUpdateItemPropertyCommand(
        itemProp.Id,
        itemProp.ItemId,
        itemProp.Name,
        itemProp.Value,
        itemProp.ValueDataTypeId,
        itemProp.EditorTypeId,
        itemProp.ReferenceItemTypeId
      );
      var result = await mediator.Send(command);
      importItem.AddOrUpdateProperty(itemProp);
      return importItem;
    }

    public static string AsIntString(this WeItemType item) {
      int itemValue = (int)item;
      return itemValue.ToString();
    } 
    public static async Task SetProperty(this IMediator mediator, ItemDto item, string propertyName, string propertyValue) {
      var prop = item.Properties.FirstOrDefault(p => p.Name == propertyName);
      if (prop != null) {
        prop.Value = propertyValue;
        prop.ValueHash = propertyValue.ComputeHash();
        await prop.SaveProp(item, mediator).ConfigureAwait(false);
      }
    }

    public static long? ComputeHash(this string? value) {
      if (value is null) return null;
      var bytes = SHA256.HashData(Encoding.Unicode.GetBytes(value));
      return BitConverter.ToInt64(bytes, 0);
    }

  }
}
