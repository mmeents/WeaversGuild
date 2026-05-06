using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemCloneExt {

    // for the form to make a copy while editing so the cancel button can revert.
    public static ItemDto? Clone(this ItemDto item) {
      var result = new ItemDto {
        Id = item.Id,
        ItemTypeId = item.ItemTypeId,
        ItemTypeName = item.ItemTypeName,
        Name = item.Name,
        Description = item.Description,
        Data = item.Data,
        Established = item.Established,
        IsActive = item.IsActive,
        Relations = item.Relations.Select(r => new RelationDto {
          Id = r.Id,
          ItemId = r.ItemId,
          ItemName = r.ItemName ?? string.Empty,
          RelatedItemId = r.RelatedItemId,
          RelatedItemName = r.RelatedItemName ?? string.Empty,
          RelatedItemTypeId = r.RelatedItemTypeId,
          RelationTypeId = r.RelationTypeId,
          RelationTypeName = r.RelationTypeName ?? string.Empty,
          Rank = r.Rank,
          Established = r.Established,
          RelatedItemHasChildren = r.RelatedItemHasChildren
        }).ToList(),
        IncomingRelations = item.IncomingRelations.Select(r => new RelationDto {
          Id = r.Id,
          ItemId = r.ItemId,
          ItemName = r.ItemName ?? string.Empty,
          RelatedItemId = r.RelatedItemId,
          RelatedItemName = r.RelatedItemName ?? string.Empty,
          RelatedItemTypeId = r.RelatedItemTypeId,
          RelationTypeId = r.RelationTypeId,
          RelationTypeName = r.RelationTypeName ?? string.Empty,
          Rank = r.Rank,
          Established = r.Established,
          RelatedItemHasChildren = r.RelatedItemHasChildren
        }).ToList(),
        Properties = item.Properties.Select(p => new ItemPropertyDto {
          Id = p.Id,
          ItemId = p.ItemId,
          Name = p.Name,
          Value = p.Value,
          ValueDataTypeId = p.ValueDataTypeId,
          ReferenceItemTypeId = p.ReferenceItemTypeId,
          EditorTypeId = p.EditorTypeId,
          IsRequired = p.IsRequired,
          IsReadOnly = p.IsReadOnly,
          IsVisible = p.IsVisible,
          ValueType = (p.ValueType == null)
            ? new DataTypeDto() {
              Id = (int)WeDataType.None,
              Name = WeDataType.None.ToString()
            }
            : new DataTypeDto {
              Id = p.ValueType.Id,
              Name = p.ValueType.Name
            },
          Editor = (p.Editor == null)
            ? new EditorTypeDto { Id = (int)WeEditorType.None, Name = WeEditorType.None.ToString() }
            : new EditorTypeDto {
              Id = p.Editor.Id,
              Name = p.Editor.Name,
              Description = p.Editor.Description,
              IsVisible = p.Editor.IsVisible,
              IsReadOnly = p.Editor.IsReadOnly,
              Rank = p.Editor.Rank
            },
          ReferenceItemType = (p.ReferenceItemType == null)
            ? null : new ItemTypeDto { Id = p.ReferenceItemType.Id, Name = p.ReferenceItemType.Name }
        }).ToList()
      };
      if (result != null) {
        foreach (var prop in result.Properties) {
          prop.Item = result;          // ← now each property points to the outer ItemDto
        }
      }
      return result;
    }

    public static RelationDto Clone(this RelationDto relation) {
      return new RelationDto {
        Id = relation.Id,
        ItemId = relation.ItemId,
        ItemName = relation.ItemName ?? string.Empty,
        RelatedItemId = relation.RelatedItemId,
        RelatedItemName = relation.RelatedItemName ?? string.Empty,
        RelatedItemTypeId = relation.RelatedItemTypeId,
        RelationTypeId = relation.RelationTypeId,
        RelationTypeName = relation.RelationTypeName ?? string.Empty,
        Rank = relation.Rank,
        Established = relation.Established,
        RelatedItemHasChildren = relation.RelatedItemHasChildren
      };
    }

  }
}
