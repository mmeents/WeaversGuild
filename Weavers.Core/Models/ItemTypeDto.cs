using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Models {
  public class ItemTypeDto {
    public int Id { get; set; } = 0;
    public int? ParentTypeId { get; set; }
    public int? EditorTypeId { get; set; }
    public int Rank { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;
    public string IconName { get; set; } = string.Empty;


    public ItemTypeDto? ParentType { get; set; }
    public ICollection<ItemTypeDto> ChildTypes { get; set; } = new List<ItemTypeDto>();

    public EditorTypeDto? EditorType { get; set; }

    public ItemTypeDto() { }
  }

  public static class ItemTypeDtoExtensions {
    public static ItemTypeDto ToDto(this ItemType itemType) {
      return new ItemTypeDto {
        Id = itemType.Id,
        ParentTypeId = itemType.ParentTypeId,
        EditorTypeId = itemType.EditorTypeId,
        Rank = itemType.Rank,
        Name = itemType.Name,
        Description = itemType.Description,
        IsVisible = itemType.IsVisible,
        IsReadOnly = itemType.IsReadOnly,
        IconName = itemType.IconName,
        ParentType = itemType.ParentType?.ToDto(),
        EditorType = itemType.Editor != null ? new EditorTypeDto {
          Id = itemType.Editor.Id,
          Name = itemType.Editor.Name
        } : null
      };
    }
  }
}
