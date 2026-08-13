using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  public class ScheduleBeatDirectorResults {
    public int StoryId { get; set; }
    public int SceneId { get; set; }
    public List<int> BeatIds { get; set; } = new List<int>();
    public List<int> AddedTodoIds { get; set; } = new List<int>();

    public List<int> Skipped { get; set; } = new List<int>();
    public List<string> Errors { get; set; } = new List<string>();
    public bool AllScheduled => Errors.Count == 0 && (AddedTodoIds.Count + Skipped.Count) == BeatIds.Count;
  }
}
