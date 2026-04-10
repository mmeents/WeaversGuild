using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Models {
  public class ItemDto {
    public int Id { get; set; }
    public int ItemTypeId { get; set; }
    public string ItemTypeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Data { get; set; } = "{}";
    public DateTime Established { get; set; }
    public bool IsActive { get; set; }

    public ItemTypeDto ItemType { get; set; } = null!;
    public ICollection<RelationDto> Relations { get; set; } = [];
    public ICollection<RelationDto> IncomingRelations { get; set; } = [];
    public ICollection<ItemPropertyDto> Properties { get; set; } = [];
  }


  public static class ItemDtoExtensions {
    public static ItemDto ToDto(this Item item, bool includeRelations = false) {
      var dto = new ItemDto {
        Id = item.Id,
        ItemTypeId = item.ItemTypeId,
        ItemType = item.ItemType?.ToDto() ?? null!,
        ItemTypeName = item.ItemType?.Name ?? string.Empty,
        Name = item.Name,
        Description = item.Description,
        Data = item.Data,
        Established = item.Established,
        IsActive = item.IsActive
      };

      if (includeRelations) {
        dto.Relations = item.Relations?.Select(r => r.ToDto()).ToList() ?? [];
        dto.IncomingRelations = item.IncomingRelations?.Select(r => r.ToDto()).ToList() ?? [];
      }
      dto.Properties = item.Properties?.Select(p => p.ToDto()).ToList() ?? [];
      return dto;
    }
  }
}
