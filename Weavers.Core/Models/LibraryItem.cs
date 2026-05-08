using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Models {
  public class LibraryItemDto : ItemDto {

    public LibraryItemDto() : base() { }

    public ICollection<BuildDto> Builds { get; set; } = new List<BuildDto>();

  }


  public static class LibraryDtoExtensions { 
    public static LibraryItemDto ToLibraryItemDto(this Item item, bool includeRelations = false) {
      var dto = new LibraryItemDto {
        Id = item.Id,
        ItemTypeId = item.ItemTypeId,
        ItemType = item.ItemType?.ToDto() ?? null!,
        ItemTypeName = item.ItemType?.Name ?? string.Empty,
        Name = item.Name,
        Description = item.Description,
        Data = item.Data,
        Established = item.Established,
        WrittenAt = item.WrittenAt,
        IsActive = item.IsActive
      };
      if (includeRelations) {
        dto.Relations = item.Relations?.Select(r => r.ToDto()).ToList() ?? [];
        dto.IncomingRelations = item.IncomingRelations?.Select(r => r.ToDto()).ToList() ?? [];
        dto.Properties = item.Properties?.Select(p => p.ToDto()).ToList() ?? [];
        dto.Builds = item.Builds?.Select(b => b.ToDto(true, false)).ToList() ?? [];
        foreach(var build in dto.Builds) {
          if (dto.Id == build.LibraryItemId) {
            build.LibraryItem = dto;
          }          
        }
      }
      return dto;
    }
  } 

}
