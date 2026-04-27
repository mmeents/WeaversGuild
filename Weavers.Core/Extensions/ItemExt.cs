using Microsoft.EntityFrameworkCore;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemExt {

    public static async Task<int> GetItemsNextRankId(this FabricDbContext context, int itemId, CancellationToken cancellationToken = default) {
      var maxRank = await context.Relations.Where(ir => ir.ItemId == itemId)
        .MaxAsync(ir => (int?)ir.Rank, cancellationToken);
      return (maxRank ?? 0) + 1;
    }

    public static async Task<RelationDto> GetRelationDtoById(this FabricDbContext context, int relationId, CancellationToken cancellationToken = default) {
      var result = await context.Relations
        .AsNoTracking()
        .Where(r => r.Id == relationId)
        .Select(r => new RelationDto {
          Id = r.Id,
          ItemId = r.ItemId,
          ItemName = r.Item.Name,
          RelatedItemId = r.RelatedItemId,
          RelatedItemTypeId = r.RelatedItem != null ?  r.RelatedItem.ItemTypeId : (int?)null,
          RelatedItemName = r.RelatedItem != null ? r.RelatedItem.Name : "",
          RelationTypeId = r.RelationTypeId,
          RelationTypeName = r.RelationType.Name ?? string.Empty,
          Rank = r.Rank,
          Established = r.Established,
          RelatedItemHasChildren = r.RelatedItem != null && r.RelatedItem.Relations.Any(cr => cr.RelationTypeId == (int)WeRelationTypes.Contains)
        })
        .FirstOrDefaultAsync(cancellationToken);
      return result ?? throw new Exception($"Relation with Id {relationId} not found");
    }

    public static async Task<ItemDto> GetItemDtoById(this FabricDbContext context, int Id, CancellationToken cancellationToken = default) {
      var result = await context.Items
        .AsNoTracking()
        .Where(i => i.Id == Id)
        .Select(i => new ItemDto {
          Id = i.Id,
          ItemTypeId = i.ItemTypeId,
          ItemTypeName = i.ItemType.Name,
          Name = i.Name,
          Description = i.Description,
          Data = i.Data,
          Established = i.Established,
          IsActive = i.IsActive,
          Relations = i.Relations.Select(r => new RelationDto {
            Id = r.Id,
            ItemId = r.ItemId,
            ItemName = r.Item.Name,
            RelatedItemId = r.RelatedItemId,
            RelatedItemTypeId = r.RelatedItem != null ?  r.RelatedItem.ItemTypeId : (int?)null,
            RelatedItemName = r.RelatedItem != null ? r.RelatedItem.Name : "",
            RelationTypeId = r.RelationTypeId,
            RelationTypeName = r.RelationType.Name ?? string.Empty,
            Rank = r.Rank,
            Established = r.Established,
            RelatedItemHasChildren = r.RelatedItem != null && r.RelatedItem.Relations.Any(cr => cr.RelationTypeId == (int)WeRelationTypes.Contains)
          }).ToList(),
          IncomingRelations = i.IncomingRelations.Select(r => new RelationDto {
            Id = r.Id,
            ItemId = r.ItemId,
            ItemName = r.Item.Name ?? string.Empty,
            RelatedItemId = r.RelatedItemId,
            RelatedItemTypeId = r.RelatedItem != null ?  r.RelatedItem.ItemTypeId : (int?)null,
            RelatedItemName = (r.RelatedItem == null) ? "" : r.RelatedItem.Name ,
            RelationTypeId = r.RelationTypeId,
            RelationTypeName = r.RelationType.Name ?? string.Empty,
            Rank = r.Rank,
            Established = r.Established,
            RelatedItemHasChildren = r.RelatedItem !=null && r.RelatedItem.Relations != null && r.RelatedItem.Relations.Any()          
          }).ToList(),
          Properties = i.Properties.Select(p => new ItemPropertyDto {
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
                Name = WeDataType.None.ToString()}                 
              :  new DataTypeDto {
                Id = p.ValueType.Id,
                Name = p.ValueType.Name
              },
            Editor = (p.Editor == null) 
              ? new EditorTypeDto{ Id = (int)WeEditorType.None, Name = WeEditorType.None.ToString() } 
              : new EditorTypeDto{ Id = p.Editor.Id, Name = p.Editor.Name, Description = p.Editor.Description,
                IsVisible = p.Editor.IsVisible, IsReadOnly = p.Editor.IsReadOnly, Rank=p.Editor.Rank},
            ReferenceItemType = (p.ReferenceItemType == null) 
              ? null : new ItemTypeDto { Id = p.ReferenceItemType.Id, Name = p.ReferenceItemType.Name }

          }).ToList()
        })
        .FirstOrDefaultAsync(cancellationToken);

      if (result != null) {       
        foreach (var prop in result.Properties) {
          prop.Item = result;          // ← now each property points to the outer ItemDto
        }
      }

      return result ?? throw new Exception($"Item with Id {Id} not found");
    }

    public static async Task<bool> SyncDefaultsByModelIdAsync(this FabricDbContext context, int itemlId, int itemTypeId, CancellationToken cancellationToken = default) {

      var properties = await context.ItemProperties.Where(p => p.ItemId == itemlId).ToListAsync(cancellationToken);
      var expected = await context.ItemPropertyDefaults.Where(d => d.ItemTypeId == itemTypeId).ToListAsync(cancellationToken);

      bool updated = false;
      foreach (var property in expected) {
        var existing = properties.FirstOrDefault(p => p.Name == property.Key);

        if (existing == null) {
          existing = new ItemProperty {
            ItemPropertyDefaultId = property.Id,
            ItemId = itemlId,
            Name = property.Key,
            Value = property.DefaultValue,
            ValueDataTypeId = property.ValueDataTypeId,
            ReferenceItemTypeId = property.ReferenceItemTypeId,
            EditorTypeId = property.EditorTypeId,
            IsRequired = property.IsRequired,
            IsVisible = property.IsVisible,
            IsReadOnly = property.IsReadOnly
          };
          context.ItemProperties.Add(existing);
          updated = true;
        }
      }
      if (updated) {
        await context.SaveChangesAsync(cancellationToken);
      }
      return updated;
    }

    public static ItemDto Clone(this ItemDto item) {
      return new ItemDto {
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
              Name = WeDataType.None.ToString()} 
            : new DataTypeDto {
              Id = p.ValueType.Id,
              Name = p.ValueType.Name
            },
          Editor = (p.Editor == null) 
            ? new EditorTypeDto{ Id = (int)WeEditorType.None, Name = WeEditorType.None.ToString() } 
            : new EditorTypeDto{ Id = p.Editor.Id, Name = p.Editor.Name, Description = p.Editor.Description,
              IsVisible = p.Editor.IsVisible, IsReadOnly = p.Editor.IsReadOnly, Rank=p.Editor.Rank},
          ReferenceItemType = (p.ReferenceItemType == null) 
            ? null : new ItemTypeDto { Id = p.ReferenceItemType.Id, Name = p.ReferenceItemType.Name }
        }).ToList()
      };
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

    public static bool IsValidFolderParent(this ItemDto item) =>
      item.ItemTypeId == (int)WeItemType.ProjectFolderModel ||
      item.ItemTypeId == (int)WeItemType.RelativeFolderModel;

    public static string ResolveParentFolderPath(this ItemDto? item, string defaultPath) {
      if (item == null) return defaultPath;
      string propertyKey = item.ItemTypeId.GetFolderPropertyName();                  
      string Value = item.Properties.FirstOrDefault(p => p.Name == propertyKey)?.Value ?? defaultPath;
      if (item.ItemTypeId == (int)WeItemType.LibraryModel 
        || item.ItemTypeId == (int)WeItemType.DependencyInjectionModel) {
        Value = Path.GetDirectoryName(Value) ?? defaultPath; 
      }
      return Value;
    }

    public static string ResolveParentNamespace(this ItemDto? item, string defaultNamespace) {
      if (item == null) return defaultNamespace;
      var propKey = item.ItemTypeId.GetNamespacePropertyName();
      return item.Properties.FirstOrDefault(p => p.Name == propKey)?.Value ?? defaultNamespace;
    }
  }
}
