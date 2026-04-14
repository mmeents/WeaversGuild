using Weavers.Core.Enums;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace TheLoomApp.Editors {


  /// <summary>
  /// Factory for creating property editors based on column types and metadata
  /// </summary>
  public static class PropertyEditorFactory {

    /// <summary>
    /// Creates an editor based on the column type with automatic type inference
    /// </summary>
    public static IAmAFieldEditor? CreateEditor(ItemPropertyDto field, IItemTypeLookupComboProvider itemTypeLookupComboProvider) {
      
      WeEditorType editorType = (field==null || field.EditorTypeId == null) ? WeEditorType.String : (WeEditorType)field.EditorTypeId;
      ColumnUIConfig? config = new ColumnUIConfig();      
      config.WithLookupProvider(itemTypeLookupComboProvider).UseEditor(editorType);
      return CreateEditor(editorType, config, field);
    }

    /// <summary>
    /// Creates an editor of a specific type
    /// </summary>
    public static IAmAFieldEditor? CreateEditor(WeEditorType editorType, ColumnUIConfig? config, ItemPropertyDto? field = null) {
      IAmAFieldEditor editor = editorType switch {
        WeEditorType.Boolean => new BooleanPropertyEditor(),
        WeEditorType.Integer => new NumericPropertyEditor(),
        WeEditorType.String => new TextPropertyEditor(),
        WeEditorType.LookupTypeEditor => new ComboBoxPropertyEditor(),      
        WeEditorType.FileName => new FilePickerPropertyEditor(),
        WeEditorType.Memo => new MemoPropertyEditor(),
        WeEditorType.Folder => new FolderPickerPropertyEditor(),
        WeEditorType.RelativeFolder => new RelativeFolderPropertyEditor(),
        WeEditorType.Url => new UrlPropertyEditor(),
        _ => new TextPropertyEditor()
      };
      if (config != null) {
        editor.ColumnConfig = config;
      }
      if (editor != null && field != null) {
        editor.Field = field;
      }

      return editor;
    }    

    /// <summary>
    /// Gets the recommended height for an editor type
    /// </summary>
    public static int GetRecommendedHeight(WeEditorType editorType) {
      return editorType switch {
        WeEditorType.Memo => 100,     
        _ => 30
      };
    }

    /// <summary>
    /// Registers a custom editor factory function for extensibility
    /// </summary>
    private static readonly Dictionary<WeEditorType, Func<IAmAFieldEditor>> _customFactories = new();

    public static void RegisterCustomEditor(WeEditorType editorType, Func<IAmAFieldEditor> factory) {
      _customFactories[editorType] = factory;
    }
  }

  /// <summary>
  /// Attribute for marking editors with their supported column types
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class PropertyEditorAttribute : Attribute {
    public WeDataType[] SupportedTypes { get; }
    public WeEditorType EditorType { get; }

    public PropertyEditorAttribute(WeEditorType editorType, params WeDataType[] supportedTypes) {
      EditorType = editorType;
      SupportedTypes = supportedTypes;
    }
  }


}
