using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using Weavers.Core.Interfaces;
using Weavers.Core.Enums;

namespace Weavers.Core.Models {
  public class ColumnUIConfig {
    public WeEditorType? EditorType { get; set; }  // null = auto-detect from ColumnType
    public bool ReadOnly { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;   // 0 = use column rank
    public bool Visible { get; set; } = true;

    // Simple properties for now
    public string? LabelText { get; set; } = null;
    public string? Tooltip { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
  }


  public static class ColumnUIConfigExtensions {
    public static ColumnUIConfig UseEditor(this ColumnUIConfig config, WeEditorType editorType) {
      config.EditorType = editorType;
      return config;
    }

    public static ColumnUIConfig AsReadOnly(this ColumnUIConfig config) {
      config.ReadOnly = true;
      return config;
    }

    public static ColumnUIConfig WithLabelText(this ColumnUIConfig config, string labelText) {
      config.LabelText = labelText;
      return config;
    }

    public static ColumnUIConfig WithOrder(this ColumnUIConfig config, int order) {
      config.DisplayOrder = order;
      return config;
    }

    public static ColumnUIConfig WithTooltip(this ColumnUIConfig config, string tooltip) {
      config.Tooltip = tooltip;
      return config;
    }

    public static ColumnUIConfig Hide(this ColumnUIConfig config) {
      config.Visible = false;
      return config;
    }

    public static ColumnUIConfig SetProperty(this ColumnUIConfig config, string key, object value) {
      config.Properties[key] = value;
      return config;
    }

    //public static ColumnUIConfig WithStaticItems(this ColumnUIConfig config, params string[] items) {
    //  config.UseEditor(EditorType.ComboBox);
    //  config.SetProperty("DataProvider", new StaticListProvider(items));
    //  return config;
   // }

   // public static ColumnUIConfig WithStaticItems(this ColumnUIConfig config, IEnumerable<ComboBoxItem> items) {
   //   config.UseEditor(EditorType.ComboBox);
   //   config.SetProperty("DataProvider", new StaticListProvider(items));
   //   return config;
    //}

    public static ColumnUIConfig WithLookupProvider(this ColumnUIConfig config, IComboBoxDataProvider provider) {
      config.UseEditor(WeEditorType.LookupTypeEditor);
      config.SetProperty("DataProvider", provider);
      return config;
    }
  }
}
