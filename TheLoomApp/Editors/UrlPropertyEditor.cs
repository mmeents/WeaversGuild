
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Policy;
using TheLoomApp.Components;
using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;
using Weavers.Core.Extensions;


namespace TheLoomApp.Editors {
  [PropertyEditor(WeEditorType.RelativeFolder, WeDataType.StrAscii)]
  public partial class UrlPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
      
    public UrlPropertyEditor() {
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
      var local = Path.GetDirectoryName(textBox1.Text);
      if (local != null && local.IsValidBrowserUrl()) {
        Process.Start(new ProcessStartInfo {
          FileName = local,
          UseShellExecute = true
        });
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
        SetupDataProviderFromSchema();
        textBox1.Text = Field?.Value ?? "";
        Modified = false;
      }
    }

    private IComboBoxDataProvider? _dataProvider;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IComboBoxDataProvider? DataProvider {
      get => _dataProvider;
      set {
        _dataProvider = value;    
      }
    }

    private void SetupDataProviderFromSchema() {
      if (Field == null) return;
      if (ColumnConfig == null) return;
      if (ColumnConfig.Properties.TryGetValue("DataProvider", out var provider) == true) {
        if (provider is IComboBoxDataProvider dataProvider) {
          DataProvider = dataProvider;
        }
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      textBox1.BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.StandardEditorWhite;
    }
  }
}
