using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLoomApp.Models;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Service;

namespace TheLoomApp.Extensions {
  public static class TvLibraryExts {

    public static async Task AddLibrary(this TreeView _tv, IMediator mediator, IAppGraphClassService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddLibrary(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);

      // so like the mcp in the app, when creating a library we also want the extras.
      var newSubSubItem = await graphSrvs.AddDiModel(newSubItem, (string?)null);
      if (newSubSubItem != null) {
        int newId = newSubItem.Id;

        var mediatorProp = newSubSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasMediator);
        if (mediatorProp != null) {
          mediatorProp.Value = "1";
          newSubSubItem = await mediator.UpdateItemProp(newSubSubItem, mediatorProp);
        }

        var dbContextProp = newSubSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasDbContext);
        if (dbContextProp != null) {
          dbContextProp.Value = "1";  // this creates the db context under the new di model.
          newSubSubItem = await mediator.Send(new UpdateItemPropertyCommand(dbContextProp.Id, "1"));
        }

        newSubSubItem = await mediator.Send(new GetItemByIdQuery(newId));
        if (newSubSubItem != null) {
          await _tv.AddNewItem(newSubSubItem);
        }
      }

    }

    public static async Task AddDiModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.LibraryModel) { return; }
      var newSubItem = await graphSrvs.AddDiModel(item, (string?)null);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddNamespaceModel(this TreeView _tv, IAppGraphClassService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (item.ItemTypeId != (int)WeItemType.LibraryModel && item.ItemTypeId != (int)WeItemType.NamespaceModel)) { return; }
      var newSubItem = await graphSrvs.AddNamespaceModel(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassModel(this TreeView _tv, IAppGraphClassService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.NamespaceModel) { return; }
      var newSubItem = await graphSrvs.AddClassModel(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassImportModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.ClassModel) { return; }
      var newSubItem = await graphSrvs.AddClassImportModel(item, (string?)null);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassPropModel(this TreeView _tv, IAppGraphClassService graphSrvs, string name, int? propertyTypeId, int? propertyClassId) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.ClassModel) { return; }
      var newSubItem = await graphSrvs.AddClassPropModel(item, name, propertyTypeId, propertyClassId);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassMethodModel(this TreeView _tv, IAppGraphClassService graphSrvs, string name, bool? isAsync, int? returnTypeId, int? returnClassId) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.ClassModel) { return; }
      var newSubItem = await graphSrvs.AddClassMethodModel(item, name, isAsync, returnTypeId, returnClassId);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddClassMethodParameterModel(this TreeView _tv, IAppGraphClassService graphSrvs, string name, int? paramTypeId, int? paramClassId) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.ClassMethodModel) { return; }
      var newSubItem = await graphSrvs.AddClassMethodParam(item, name, paramTypeId, paramClassId);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddEntityClassModel(this TreeView _tv, IAppGraphClassService graphSrvs, string? className, string? dbTableName) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (item.ItemTypeId != (int)WeItemType.NamespaceModel && item.ItemTypeId != (int)WeItemType.LibraryModel)) { return; }
      var newSubItem = await graphSrvs.AddEntityClassModel(item, className, dbTableName);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddEntityClassImportModel(this TreeView _tv, IAppGraphClassService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.EntityClassModel) { return; }
      var newSubItem = await graphSrvs.AddEntityClassImportModel(item, (string?)null);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddEntityPropertyModel(this TreeView _tv, IAppGraphClassService graphSrvs, string propName, int? propertyTypeId, bool isNav, int? navEntityClassId) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.EntityClassModel) { return; }
      var newSubItem = await graphSrvs.AddEntityPropertyModel(item, propName, propertyTypeId, isNav, navEntityClassId);
      if (newSubItem == null) { return; }
      _tv.ReExpandSelectNode(newSubItem.Id);
    }


  }
}
