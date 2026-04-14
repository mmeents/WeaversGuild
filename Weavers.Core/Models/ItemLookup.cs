using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  public class ItemLookup {
    public object Value { get; set; }
    public string DisplayText { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;

    public ItemLookup(object value, string displayText, string? description = null) {
      Value = value;
      DisplayText = displayText;
      Description = description;
    }

    public override string ToString() => DisplayText;
  }

}
