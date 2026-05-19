using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.ItemTypes;

namespace TheLoomApp {
  public partial class GetNewItemDetailsDialog : Form {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private WeItemType _targetTypeToCreate;
    public string ItemName => edName.Text;
    public WeItemType? NewFileType { get; private set; } = WeItemType.FileMdModel;
    public WeItemType? NewDataType {
      get {
        if (_targetTypeToCreate == WeItemType.ClassPropertyModel) {
          var selectedIndex = cbDataType.SelectedIndex;
          if (_dataTypeMap.TryGetValue(selectedIndex, out var dataType)) {
            return dataType;
          }
        }
        return null;
      }
    }
    public bool IsAsync => cbIsAsync.Checked;
    public bool IsNav => cbIsAsync.Checked;
    public int? LookupItemId {
      get {
        var selectedIndex = cbItemLookup.SelectedIndex;
        if (_lookupMap.TryGetValue(selectedIndex, out var itemId)) {
          return itemId;
        }
        return null;
      }
    }
    public string? DbTableName => edDbTableName.Text;

    public GetNewItemDetailsDialog(IServiceScopeFactory serviceScopeFactory, WeItemType targetTypeToCreate) {
      _serviceScopeFactory = serviceScopeFactory;
      _targetTypeToCreate = targetTypeToCreate;
      InitializeComponent();
      this.Text = $"Create New {_targetTypeToCreate.Description()}";
      lbAddTarget.Text = $"Please enter the following for the new {_targetTypeToCreate.Description()}";
      cbIsAsync.Visible = false;
      cbDataType.Visible = false;
      cbNewFileType.Visible = false;
      lbItemType.Visible = false;
      lbWhich.Visible = false;
      cbItemLookup.Visible = false;
      edDbTableName.Visible = false;
      lbDbTableName.Visible = false;

      if (_targetTypeToCreate == WeItemType.FileMdModel) {
        cbNewFileType.Top = edName.Top + edName.Height + 10;
        cbNewFileType.Left = edName.Left;
        cbNewFileType.SelectedIndex = 0;
        lbItemType.Top = cbNewFileType.Top;
        lbItemType.Left = lbNewItemName.Left;
        cbNewFileType.Visible = true;
        lbItemType.Visible = true;
      }

      if (_targetTypeToCreate == WeItemType.ClassPropertyModel) {
        cbDataType.Top = edName.Top + edName.Height + 10;
        cbDataType.Left = edName.Left;
        lbItemType.Top = cbDataType.Top;
        lbItemType.Left = lbNewItemName.Left;
        loadDataTypes();

        lbItemType.Visible = true;
        cbDataType.Visible = true;
      }

      if (_targetTypeToCreate == WeItemType.ClassMethodModel) {
        cbDataType.Top = edName.Top + edName.Height + 10;
        cbDataType.Left = edName.Left;
        loadDataTypes();

        lbItemType.Top = cbDataType.Top;
        lbItemType.Text = "Return Type";
        lbItemType.Left = lbNewItemName.Left + lbNewItemName.Width - lbItemType.Width;

        cbIsAsync.Visible = true;
        lbItemType.Visible = true;
        cbDataType.Visible = true;
      }

      if (_targetTypeToCreate == WeItemType.ClassMethodParameterModel) {
        cbDataType.Top = edName.Top + edName.Height + 10;
        cbDataType.Left = edName.Left;
        loadDataTypes();
        lbItemType.Top = cbDataType.Top;
        lbItemType.Text = "Parameter Type";
        lbItemType.Left = lbNewItemName.Left + lbNewItemName.Width - lbItemType.Width;
        lbItemType.Visible = true;
        cbDataType.Visible = true;
      }

      if (_targetTypeToCreate == WeItemType.EntityClassModel) {
        edDbTableName.Top = edName.Top + edName.Height + 10;
        edDbTableName.Left = edName.Left;
        lbDbTableName.Top = edDbTableName.Top;
        lbDbTableName.Left = lbNewItemName.Left + lbNewItemName.Width - lbDbTableName.Width;
        edDbTableName.Visible = true;
        lbDbTableName.Visible = true;
      }

      if (_targetTypeToCreate == WeItemType.EntityPropertyModel) {
        lbItemType.Text = "Property Type";
        lbItemType.Top = edName.Top + edName.Height + 10;
        lbItemType.Left = lbNewItemName.Left + lbNewItemName.Width - lbItemType.Width;
        cbDataType.Top = lbItemType.Top;
        cbDataType.Left = lbItemType.Left + lbItemType.Width + 10;
        cbIsAsync.Text = "Is Navigation Property?";
        loadDataTypes();
        cbIsAsync.Visible = true;
        lbItemType.Visible = true;
        cbDataType.Visible = true;
      }

    }

