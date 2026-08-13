using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  public class ScheduleActorPerformanceResults {
    public int SceneId { get; set; }
    public int PerformanceId { get; set; }
    public List<int> ActorPerformanceIds { get; set; } = new List<int>();

    public List<int> AddedTodoIds { get; set; } = new List<int>();
    public List<int> Skipped { get; set; } = new List<int>();
    public List<string> Errors { get; set; } = new List<string>();
    public bool AllScheduled => Errors.Count == 0 && (AddedTodoIds.Count + Skipped.Count) == ActorPerformanceIds.Count;
  }
}
