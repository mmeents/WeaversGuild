using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Service;

namespace TheLoomApp.Extensions {
  public static class TvWorkgroupExts {
    public static async Task AddOrgWorkGroup(this TreeView _tv, IAppGraphOrgService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null ||
        (item.ItemTypeId != (int)WeItemType.OrganizationModel) && (item.ItemTypeId != (int)WeItemType.WorkGroupModel)) {
        return;
      }
      var newSubItem = await graphSrvs.AddOrgWorkGroup(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddOrgDeskRole(this TreeView _tv, IAppGraphOrgService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.OrgDeskRolesModel) { return; }
      var newSubItem = await graphSrvs.AddOrgDeskRole(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddDesk(this TreeView _tv, IAppGraphOrgService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.WorkGroupModel) { return; }
      var newSubItem = await graphSrvs.AddOrgDesk(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }
    public static async Task AddDeskTodo(this TreeView _tv, IAppGraphOrgService graphSrvs, string? name, int? refId = null, string? promptTemplate = null) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.DeskModel) { return; }
      var newSubItem = await graphSrvs.AddDeskTodo(item, name, refId, promptTemplate);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddDigitalOperator(this TreeView _tv, IAppGraphOrgService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.DigitalOperatorPoolModel) { return; }
      var newSubItem = await graphSrvs.AddDigitalOperator(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }
  }
}
