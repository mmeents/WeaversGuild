using System.ComponentModel;
using Weavers.Core.Interfaces;
using TheLoomApp.Models;
using Weavers.Core.Models;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;
using TheLoomApp.Components;


namespace TheLoomApp.Editors {

  [PropertyEditor(WeEditorType.CmdPickerEditor, WeDataType.StrAscii)]
  public partial class CmdPickerPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    private IComboBoxDataProvider? _dataProvider;
    private ItemPropertyDto? _fieldModel;
    private string? _originalValue;
    private bool _isLoading = false;

    public event EventHandler? ValueChanged;

    public CmdPickerPropertyEditor() {
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
        RunOnUi(() => edCmdChkList.Enabled = this.Enabled);
      }
    }

    private void FillCombo(IEnumerable<ItemLookup> items, string? restoreId) {

      // 1. Prevent the UI from repainting during a bulk update
      edCmdChkList.BeginUpdate();
      try {
        edCmdChkList.Items.Clear();

        // 2. Parse the IDs into a HashSet for O(1) lookups
        HashSet<int> selected = new HashSet<int>();
        if (!string.IsNullOrEmpty(restoreId)) {
          selected = restoreId
              .Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(s => int.TryParse(s, out int id) ? id : (int?)null)
              .Where(id => id.HasValue)
              .Select(id => id!.Value)
              .ToHashSet();
        }

        // 3. Populate and check items
        foreach (var item in items) {
          int itemId = Convert.ToInt32(item.Value);

          // Fix: Pass just the text, or text + imageIndex if you have icons
          ListViewItem dd = new ListViewItem(item.DisplayText);

          // Store the actual integer ID in the Tag property for later retrieval
          dd.Tag = itemId;

          // Determine checked state before adding to avoid multiple UI triggers
          if (selected.Contains(itemId)) {
            dd.Checked = true;
          }

          edCmdChkList.Items.Add(dd);
        }
      } finally {
        // 4. Ensure the UI redraws even if an exception occurs
        edCmdChkList.EndUpdate();
      }
    }

    private void SetComboBusy(bool busy) {
      RunOnUi(() => {
        if (busy) {
          edCmdChkList.Items.Clear();
          edCmdChkList.Enabled = false;
        }
      });
    }

    private void ShowComboError(string message) {
      edCmdChkList.Items.Clear();
      edCmdChkList.Items.Add($"Error: {message}");
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
        edCmdChkList.Left = value + 3;
        edCmdChkList.Width = this.Width - value - 6;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyValue {
      get {
        if (Field == null) return string.Empty;

        List<string> selectedValues = new List<string>();

        // Loop through the checked ListViewItems
        foreach (ListViewItem checkedItem in edCmdChkList.CheckedItems) {
          // Retrieve the ID we stored in the Tag property
          if (checkedItem.Tag != null) {
            selectedValues.Add(checkedItem.Tag.ToString() ?? "");
          }
        }

        return string.Join(",", selectedValues);
      }
      set {
        // 1. Parse the incoming comma-separated string into a lookup set
        HashSet<int> selected = new HashSet<int>();
        if (!string.IsNullOrEmpty(value)) {
          selected = value
              .Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(s => int.TryParse(s, out int id) ? id : (int?)null)
              .Where(id => id.HasValue)
              .Select(id => id!.Value)
              .ToHashSet();
        }

        // 2. Wrap in BeginUpdate to stop UI flickering while updating checks
        edCmdChkList.BeginUpdate();
        try {
          foreach (ListViewItem item in edCmdChkList.Items) {
            // Extract the ID from the Tag
            if (item.Tag is int itemId) {
              // Directly set the Checked boolean state
              item.Checked = selected.Contains(itemId);
            } else {
              // Uncheck if the Tag is missing or invalid
              item.Checked = false;
            }
          }
        } finally {
          edCmdChkList.EndUpdate();
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
        edCmdChkList.Enabled = value && !_isLoading;
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
        Field.Value = PropertyValue;
      }
    }

    public void SetEditingState(bool editing) {      
      edCmdChkList.BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.StandardEditorWhite;
    }

    private void edCmdChkList_ItemChecked(object sender, ItemCheckedEventArgs e) {
      if (_isLoading) return;
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
