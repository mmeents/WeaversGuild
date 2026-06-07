using System.ComponentModel;
using Weavers.Core.Interfaces;
using TheLoomApp.Models;
using Weavers.Core.Models;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;
using TheLoomApp.Components;


namespace TheLoomApp.Editors {

  [PropertyEditor(WeEditorType.Reference, WeDataType.Int)]
  public partial class ReferencePropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    private IComboBoxDataProvider? _dataProvider;
    private ItemPropertyDto? _fieldModel;
    private bool _isLoading = false;
    private string? _originalValue;
    private string? _originalRefTypeId;

    public event EventHandler? ValueChanged;

    public ReferencePropertyEditor() {
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<object> Items { get; set; } = new();

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DisplayMember { get; set; } = "";
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ValueMember { get; set; } = "";

    #region Loading v2 

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

      if (!nameSet) {
        PropertyName = Field?.Name ?? "";
      }

      _originalRefTypeId = Field?.ReferenceItemTypeId?.ToString(); 
      _originalValue = Field?.Value;
      Modified = false;

      _ = LoadAsync(); ;      
    }
    
    private async Task LoadAsync() {
      if (_dataProvider == null || _isLoading) return;

      try {
        _isLoading = true; // suppresses SelectedIndexChanged handlers during programmatic fill

        SetComboBusy(cbType, true);
        SetComboBusy(cbValues, true);

        // 1) TYPES — load once, fill, restore the captured type id.
        var types = await _dataProvider.GetTypesAsync();
        RunOnUi(() => FillCombo(cbType, types, _originalRefTypeId));

        // 2) VALUES — load for whatever type ended up selected, restore captured value.
        //    Null/NotSet type => empty value list (nothing pickable yet). No guessing.
        int? typeId = SelectedId(cbType);
        var values = (typeId is null or 0)
            ? Enumerable.Empty<ItemLookup>()
            : await _dataProvider.GetValuesAsync(typeId);
        RunOnUi(() => FillCombo(cbValues, values, _originalValue));
      } catch (Exception ex) {
        RunOnUi(() => ShowComboError(cbValues, ex.Message));
      } finally {
        _isLoading = false;
        RunOnUi(() => {
          cbType.Enabled = this.Enabled;
          cbValues.Enabled = this.Enabled;
        });
      }
    }

    // ---- small helpers ----

    private void FillCombo(ComboBox box, IEnumerable<ItemLookup> items, string? restoreId) {
      box.Items.Clear();
      box.Items.Add(new ItemLookup("", "(None)")); // index 0

      int restoreIndex = 0; // default to (None)
      foreach (var item in items) {
        int idx = box.Items.Add(item);
        if (!string.IsNullOrEmpty(restoreId) &&
            Equals(item.Value?.ToString(), restoreId)) {
          restoreIndex = idx;
        }
      }
      box.SelectedIndex = restoreIndex; // set ONCE, on a populated list
    }

    private static int? SelectedId(ComboBox box)
        => (box.SelectedItem as ItemLookup)?.Value is { } v
           && int.TryParse(v.ToString(), out var id) ? id : (int?)null;

    private void SetComboBusy(ComboBox box, bool busy) {
      RunOnUi(() =>
      {
        if (busy) { box.Items.Clear(); box.Items.Add("Loading..."); box.Enabled = false; }
      });
    }

    private void ShowComboError(ComboBox box, string message) {
      box.Items.Clear();
      box.Items.Add($"Error: {message}");
    }

    private void RunOnUi(Action action) {
      if (InvokeRequired) Invoke(action);
      else action();
    }

    private async void cbType_SelectedIndexChanged(object sender, EventArgs e) {
      if (_isLoading) return;            // ignore programmatic fills
      Modified = true;                  // type-only change must commit
      _originalValue = null;            // old value invalid under new type

      int? typeId = SelectedId(cbType);
      await ReloadValuesAsync(typeId);
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    // Reload ONLY the value list — used when the user deliberately changes the type.
    private async Task ReloadValuesAsync(int? typeId) {
      if (_dataProvider == null) return;

      try {
        _isLoading = true;
        SetComboBusy(cbValues, true);

        var values = (typeId is null or 0)
            ? Enumerable.Empty<ItemLookup>()
            : await _dataProvider.GetValuesAsync(typeId);

        // originalValue is intentionally NULL here: the old value belonged to the
        // old type and is no longer valid, so default to (None).
        RunOnUi(() => FillCombo(cbValues, values, restoreId: null));
      } catch (Exception ex) {
        RunOnUi(() => ShowComboError(cbValues, ex.Message));
      } finally {
        _isLoading = false;
        RunOnUi(() => cbValues.Enabled = this.Enabled);
      }
    }


    #endregion
      

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        cbValues.Left = value + 3;
        cbValues.Width = this.Width - value - 6;
        cbType.Left = value + 3;
        cbType.Width = cbValues.Width;
      }
    }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyValue {
      get => cbValues.SelectedItem?.ToString() ?? "";
      set {
        if (string.IsNullOrEmpty(value)) return;
        // First try to find exact match in ItemLookup objects
        foreach (var item in cbValues.Items) {
          if (item is ItemLookup comboItem) {
            if (Equals(comboItem.Value?.ToString(), value)) {
              cbValues.SelectedItem = item;
              return;
            }
          } else if (Equals(item.ToString(), value)) {
            cbValues.SelectedItem = item;
            return;
          }
        }       

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
        cbValues.Enabled = value && !_isLoading;
      }
    }

    private void ComboBox1_SelectedIndexChanged(object? sender, EventArgs e) {
      if (_isLoading) return;
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null || !Modified) return;


      var typeSel = cbType.SelectedItem as ItemLookup;
      var valueSel = cbValues.SelectedItem as ItemLookup;

      Field.Value = Convert.ToString(valueSel?.Value ?? "");
      var typeStr = Convert.ToString(typeSel?.Value ?? "");
      Field.ReferenceItemTypeId = int.TryParse(typeStr, out var t) ? t : (int?)null;
    }


    public void SetEditingState(bool editing) {
      cbValues.BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.StandardEditorWhite;
    }


  }
}
