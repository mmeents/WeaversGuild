using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  public class EditorTypeDto {
    public int Id { get; set; }
    public int Rank { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public bool IsReadOnly { get; set; }
    public string IconName { get; set; } = string.Empty;

  }

  public static class EditorTypeDtoExtensions {
    public static EditorTypeDto ToDto(this Entities.EditorType editorType) {
      return new EditorTypeDto {
        Id = editorType.Id,
        Rank = editorType.Rank,
        Name = editorType.Name,
        Description = editorType.Description,
        IsVisible = editorType.IsVisible,
        IsReadOnly = editorType.IsReadOnly,
        IconName = editorType.IconName
      };
    }
  }

}
