using Microsoft.EntityFrameworkCore;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemSummaryExt {


    public static async Task<ItemSummaryDto?> GetSummaryDtoById(this FabricDbContext context, 
      int id, bool nodesUp, bool includeProps = true, CancellationToken cancellationToken = default) {

      var result = await context.Items
        .AsNoTracking()
        .Where(i => i.Id == id)
        .Select(i => new ItemSummaryDto {
          Id = i.Id,
          ParentId = i.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(parentId => parentId != id),
          TypeId = i.ItemTypeId,
          TypeName = i.ItemType.Name,
          Name = i.Name,
          Content = i.ItemTypeId.IsContentType() ? i.Description : null,
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

      if (result != null && result.TypeId.IsMethodCodeType()) {
        var codeProps = await context.BuildMethod(result.Id, cancellationToken);
        if (codeProps != null) {
          var code = codeProps.MethodSignature + "{" + Environment.NewLine + Cx.MethodStartMarker + Environment.NewLine
            + codeProps.MethodBody + Environment.NewLine + Cx.MethodEndMarker;
          result.Content = code;
        }
      }

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
          Content = r.RelatedItem != null && r.RelatedItem.ItemTypeId.IsContentType() ? r.RelatedItem.Description  : null,
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

      string? code = null;
      if (itemSummary.TypeId.IsMethodCodeType()) {               
        var codeProps = await context.BuildMethod(itemSummary.Id, cancellationToken);
        if (codeProps != null) {
          code = codeProps.MethodSignature + $"{{" + Environment.NewLine + Cx.MethodStartMarker + Environment.NewLine
            + codeProps.MethodBody + Environment.NewLine + Cx.MethodEndMarker;
          itemSummary.Content = code;
        }        
      }

      foreach (var child in itemSummary.Nodes) {
        await context.LoadSummaryRecursively(child, includeProps, cancellationToken);
      }

      return itemSummary;
    }


    public static async Task<ItemSummaryDto> ToSummary(this FabricDbContext context, ItemDto item, CancellationToken cancellationToken = default) {
      if (item == null) throw new ArgumentNullException(nameof(item));
      string? code = null;
      if (item.ItemTypeId.IsMethodCodeType()) {
        var codeProps = await context.BuildMethod(item.Id, cancellationToken);
        if (codeProps != null) {
          code = codeProps.MethodSignature + $"{{" + Environment.NewLine+Cx.MethodStartMarker + Environment.NewLine
            + codeProps.MethodBody + Environment.NewLine + Cx.MethodEndMarker;
        }
      }

      return new ItemSummaryDto {
        Id = item.Id,
        Name = item.Name,
        TypeId = item.ItemTypeId,
        TypeName = item.ItemType != null ? item.ItemType.Name : "",
        ParentId = item.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(parentId => parentId != item.Id),        
        Content = item.ItemTypeId.IsContentType() ? item.Description : item.ItemTypeId.IsMethodCodeType() ? code : null,
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
