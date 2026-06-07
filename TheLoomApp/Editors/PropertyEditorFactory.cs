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
    public static IAmAFieldEditor? CreateEditor(ItemPropertyDto field, IItemTypeLookupComboProvider itemTypeLookupComboProvider, ICryptoService cryptoService) {
      
      WeEditorType editorType = (field==null || field.EditorTypeId == null) ? WeEditorType.String : (WeEditorType)field.EditorTypeId;
      if (editorType == WeEditorType.Hidden || editorType == WeEditorType.None) { 
        return null;
      }
      ColumnUIConfig? config = new ColumnUIConfig();            
      config.WithCryptoProvider(cryptoService).WithLookupProvider(itemTypeLookupComboProvider).UseEditor(editorType);
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
        WeEditorType.Password => new PasswordPropertyEditor(),
        WeEditorType.LookupTypeEditor => new ComboBoxPropertyEditor(),      
        WeEditorType.FileName => new FilePickerPropertyEditor(),
        WeEditorType.Memo => new MemoPropertyEditor(),
        WeEditorType.Folder => new FolderPickerPropertyEditor(),
        WeEditorType.RelativeFolder => new RelativeFolderPropertyEditor(),
        WeEditorType.Url => new UrlPropertyEditor(),
        WeEditorType.Reference => new ReferencePropertyEditor(),
        WeEditorType.Template => new TemplatePropertyEditor(),
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
