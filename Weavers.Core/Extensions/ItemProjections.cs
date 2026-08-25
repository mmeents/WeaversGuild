using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemProjections {

    // Declaration order matters — these must come before ToItemDto,
    // static field initializers run top to bottom.
    public static readonly Expression<Func<Relation, RelationDto>> ToRelationDto =
      r => new RelationDto {
        Id = r.Id,
        ItemId = r.ItemId,
        ItemTypeId = r.Item.ItemTypeId,
        ItemName = r.Item.Name ?? string.Empty,
        RelatedItemId = r.RelatedItemId,
        RelatedItemTypeId = r.RelatedItem != null ? r.RelatedItem.ItemTypeId : (int?)null,
        RelatedItemName = r.RelatedItem != null ? r.RelatedItem.Name : string.Empty,
        RelationTypeId = r.RelationTypeId,
        RelationTypeName = r.RelationType.Name ?? string.Empty,
        Rank = r.Rank,
        Established = r.Established,
        RelatedItemHasChildren = r.RelatedItem != null
          && r.RelatedItem.Relations.Any(cr => cr.RelationTypeId == (int)WeRelationTypes.Contains)
      };

    public static readonly Expression<Func<ItemProperty, ItemPropertyDto>> ToPropertyDto =
      p => new ItemPropertyDto {
        Id = p.Id,
        ItemId = p.ItemId,
        Name = p.Name,
        Value = p.Value,
        ValueHash = p.ValueHash,
        ValueDataTypeId = p.ValueDataTypeId,
        ReferenceItemTypeId = p.ReferenceItemTypeId,
        EditorTypeId = p.EditorTypeId,
        IsRequired = p.IsRequired,
        IsReadOnly = p.IsReadOnly,
        IsVisible = p.IsVisible,
        ValueType = p.ValueType == null
          ? new DataTypeDto { Id = (int)WeDataType.None, Name = "None" }
          : new DataTypeDto { Id = p.ValueType.Id, Name = p.ValueType.Name },
        Editor = p.Editor == null
          ? new EditorTypeDto { Id = (int)WeEditorType.None, Name = "None" }
          : new EditorTypeDto {
            Id = p.Editor.Id,
            Name = p.Editor.Name,
            Description = p.Editor.Description,
            IsVisible = p.Editor.IsVisible,
            IsReadOnly = p.Editor.IsReadOnly,
            Rank = p.Editor.Rank
          },
        ReferenceItemType = p.ReferenceItemType == null
          ? null
          : new ItemTypeDto { Id = p.ReferenceItemType.Id, Name = p.ReferenceItemType.Name }
      };

    public static readonly Expression<Func<Item, ItemDto>> ToItemDto =
      i => new ItemDto {
        Id = i.Id,
        ItemTypeId = i.ItemTypeId,
        ItemTypeName = i.ItemType.Name,
        Name = i.Name,
        Description = i.Description,
        Data = i.Data,
        Established = i.Established,
        WrittenAt = i.WrittenAt,
        IsActive = i.IsActive,
        Relations = i.Relations.AsQueryable().Select(ToRelationDto).ToList(),
        IncomingRelations = i.IncomingRelations.AsQueryable().Select(ToRelationDto).ToList(),
        Properties = i.Properties.AsQueryable().Select(ToPropertyDto).ToList()
      };
    public static ItemDto LinkProperties(this ItemDto dto) {
      foreach (var p in dto.Properties) p.Item = dto;
      return dto;
    }

    public static List<ItemDto> LinkProperties(this List<ItemDto> dtos) {
      foreach (var d in dtos) d.LinkProperties();
      return dtos;
    }
  }
}
