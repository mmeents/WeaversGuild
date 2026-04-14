using System.ComponentModel;
using Weavers.Core.Enums;
using Weavers.Core.Interfaces;
using TheLoomApp.Models;
using TheLoomApp.Components;
using Weavers.Core.Extensions;
using Weavers.Core.Models;


namespace TheLoomApp.Editors {

  [PropertyEditor(WeEditorType.FileName, WeDataType.StrAscii)]
  public partial class FilePickerPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
      
    public FilePickerPropertyEditor() {
      InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ColumnUIConfig? ColumnConfig { get; set; } = null;
    private bool _isEditing;

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

    // FilePicker specific properties
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Filter { get; set; } = "All files (*.*)|*.*";
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool CheckFileExists { get; set; } = true;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SaveMode { get; set; } = false;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        textBox1.Left = value + 3;
        textBox1.Width = this.Width - value - btnBrowse.Width - 9;
        btnBrowse.Left = this.Width - btnBrowse.Width - 4;
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
        btnBrowse.Enabled = value;
      }
    }

    private void TextBox1_TextChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void BtnBrowse_Click(object? sender, EventArgs e) {
      if (SaveMode) {
        using var saveDialog = new SaveFileDialog();
        saveDialog.Filter = Filter;
        saveDialog.FileName = textBox1.Text;

        if (saveDialog.ShowDialog() == DialogResult.OK) {
          textBox1.Text = saveDialog.FileName;
        }
      } else {
        using var openDialog = new OpenFileDialog();
        openDialog.Filter = Filter;
        openDialog.CheckFileExists = CheckFileExists;
        openDialog.FileName = textBox1.Text;

        if (openDialog.ShowDialog() == DialogResult.OK) {
          textBox1.Text = openDialog.FileName;
        }
      }
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
        textBox1.Text = Field?.Value ?? "";
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      textBox1.BackColor = editing ?
        PropertiesTabColors.EditingBackground :
        PropertiesTabColors.StandardEditorWhite;
    }
  }
}
