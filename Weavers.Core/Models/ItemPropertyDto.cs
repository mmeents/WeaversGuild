using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Models {
  public class ItemPropertyDto {
    public int Id { get; set; }
    public int? ItemPropertyDefaultId { get; set; }
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public int? ValueDataTypeId { get; set; }
    public int? ReferenceItemTypeId { get; set; }
    public int? EditorTypeId { get; set; }
    public bool IsRequired { get; set; } = false;
    public bool IsReadOnly { get; set; }
    public bool IsVisible { get; set; } = true;

    // Navigation properties
    public ItemDto? Item { get; set; } = null!;
    public DataTypeDto? ValueType { get; set; } = null!;
    public EditorTypeDto? Editor { get; set; }
    public ItemTypeDto? ReferenceItemType { get; set; }
  }


  public static class ItemPropertyDtoExtensions {
    public static ItemPropertyDto ToDto(this ItemProperty itemProperty) {
      return new ItemPropertyDto {
        Id = itemProperty.Id,
        ItemPropertyDefaultId = itemProperty.ItemPropertyDefaultId,
        ItemId = itemProperty.ItemId,
        Name = itemProperty.Name,
        Value = itemProperty.Value,
        ValueDataTypeId = itemProperty.ValueDataTypeId,
        ReferenceItemTypeId = itemProperty.ReferenceItemTypeId,
        EditorTypeId = itemProperty.EditorTypeId,
        IsRequired = itemProperty.IsRequired,
        IsReadOnly = itemProperty.IsReadOnly,
        IsVisible = itemProperty.IsVisible,
        Item = itemProperty.Item.ToDto(),
        ValueType = itemProperty.ValueType?.ToDto() ?? null!,
        Editor = itemProperty.Editor?.ToDto(),
        ReferenceItemType = itemProperty.ReferenceItemType?.ToDto()
      };
    }
  }


}
