using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Exceptions;
using Weavers.Core.Models;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Weavers.Core.Extensions {
  public static class OrganizationItemExt {
    public static async Task<LibraryItemDto?> GetOrganizationItemDto(this FabricDbContext fabricDbContext, CancellationToken cancellationToken) {

      var organization = await fabricDbContext.Items
        .AsNoTracking()
        .Where(i => i.ItemTypeId == (int)WeItemType.OrganizationModel)
        .Select(i => new LibraryItemDto {
          Id = i.Id,
          ItemTypeId = i.ItemTypeId,
          ItemTypeName = i.ItemType.Name,
          Name = i.Name,
          Description = i.Description,
          Data = i.Data,
          Established = i.Established,
          WrittenAt = i.WrittenAt,
          IsActive = i.IsActive,

          Relations = i.Relations.Select(r => new RelationDto {
            Id = r.Id,
            ItemId = r.ItemId,
            ItemName = r.Item.Name,
            RelatedItemId = r.RelatedItemId,
            RelatedItemTypeId = r.RelatedItem != null ? r.RelatedItem.ItemTypeId : (int?)null,
            RelatedItemName = r.RelatedItem != null ? r.RelatedItem.Name : "",
            RelationTypeId = r.RelationTypeId,
            RelationTypeName = r.RelationType.Name ?? string.Empty,
            Rank = r.Rank,
            Established = r.Established,
            RelatedItemHasChildren = r.RelatedItem != null && r.RelatedItem.Relations.Any(cr => cr.RelationTypeId == (int)WeRelationTypes.Contains)
          }).ToList(),

          Properties = i.Properties.Select(p => new ItemPropertyDto {
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
          }).ToList(),

          Builds = i.Builds.Select(b => new BuildDto {
            Id = b.Id,
            StartedAt = b.StartedAt,
            CompletedAt = b.CompletedAt,
            Status = b.Status,
            BuildOutput = b.BuildOutput,
            CompilerOutput = b.CompilerOutput,
            LibraryItemId = b.LibraryItemId,
            BuildFiles = b.BuildFiles.Select(bf => new BuildFileDto {
              Id = bf.Id,
              BuildId = bf.BuildId,
              ItemId = bf.ItemId,
              FilePath = bf.FilePath,
              WasDeleted = bf.WasDeleted,
              WasWritten = bf.WasWritten
            }).ToList()
          }).ToList()
        })
        .FirstOrDefaultAsync(cancellationToken);

      if (organization == null) return null;

      return organization;

    }
  }
}
