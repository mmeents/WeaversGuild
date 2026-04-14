using System.ComponentModel;
using TheLoomApp.Models;
using Weavers.Core.Interfaces;
using TheLoomApp.Components;
using Weavers.Core.Models;
using Weavers.Core.Enums;
using Weavers.Core.Exceptions;

namespace TheLoomApp.Editors {
  public partial class NumericPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public NumericPropertyEditor() {
      InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;
    private string? _originalValue;
    private DataTypeDto? _columnType;
    public event EventHandler? ValueChanged;

    private ItemPropertyDto? _fieldModel;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemPropertyDto? Field {
      get => _fieldModel;
      set {
        _fieldModel = value;
        if (value != null) {
          DataTypeDto? dataTypeDto = value?.ValueType ?? null;
          _columnType = dataTypeDto;
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

    private void TextBox1_KeyPress(object? sender, KeyPressEventArgs e) {
      // Allow control keys (backspace, delete, etc.)
      if (char.IsControl(e.KeyChar)) return;
      if (_columnType == null) return;
      switch ((WeDataType)_columnType.Id) {
        case WeDataType.Int32:
        case WeDataType.Int64:
          // Allow digits, minus sign at start
          if (!char.IsDigit(e.KeyChar) &&
              !(e.KeyChar == '-' && textBox1.SelectionStart == 0 && !textBox1.Text.Contains('-'))) {
            e.Handled = true;
          }
          break;

        case WeDataType.Decimal128:
          // Allow digits, minus sign at start, one decimal point
          if (!char.IsDigit(e.KeyChar) &&
              !(e.KeyChar == '-' && textBox1.SelectionStart == 0 && !textBox1.Text.Contains('-')) &&
              !(e.KeyChar == '.' && !textBox1.Text.Contains('.'))) {
            e.Handled = true;
          }
          break;
      }
    }

    private void TextBox1_TextChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TextBox1_Leave(object? sender, EventArgs e) {
      ValidateAndFormat();
    }

    private void ValidateAndFormat() {
      if (string.IsNullOrWhiteSpace(textBox1.Text)) return;
      if (_columnType == null) return;  

      try {
        switch ((WeDataType)_columnType.Id) {
          case WeDataType.Int32:
            var int32Value = int.Parse(textBox1.Text);
            textBox1.Text = int32Value.ToString();
            textBox1.BackColor = _isEditing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
            break;

          case WeDataType.Int64:
            var int64Value = long.Parse(textBox1.Text);
            textBox1.Text = int64Value.ToString();
            textBox1.BackColor = _isEditing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
            break;

          case WeDataType.Decimal128:
            var decimalValue = decimal.Parse(textBox1.Text);
            textBox1.Text = decimalValue.ToString("F2");
            textBox1.BackColor = _isEditing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
            break;
        }
      } catch {
        textBox1.BackColor = Color.FromArgb(255, 230, 230); // Light red for error
      }
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        if (_columnType == null) return;
        try {
          switch ((WeDataType)_columnType.Id) {
            case WeDataType.Int32:
              Field.Value = int.Parse(textBox1.Text).ToString();
              break;
            case WeDataType.Int64:
              Field.Value = long.Parse(textBox1.Text).ToString();
              break;
            case WeDataType.Decimal128:
              Field.Value = decimal.Parse(textBox1.Text).ToString();
              break;
          }
          Modified = false;
        } catch {
          throw new ValidationException($"Invalid {_columnType} value: {textBox1.Text}");
        }
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
          if (Field.Name == null) {
            PropertyName = "(Row ID)";
          } else {
            PropertyName = Field?.Name ?? "";
          }
          
        }
        textBox1.Text = Field?.Value ?? "";
        _originalValue = textBox1.Text;
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      if (!editing) ValidateAndFormat();
      textBox1.BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
    }
  }
}
