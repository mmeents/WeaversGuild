using System.ComponentModel;
using Weavers.Core.Interfaces;
using TheLoomApp.Models;
using Weavers.Core.Models;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;
using TheLoomApp.Components;


namespace TheLoomApp.Editors {

  [PropertyEditor(WeEditorType.LookupTypeEditor, WeDataType.Int)]
  public partial class ComboBoxPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {    
    private IComboBoxDataProvider? _dataProvider;    
    private ItemPropertyDto? _fieldModel;
    private string? _originalValue;
    private bool _isLoading = false;

    public event EventHandler? ValueChanged;

    public ComboBoxPropertyEditor() {
      InitializeComponent();
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
    public ItemPropertyDto? Field {
      get => _fieldModel;
      set {
        _fieldModel = value;
        if (value != null) {
          ResetToField();
        }
      }
    }

    // Data provider for getting combo box items
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IComboBoxDataProvider? DataProvider {
      get => _dataProvider;
      set {
        _dataProvider = value;
      }
    }

    public void ResetToField() {
      if (Field == null) { return; }
            
      var nameSet = false;
      if (ColumnConfig != null) {
        if (ColumnConfig.LabelText != null) { 
          PropertyName = ColumnConfig.LabelText; 
          nameSet = true; 
        }
        Enabled = !ColumnConfig.ReadOnly;
      }
      if (!nameSet) PropertyName = Field.Name ?? "";

      _originalValue = Field.Value;
      Modified = false;

      _ = LoadAsync(); // single ordered pass owns selection restoration
    }

    private async Task LoadAsync() {
      if (_dataProvider == null || _isLoading) return;

      try {
        _isLoading = true; 
        SetComboBusy(true);
        
        int? typeId = Field?.ReferenceItemTypeId;
        var items = (typeId is null or 0)
            ? Enumerable.Empty<ItemLookup>()
            : await _dataProvider.GetValuesAsync(typeId);

        RunOnUi(() => FillCombo(items, _originalValue));
      } catch (Exception ex) {
        RunOnUi(() => ShowComboError(ex.Message));
      } finally {
        _isLoading = false;
        RunOnUi(() => comboBox1.Enabled = this.Enabled);
      }
    }

    private void FillCombo(IEnumerable<ItemLookup> items, string? restoreId) {
      comboBox1.Items.Clear();
      comboBox1.Items.Add(new ItemLookup("", "(None)")); // index 0

      int restoreIndex = 0; // default (None)
      foreach (var item in items) {
        int idx = comboBox1.Items.Add(item);
        if (!string.IsNullOrEmpty(restoreId) &&
            Equals(item.Value?.ToString(), restoreId)) {
          restoreIndex = idx;
        }
      }
      comboBox1.SelectedIndex = restoreIndex; 
    }

    private void SetComboBusy(bool busy) {
      RunOnUi(() => {
        if (busy) { comboBox1.Items.Clear(); comboBox1.Items.Add("Loading..."); comboBox1.Enabled = false; }
      });
    }

    private void ShowComboError(string message) {
      comboBox1.Items.Clear();
      comboBox1.Items.Add($"Error: {message}");
    }

    private void RunOnUi(Action action) {
      if (InvokeRequired) Invoke(action);
      else action();
    }
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        comboBox1.Left = value + 3;
        comboBox1.Width = this.Width - value - 6;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyValue {
      get => comboBox1.SelectedItem?.ToString() ?? "";
      set {
        // First try to find exact match in ItemLookup objects
        foreach (var item in comboBox1.Items) {
          if (item is ItemLookup comboItem) {
            if (Equals(comboItem.Value?.ToString(), value)) {
              comboBox1.SelectedItem = item;
              return;
            }
          } else if (Equals(item.ToString(), value)) {
            comboBox1.SelectedItem = item;
            return;
          }
        }

        // Fallback - set to null/empty
        comboBox1.SelectedIndex = string.IsNullOrEmpty(value) ? 0 : -1;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Modified { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        comboBox1.Enabled = value && !_isLoading;
      }
    }

    private void ComboBox1_SelectedIndexChanged(object? sender, EventArgs e) {
      if (_isLoading) return;
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        ItemLookup? selectedItem = comboBox1.SelectedItem as ItemLookup;
        Field.Value = Convert.ToString( selectedItem?.Value ?? "");
      }
    }

    public void SetEditingState(bool editing) {      
      comboBox1.BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.StandardEditorWhite;      
    }

  }
}
