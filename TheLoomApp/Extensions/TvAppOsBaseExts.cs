using MediatR;
using System.Xml.Linq;
using TheLoomApp.Models;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Chess;
using Weavers.Core.Handlers.Cache;
using Weavers.Core.Models;
using Weavers.Core.Service;
using Weavers.Core.Handlers.Pattern;

namespace TheLoomApp.Extensions {
  public static class TvAppOsBaseExts {

    public static async Task DuplicateItem(this TreeView _tv, IMediator _mediator) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null) { return; }
      var newSubItem = await _mediator.Send(new DuplicateItemCommand(item.Id));
      if (newSubItem == null) { return; }
      _tv.SelectedNode = _selectedNode.Parent;
      _selectedNode = _selectedNode.Parent as ItemNode;
      await _tv.AddNewItem(newSubItem);
    }
      

    public static async Task AddOrgFolder(this TreeView _tv, IAppGraphOrgService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddOrgFolder(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddOrgFile(this TreeView _tv, IAppGraphOrgService graphSrvs, string name, string? content = null) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddOrgFile(item, name, content);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }    

    public static async Task AddProjectRoot(this TreeView _tv, IAppGraphFileService graphSrvs, string name, string defaultFolder) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (item == null || item.ItemTypeId != (int)WeItemType.OrganizationModel) { return; }
      var newItem = await graphSrvs.AddProjectRoot(name, defaultFolder);
      if (newItem == null) return;
      await _tv.AddNewItem(newItem);
    }

    public static async Task AddSubFolder(this TreeView _tv, IAppGraphFileService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddSubFolder(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);      
    }

    public static async Task AddSolution(this TreeView _tv, IAppGraphFileService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddSolution(item, name);

      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
      
    }

    public static async Task AddSolutionImport(this TreeView _tv, IAppGraphFileService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.SolutionModel) { return; }
      var newSubItem = await graphSrvs.AddSolutionImport(item, (string?)null);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);      
    }

    public static async Task AddMdFile(this TreeView _tv, IAppGraphFileService graphSrvs, string name, string? content = null) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddMdFile(item, name, content);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);      
    }

    public static async Task AddHtmlFile(this TreeView _tv, IAppGraphFileService graphSrvs, string name, string? content = null) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddHtmlFile(item, name, content);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }
    public static async Task AddConfigFile(this TreeView _tv, IAppGraphFileService graphSrvs, string name, string? content = null) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddConfigFile(item, name, content);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }
    
    public static void ReExpandSelectNode(this TreeView _tv, int itemId) {
      if (_tv.SelectedNode == null) { return; }      
      ItemNode? selectedNode = _tv.SelectedNode as ItemNode;
      if (selectedNode == null) return; 
      if (selectedNode.IsExpanded) {
        selectedNode.Collapse();
      }
      selectedNode?.Expand();
      if (selectedNode != null) {
        foreach (ItemNode child in selectedNode.Nodes) {
          if (child.Item != null && child.Item.Id == itemId) {
            _tv.SelectedNode = child;
            break;
          }
        }
      }
    }


    public static async Task AddNewItem(this TreeView _tv, ItemDto item) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      if (item == null || _selectedNode == null) { return; }
      var selectedNodeId = _selectedNode.Item?.Id ?? 0;
      var newParentRelation = item.IncomingRelations
        .FirstOrDefault(r => r.RelatedItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) { return; }
      var newNode = newParentRelation.ToItemNode(item);
      if (item.Relations.Count > 0) {
        newNode.Nodes.Add(new ItemNode());
      }
      var idx = _selectedNode.Nodes.Add(newNode);
      if (_selectedNode.Nodes.Count > 0 && idx >= 0 && idx < _selectedNode.Nodes.Count) {
        _tv.SelectedNode = _selectedNode.Nodes[idx];        
      }
      
      int[] itemIds = new int[] { item.Id, selectedNodeId };
      await RemoveTool.RemoveItemsFromCache(itemIds);
    }


// below is end of namespace and end of class leave it.
  }
}
