using System.ComponentModel;
using Weavers.Core.Interfaces;
using TheLoomApp.Models;
using Weavers.Core.Models;
using Weavers.Core.Extensions;

namespace TheLoomApp.Editors {
  public partial class BooleanPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    private bool _isEditing;
    private bool? _originalValue;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    public BooleanPropertyEditor() {
      InitializeComponent();
    }

    public event EventHandler? ValueChanged;

    private ItemPropertyDto? _field;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemPropertyDto? Field {
      get => _field;
      set {
        _field = value;
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
        checkBox1.Left = value + 3;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyValue {
      get => checkBox1.Checked.ToString();
      set => checkBox1.Checked = bool.TryParse(value, out bool result) && result;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Modified { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        checkBox1.Enabled = value;
      }
    }

    private void CheckBox1_CheckedChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        Field.Value = checkBox1.Checked.ToString();
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
        }
        if (!nameSet) {
          PropertyName = Field?.Name ?? string.Empty;
        }
        var boolValue = Field?.Value?.AsBoolean() ?? false;
        checkBox1.Checked = boolValue;
        _originalValue = boolValue;
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;      
    }
  }

}

