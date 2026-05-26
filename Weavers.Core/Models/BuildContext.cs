using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  public class BuildContext {
    public bool Success { get; set; } = false;
    public int BuildId { get; init; }
    public bool ForceWrite { get; init; } = false;
    public ConcurrentDictionary<int, ItemDto> LibItems { get; init; } = new ConcurrentDictionary<int, ItemDto>();
    public List<string> Errors { get; } = new List<string>();
    public List<string> Warnings { get; } = new List<string>();
    public string ShellOutput { get; set; } = string.Empty;
    public int FilesWritten { get; set; } = 0;
    public int FilesSkipped { get; set; } = 0;
    public int FilesRemoved { get; set; } = 0;

    public BuildContext Fail(string errorMessage) {
      Errors.Add(errorMessage);
      return this;
    }

    public BuildContext Ok(string warningMessage) {
      Success = true;
      Warnings.Add(warningMessage);
      return this;
    }
  }
}
