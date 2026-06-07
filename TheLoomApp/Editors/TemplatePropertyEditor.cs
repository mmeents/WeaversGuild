using Newtonsoft.Json.Linq;
using System.ComponentModel;
using TheLoomApp.Components;
using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace TheLoomApp.Editors {

  [PropertyEditor(WeEditorType.Template, WeDataType.StrAscii)]
  public partial class TemplatePropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    private IComboBoxDataProvider? _dataProvider;
    private ItemPropertyDto? _fieldModel;
    private bool _isEditing;
    private bool _suppressEvents = false;

    public TemplatePropertyEditor() {
      InitializeComponent();
      tabControl1.SelectedTab = tabTemplate;
    }

    private ColumnUIConfig? _columnConfig;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ColumnUIConfig? ColumnConfig {
      get { return _columnConfig; }
      set {
        _columnConfig = value;
        if (_columnConfig != null) {
          if (_columnConfig.Properties.TryGetValue("DataProvider", out var provider) == true) {
            if (provider is IComboBoxDataProvider dataProvider) {
              DataProvider = dataProvider;
            }
          }
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IComboBoxDataProvider? DataProvider {
      get => _dataProvider;
      set {
        _dataProvider = value;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler? ValueChanged;


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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        tabControl1.Left = value + 3;
        tabControl1.Width = this.Width - value - 6;
      }
    }

    public void ResetToField() {
      if (Field == null) { 
        textBox1.Text = string.Empty; 
        return; 
      }
      _suppressEvents = true;
      try { 
        var nameSet = false;
        if (ColumnConfig != null) {
          if (ColumnConfig.LabelText != null) {
            PropertyName = ColumnConfig.LabelText;
            nameSet = true;
          }
          Enabled = !ColumnConfig.ReadOnly;
        }
        if (!nameSet) {
          PropertyName = Field?.Name ?? "";
        }

        textBox1.Text = Field?.Value ?? "";
        tabControl1.SelectedTab = tabTemplate;
        Modified = false;
      } finally { _suppressEvents = false; }
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        Field.Value = textBox1.Text;
        Modified = false;
      }
    }
    private void textBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
      if (_suppressEvents) return;
      Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
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

    private void TemplatePropertyEditor_Resize(object sender, EventArgs e) {
      var value = LabelRight;
      lbName.Left = value - lbName.Width;
      lbName.TextAlign = ContentAlignment.TopRight;
      tabControl1.Width = this.Width - value - 6;
    }

    private async void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
      var currentTab = tabControl1.SelectedTab;
      if (currentTab == tabTemplate) {
        txtPreview.Text = "";
      } else if (currentTab == tabPreview) {

        try {
          if (_dataProvider != null && Field != null) {
            var tmpField = Field.ToClone();
            tmpField.Value = textBox1.Text;
            string result = await _dataProvider.RenderTemplate(tmpField, CancellationToken.None);
            txtPreview.Text = result;
          } else {
            txtPreview.Text = "No data provider configured for rendering.";          }
        } catch (Exception ex) {
          txtPreview.Text = $"Render error: {ex.Message}";
        } finally {
          tabControl1.SelectedTab = tabPreview; // the button's job: show the result
        }

      }
    }
  }
}
