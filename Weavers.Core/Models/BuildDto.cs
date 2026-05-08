using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Models {
  public class BuildDto {
    public int Id { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Status { get; set; }
    public BuildStatus BuildStatus => (BuildStatus)Status;
    public string? BuildOutput { get; set; } = "";
    public string? CompilerOutput { get; set; }
    public int LibraryItemId { get; set; }

    // Nav properties
    public ItemDto? LibraryItem { get; set; } = null;
    // Inbound nav properties
    public ICollection<BuildFileDto> BuildFiles { get; set; } = new List<BuildFileDto>();

  }

  public enum BuildStatus {
    Pending = 1,
    InProgress = 2,
    Success = 3,
    Failed = 4
  }

  public static class BuildDtoExtensions {
    public static BuildDto ToDto(this Build build, bool includeBuildFiles,  bool includeLib = false) {
      var dto = new BuildDto {
        Id = build.Id,
        StartedAt = build.StartedAt,
        CompletedAt = build.CompletedAt,
        Status = build.Status,
        BuildOutput = build.BuildOutput ?? "",
        CompilerOutput = build.CompilerOutput,
        LibraryItemId = build.LibraryItemId
      };
      if (includeLib) { // ef core recursive outs.
        dto.LibraryItem = build.LibraryItem?.ToDto();        
      }
      if (includeBuildFiles) {
        dto.BuildFiles = build.BuildFiles.Select(bf => bf.ToDto()).ToList();
        foreach (var file in dto.BuildFiles) {
          file.Build = dto;
        }
      }
      return dto;
    }

  }

}
