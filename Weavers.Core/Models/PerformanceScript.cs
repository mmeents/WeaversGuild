using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;

namespace Weavers.Core.Models {

  public class PerformanceScript {
    public List<PerformanceEntry> Entries { get; set; } = new();
  }

  public class PerformanceEntry {
    public int Rank { get; set; }
    public string Type { get; set; } = Cx.LineType; // "Line" | "Action" | "Narration"    
    public int? CharacterId { get; set; } = null;
    public string CharacterName { get; set; } = "";
    public string Text { get; set; } = "";
  }


}
