using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Service;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace TheLoomApp.Extensions {
  public static class AppGraphExts {
    public static async Task AddProjectRoot(this TreeView _tv, IAppGraphFileService graphSrvs, string defaultFolder) {
      var newItem = await graphSrvs.AddProjectRoot((string?)null, defaultFolder);
      if (newItem == null) return;
      var idx = _tv.Nodes.Add(newItem.ToItemNode());
      _tv.SelectedNode = _tv.Nodes[idx];
    }

    public static async Task AddSubFolder(this TreeView _tv, IAppGraphFileService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddSubFolder(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);      
    }

    public static async Task AddSolution(this TreeView _tv, IAppGraphFileService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddSolution(item, (string?)null);

      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
      
    }

    public static async Task AddSolutionImport(this TreeView _tv, IAppGraphFileService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.SolutionModel) { return; }
      var newSubItem = await graphSrvs.AddSolutionImport(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);      
    }

    public static async Task AddFile(this TreeView _tv, IAppGraphFileService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddFile(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);      
    }

    public static async Task AddLibrary(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddLibrary(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddDiModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.LibraryModel) { return; }
      var newSubItem = await graphSrvs.AddDiModel(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddNamespaceModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (item.ItemTypeId != (int)WeItemType.LibraryModel && item.ItemTypeId != (int)WeItemType.NamespaceModel)) { return; }
      var newSubItem = await graphSrvs.AddNamespaceModel(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.NamespaceModel) { return; }
      var newSubItem = await graphSrvs.AddClassModel(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassImportModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.ClassModel) { return; }
      var newSubItem = await graphSrvs.AddClassImportModel(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassPropModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.ClassModel) { return; }
      var newSubItem = await graphSrvs.AddClassPropModel(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassMethodModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.ClassModel) { return; }
      var newSubItem = await graphSrvs.AddClassMethodModel(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassMethodParamModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.ClassMethodModel) { return; }
      var newSubItem = await graphSrvs.AddClassMethodParam(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddEntityClassModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (item.ItemTypeId != (int)WeItemType.NamespaceModel && item.ItemTypeId != (int)WeItemType.LibraryModel)) { return; }
      var newSubItem = await graphSrvs.AddEntityClassModel(item, (string?)null);
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddEntityClassImportModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
        ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
        var item = _selectedNode?.Item;
        if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.EntityClassModel) { return; }
        var newSubItem = await graphSrvs.AddEntityClassImportModel(item, (string?)null);
        if (newSubItem == null) { return; }
        _tv.AddNewItem(newSubItem);
    }

    public static async Task AddEntityPropertyModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
        ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
        var item = _selectedNode?.Item;
        if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.EntityClassModel) { return; }
        var newSubItem = await graphSrvs.AddEntityPropertyModel(item, (string?)null);
        if (newSubItem == null) { return; }        
        _tv.ReExpandSelectNode(newSubItem.Id);
    }


    private static void ReExpandSelectNode(this TreeView _tv, int itemId) {
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


    public static void AddNewItem(this TreeView _tv, ItemDto item) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      if (item == null || _selectedNode == null) { return; }
      var newParentRelation = item.IncomingRelations
        .FirstOrDefault(r => r.RelatedItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) { return; }
      var newNode = newParentRelation.ToItemNode(item);
      var idx = _selectedNode.Nodes.Add(newNode);
      if (_selectedNode.Nodes.Count > 0 && idx >= 0 && idx < _selectedNode.Nodes.Count) {
        _tv.SelectedNode = _selectedNode.Nodes[idx];
      }
    }


// below is end of namespace and end of class leave it.
  }
}
