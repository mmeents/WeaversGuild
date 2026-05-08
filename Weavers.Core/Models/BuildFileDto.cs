using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Models {

  public class BuildFileDto {
    public int Id { get; set; }
    public int BuildId { get; set; }
    public int ItemId { get; set; }
    public string FilePath { get; set; } = "";
    public bool WasWritten { get; set; }
    public bool WasDeleted { get; set; }

    // Nav properties
    public BuildDto? Build { get; set; } = null;
    public ItemDto? Item { get; set; } = null;  
  
  }


  public static class BuildFileDtoExtensions {
    public static BuildFileDto ToDto(this BuildFile buildFile, bool includeNavs = false) {
      var dto = new BuildFileDto {
        Id = buildFile.Id,
        BuildId = buildFile.BuildId,
        ItemId = buildFile.ItemId,
        FilePath = buildFile.FilePath,
        WasWritten = buildFile.WasWritten,
        WasDeleted = buildFile.WasDeleted        
      };      
      return dto;
    }
  }


}
