using MediatR;
using Weavers.Core.Models;
using Weavers.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Weavers.Core.Handlers.Items {

  public record GetRootProjectsQuery() : IRequest<List<ItemDto>>;

  public class GetRootProjectsQueryHandler : IRequestHandler<GetRootProjectsQuery, List<ItemDto>> {
    private readonly FabricDbContext _context;

    public GetRootProjectsQueryHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<List<ItemDto>> Handle(GetRootProjectsQuery request, CancellationToken cancellationToken) {

      var result = await _context.Items.AsNoTracking()
        .Where(i => i.ItemTypeId == (int)WeItemType.ProjectFolderModel && i.IsActive
          && !i.IncomingRelations.Any(r => r.Item.ItemTypeId == (int)WeItemType.ProjectFolderModel))
        .OrderBy(i => i.Name)
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
            ItemName = r.Item != null ? r.Item.Name : "",
            RelatedItemId = r.RelatedItemId,
            RelatedItemName = r.RelatedItem != null ? r.RelatedItem.Name : "",
            RelationTypeId = r.RelationTypeId,
            RelationTypeName = r.RelationType.Name,
            Rank = r.Rank,
            Established = r.Established,
            RelatedItemHasChildren = r.RelatedItem != null && r.RelatedItem.Relations.Any()
          }).ToList(),
          IncomingRelations = i.IncomingRelations.Select(r => new RelationDto {
            Id = r.Id,
            ItemId = r.ItemId,
            ItemName = r.Item != null ? r.Item.Name : "",
            RelatedItemId = r.RelatedItemId,
            RelationTypeId = r.RelationTypeId,
            RelationTypeName = r.RelationType.Name,
            Rank = r.Rank,
            Established = r.Established
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
                Name = WeDataType.None.ToString()
              }
              : new DataTypeDto {
                Id = p.ValueType.Id,
                Name = p.ValueType.Name
              },
            Editor = (p.Editor == null)
              ? new EditorTypeDto { Id = (int)WeEditorType.None, Name = WeEditorType.None.ToString() }
              : new EditorTypeDto {
                Id = p.Editor.Id, Name = p.Editor.Name, Description = p.Editor.Description,
                IsVisible = p.Editor.IsVisible, IsReadOnly = p.Editor.IsReadOnly, Rank = p.Editor.Rank
              },
            ReferenceItemType = (p.ReferenceItemType == null)
              ? null : new ItemTypeDto { Id = p.ReferenceItemType.Id, Name = p.ReferenceItemType.Name }

          }).ToList()
        })
        .ToListAsync();
      return result;    
    }
  }
}