    private ConcurrentDictionary<int, WeItemType> _dataTypeMap = new ConcurrentDictionary<int, WeItemType>();
    private void loadDataTypes() {
      cbDataType.Items.Clear();
      int defaultsToIntIndex = 0;
      WeItemType startAt = WeItemType.CSharpIntType;
      if (_targetTypeToCreate == WeItemType.EntityPropertyModel) {
        startAt = WeItemType.CSharpStringType;
      }
      for (WeItemType i = startAt; i <= WeItemType.CSharpGuidType; i++) {
        var index = cbDataType.Items.Add(i.Description());
        _dataTypeMap[index] = i;
        if (i == WeItemType.CSharpIntType) {
          defaultsToIntIndex = index;
        }
      }
      cbDataType.SelectedIndex = defaultsToIntIndex;
    }

    private ConcurrentDictionary<int, int> _lookupMap = new ConcurrentDictionary<int, int>();
    private async void cbDataType_SelectedIndexChanged(object sender, EventArgs e) {
      var selectedIndex = cbDataType.SelectedIndex;
      if (selectedIndex != -1 && _dataTypeMap.Keys.Contains(selectedIndex)) {
        var selectedDataType = _dataTypeMap[selectedIndex];
        if (selectedDataType == WeItemType.CSharpClassType
          || selectedDataType == WeItemType.CSharpStructType
          || selectedDataType == WeItemType.CSharpRecordType) {


          using var scope = _serviceScopeFactory.CreateScope();
          var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          var items = await _mediator.Send(new GetItemsByItemTypeQuery((int)selectedDataType), CancellationToken.None);

          cbItemLookup.Items.Clear();
          foreach (var item in items) {
            if (item != null && item.Description != null) {
              var index = cbItemLookup.Items.Add(item.DisplayText);
              _lookupMap[index] = item.Value.AsInt();
            }
          }
          if (cbItemLookup.Items.Count > 0) {
            cbItemLookup.SelectedIndex = 0;
          }

          cbItemLookup.Left = cbDataType.Left;
          cbItemLookup.Top = cbDataType.Top + cbDataType.Height + 10;
          cbItemLookup.Visible = true;
          lbWhich.Left = lbNewItemName.Left + lbNewItemName.Width - lbWhich.Width;
          lbWhich.Top = cbItemLookup.Top;
          lbWhich.Visible = true;

        } else {
          lbWhich.Visible = false;
          cbItemLookup.Visible = false;
        }

      }
    }

    private void GetNewItemDetailsDialog_Load(object sender, EventArgs e) {

    }

    private void GetNewItemDetailsDialog_Shown(object sender, EventArgs e) {

    }

    private void cbNewFileType_SelectedIndexChanged(object sender, EventArgs e) {
      var NewSelectedIndex = cbNewFileType.SelectedIndex;
      if (NewSelectedIndex == 0) {
        NewFileType = WeItemType.FileMdModel;
      } else if (NewSelectedIndex == 1) {
        NewFileType = WeItemType.FileHtmlModel;
      } else if (NewSelectedIndex == 2) {
        NewFileType = WeItemType.FileConfigModel;
      } else {
        NewFileType = null;
      }
    }

    private async void cbIsAsync_CheckedChanged(object sender, EventArgs e) {
      if (_targetTypeToCreate == WeItemType.EntityPropertyModel) {
        if (cbIsAsync.Checked == true) {
          using var scope = _serviceScopeFactory.CreateScope();
          var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          var items = await _mediator.Send(new GetItemsByItemTypeQuery((int)WeItemType.EntityClassModel), CancellationToken.None);

          cbItemLookup.Items.Clear();
          foreach (var item in items) {
            if (item != null && item.Description != null) {
              var index = cbItemLookup.Items.Add(item.DisplayText);
              _lookupMap[index] = item.Value.AsInt();
            }
          }
          if (cbItemLookup.Items.Count > 0) {
            cbItemLookup.SelectedIndex = 0;
          }

          cbItemLookup.Left = cbDataType.Left;
          cbItemLookup.Top = cbDataType.Top + cbDataType.Height + 10;
          cbItemLookup.Visible = true;
          lbWhich.Left = lbNewItemName.Left + lbNewItemName.Width - lbWhich.Width;
          lbWhich.Top = cbItemLookup.Top;
          lbWhich.Visible = true;
        } else {
          cbItemLookup.Visible = false;
          lbWhich.Visible = false;
        }
      }
    }
  }
}
