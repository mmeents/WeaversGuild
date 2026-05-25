using System.ComponentModel;
using TheLoomApp.Models;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;
using Weavers.Core.Exceptions;
using TheLoomApp.Components;
using Weavers.Core.Service;

namespace TheLoomApp.Editors {
  /// <summary>
  /// Represents a text property editor control that allows editing of a field's value. <see cref="PropertiesTab"/>
  /// </summary>
  /// <remarks>This control is designed to be used within the PropertiesTab.
  /// Connections with associated with a <see cref="FieldModel"/>. It provides functionality to display and modify the field's value, and
  /// to commit changes back to the field model.</remarks>
  public partial class PasswordPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public PasswordPropertyEditor() {
      InitializeComponent();
      textBox1.UseSystemPasswordChar = true;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private string? _originalValue;
    private bool _isEditing;

    public event EventHandler? ValueChanged;
    public event EventHandler? EditingStarted;
    public bool HasUnsavedChanges =>
        _originalValue != textBox1?.Text;

    private ItemPropertyDto? _fieldModel;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ItemPropertyDto? Field {
      get { return _fieldModel; }
      set {
        _fieldModel = value;
        if (value != null) {
          ResetToField();
        } else {
          PropertyName = string.Empty;
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual int LabelRight {
      get {
        return lbName.Left + lbName.Width;
      }
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        textBox1.Left = value + 3;
        textBox1.Width = this.Width - value - 6;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual string PropertyName {
      get { return lbName.Text; }
      set { lbName.Text = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyValue {
      get { return textBox1.Text; }
      set { textBox1.Text = value; }
    }

    private bool modified = false;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Modified {
      get { return modified; }
      set { modified = value; }
    }

    private bool _enabled = true;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new virtual bool Enabled {
      get {
        return _enabled;
      }
      set {
        _enabled = value;
        if (lbName.Enabled != value) lbName.Enabled = value;
        if (textBox1.Enabled != value) textBox1.Enabled = value;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ICryptoService? CryptoService { get; set; }

    private void SetupCryptoServiceFromSchema() {
      if (Field == null ) return;
      if (ColumnConfig == null) return;
      if (ColumnConfig.Properties.TryGetValue("CryptoService", out var provider) == true) {
        if (provider is ICryptoService cryptoService) {
          CryptoService = cryptoService;
        }
      }
    }

    private void textBox1_TextChanged(object sender, EventArgs e) {
      if (!Modified) {
        Modified = true;
      }
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        try {

          if (CryptoService != null) {
            Field.Value = CryptoService.Encrypt(textBox1.Text);
          } else {
            Field.Value = textBox1.Text;
          }
          
          _originalValue = textBox1.Text;
          Modified = false;
        } catch (ValidationException) {
          textBox1.BackColor = PropertiesTabColors.ValidationError;
          throw;
        }
      }
    }

    public void ResetToField() {
      if (Field != null) {
        bool nameSet = false;
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
        SetupCryptoServiceFromSchema();
        if (CryptoService != null && !string.IsNullOrEmpty(Field?.Value)) {
          try {
            textBox1.Text = CryptoService.Decrypt(Field.Value);
          } catch {
            textBox1.Text = Field.Value;
          }
        } 
        //textBox1.Text = Field?.Value;
        textBox1.UseSystemPasswordChar = true;
        _originalValue = textBox1.Text;
        Modified = false;
      } else {
        textBox1.Text = string.Empty;
      }
    }

    public void CommitValue() {
      if (Field != null && textBox1 != null) {
        if (CryptoService != null) {
          Field.Value = CryptoService.Encrypt(textBox1.Text);
        } else {
          Field.Value = textBox1.Text;
        }
        _originalValue = textBox1.Text;
      }
    }

    public void RevertValue() {
      ResetToField();
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      if (textBox1 != null) {
        textBox1.BackColor = editing ?
            PropertiesTabColors.EditingBackground :
            PropertiesTabColors.StandardEditorWhite;

      }
    }

    private void textBox1_Enter(object sender, EventArgs e) {
      if (!_isEditing) {
        _originalValue = textBox1?.Text;
        EditingStarted?.Invoke(this, EventArgs.Empty);
      }
    }

    private void cbView_CheckedChanged(object sender, EventArgs e) {
      bool isChecked = cbView.Checked;
      if (isChecked) {
        textBox1.UseSystemPasswordChar = false;
      } else {
        textBox1.UseSystemPasswordChar = true;
      }
    }
  }
}

