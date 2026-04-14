using System.ComponentModel;
using Weavers.Core.Enums;
using Weavers.Core.Models;
using Weavers.Core.Interfaces;
using TheLoomApp.Models;
using TheLoomApp.Components;

namespace TheLoomApp.Editors {

  [PropertyEditor(WeEditorType.Memo, WeDataType.StrAscii)]
  public partial class MemoPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public MemoPropertyEditor() {
      InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler? ValueChanged;

    private ItemPropertyDto? _fieldModel;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemPropertyDto? Field {
      get => _fieldModel;
      set {
        _fieldModel = value;
        if (value != null) {
          ResetToField();
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        textBox1.Left = value + 3;
        textBox1.Width = this.Width - value - 6;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
    public string PropertyValue {
      get => textBox1.Text;
      set => textBox1.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Modified { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        textBox1.Enabled = value;
      }
    }

    private void TextBox1_TextChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        Field.Value = textBox1.Text;
        Modified = false;
      }
    }

    public void ResetToField() {
      if (Field != null) {
        var nameSet = false;
        if (ColumnConfig != null) {
          if (ColumnConfig.LabelText != null) {
            PropertyName = ColumnConfig.LabelText;
            nameSet = true;
          }
          if (ColumnConfig.ReadOnly) {
            Enabled = false;
          } else {
            Enabled = true;
          }
        }
        if (!nameSet) {
          PropertyName = Field?.Name ?? "";
        }
        textBox1.Text = Field?.Value;     
        Modified = false;
      } else {
        textBox1.Text = string.Empty;
      }
    }

    public void CommitValue() {
      if (Field != null && textBox1 != null) {
        Field.Value = textBox1.Text;        
      }
    }

    public void RevertValue() {
      ResetToField();
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      if (textBox1 != null) {
        textBox1.BackColor = editing ?
            PropertiesTabColors.EditingLabelBackground :
            PropertiesTabColors.StandardEditorWhite;
      }
    }

  }
}
