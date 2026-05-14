using Microsoft.EntityFrameworkCore;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemSummaryExt {


    public static async Task<ItemSummaryDto?> GetSummaryDtoById(this FabricDbContext context, 
      int id, bool nodesUp, bool includeProps = true, CancellationToken cancellationToken = default) 
    {

      var result = await context.Items
        .AsNoTracking()
        .Where(i => i.Id == id)
        .Select(i => new ItemSummaryDto {
          Id = i.Id,
          ParentId = i.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(parentId => parentId != id),
          TypeId = i.ItemTypeId,
          TypeName = i.ItemType.Name,
          Name = i.Name,
          NodesUp = nodesUp,
          Nodes = !nodesUp ? null : i.Relations.Select(r => new ItemSummaryDto {
            Id = r.RelatedItemId ?? 0,
            ParentId = r.ItemId,
            Name = r.RelatedItem != null ? r.RelatedItem.Name : "",
            TypeId = r.RelatedItem != null ? r.RelatedItem.ItemTypeId : 0,
            TypeName = r.RelatedItem != null ? r.RelatedItem.ItemType.Name : "",
          }).ToList(),
          Props = !includeProps ? null : i.Properties.Select(p => new PropSummaryDto {
            Id = p.Id,
            Name = p.Name,
            Value = p.Value ?? "",            
            DataType = p.ValueType == null 
              ? null : ((WeDataType)p.ValueType.Id).ToString(),
            EditorType = p.Editor == null ? null : p.Editor.Name,
            ReferenceType = p.ReferenceItemType == null 
              ? null : p.ReferenceItemType.Name

          }).ToList()

        })
        .FirstOrDefaultAsync(cancellationToken);

      if (nodesUp && result != null) {
        result = await context.LoadSummaryRecursively(result, includeProps, cancellationToken);
      }

      return result;
    }


    public static async Task<ItemSummaryDto> LoadSummaryRecursively(this FabricDbContext context, 
      ItemSummaryDto itemSummary, bool includeProps = true,
      CancellationToken cancellationToken = default)
    {
      if (itemSummary == null) throw new ArgumentNullException(nameof(itemSummary));

      var children = await context.Items
        .AsNoTracking()
        .Where(i => i.Id == itemSummary.Id)
        .SelectMany(i => i.Relations)
        .Select(r => new ItemSummaryDto
        {
          Id = r.RelatedItemId ?? 0,
          ParentId = r.ItemId,
          Name = r.RelatedItem != null ? r.RelatedItem.Name : "",
          TypeId = r.RelatedItem != null ? r.RelatedItem.ItemTypeId : 0,
          TypeName = r.RelatedItem != null ? r.RelatedItem.ItemType.Name : "",
          Props = !includeProps ? null : r.RelatedItem != null
            ? r.RelatedItem.Properties.Select(p => new PropSummaryDto {
            Id = p.Id,
            Name = p.Name,
            Value = p.Value ?? "",
            DataType = p.ValueType == null ? null : ((WeDataType)p.ValueType.Id).ToString(),
            EditorType = p.Editor == null ? null : p.Editor.Name,
            ReferenceType = p.ReferenceItemType == null ? null : p.ReferenceItemType.Name
          }).ToList()
        : new List<PropSummaryDto>()
        })
        .ToListAsync(cancellationToken);

      itemSummary.Nodes = children;
      itemSummary.NodesUp = true;

      foreach(var child in itemSummary.Nodes) {
        await context.LoadSummaryRecursively(child, includeProps, cancellationToken);
      }

      return itemSummary;
    }


    public static ItemSummaryDto ToSummary(this ItemDto item) {
      return new ItemSummaryDto {
        Id = item.Id,
        Name = item.Name,
        TypeId = item.ItemTypeId,
        TypeName = item.ItemType != null ? item.ItemType.Name : "",
        ParentId = item.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(parentId => parentId != item.Id),
        Props = item.Properties.Select(p => new PropSummaryDto {
          Id = p.Id,
          Name = p.Name,
          Value = p.Value ?? "",
          DataType = p.ValueType == null ? null : ((WeDataType)p.ValueType.Id).ToString(),
          EditorType = p.Editor == null ? null : p.Editor.Name,
          ReferenceType = p.ReferenceItemType == null ? null : p.ReferenceItemType.Name
        }).ToList()
      };
    }

  }
}
