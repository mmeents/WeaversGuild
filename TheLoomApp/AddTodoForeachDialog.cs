using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace TheLoomApp {
  public partial class AddTodoForeachDialog : Form {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IComboBoxDataProvider? _dataProvider;
    private bool _isLoading = false;
    private string? _originalValue;
    private string? _originalRefTypeId;
    public AddTodoForeachDialog(IServiceScopeFactory scopeFactory) {
      InitializeComponent();
      _scopeFactory = scopeFactory;
      var scope = _scopeFactory.CreateScope();
      _dataProvider = scope.ServiceProvider.GetRequiredService<IItemTypeLookupComboProvider>();

    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DeskId { get; set; } = 0;


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DeskName {
      get => lbAtDesk.Text;
      set {
        lbAtDesk.Text = $"At Desk: {value}";
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PromptTemplate {
      get => edPromptTemplate.Text;
      set => edPromptTemplate.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ForeachList {
      get => edForeach.Text;
      set => edForeach.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? TypeRefId {
      get {
        return (cbType.SelectedItem as ItemLookup)?.Value is { } v
            && int.TryParse(v.ToString(), out var id) ? id : (int?)null;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? RefId {
      get { 
        return (cbValues.SelectedItem as ItemLookup)?.Value is { } v
            && int.TryParse(v.ToString(), out var id) ? id : (int?)null;
      }      
    }

    public string FirstForeachItemPreview => edForeach.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";

    private void btnNext_Click(object sender, EventArgs e) {
      if (edForeach.Text.Trim() == "") { return; }
      lbItemDemo.Text = "then: " + FirstForeachItemPreview;
      tabState.SelectedTab = tabTodo;
    }

    private void btnBack_Click(object sender, EventArgs e) {
      tabState.SelectedTab = tabForeach;
    }

    private void AddTodoForeachDialog_Shown(object sender, EventArgs e) {
      _originalRefTypeId = WeItemType.TodoModel.AsIntString();
      _ = LoadAsync();
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
      RunOnUi(() => {
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
      _originalValue = null;            // old value invalid under new type

      int? typeId = SelectedId(cbType);
      await ReloadValuesAsync(typeId);      
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


  }
}
