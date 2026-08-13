using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;

namespace Weavers.Core.Models {

  public class CallSheetScript {
    public List<CallSheetScriptItem> Script { get; set; } = new();
  }

  public class CallSheetScriptItem {
    public int Rank { get; set; }
    public string Type { get; set; } = Cx.RoleType; // Role | Narration
    public int? CharacterId { get; set; }
    public string Name { get; set; } = "";
    public string Instruction { get; set; } = "";
  }

}
