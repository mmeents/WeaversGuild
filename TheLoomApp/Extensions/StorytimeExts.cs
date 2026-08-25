using MediatR;
using Microsoft.AspNetCore.Http;
using TheLoomApp.Models;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Service;
using Weavers.Core.Handlers.Storytime;

namespace TheLoomApp.Extensions {
  public static class StorytimeExts {

    public static async Task AddRealm(this TreeView _tv, IMediator _mediator, string name, string description, string tone) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || ((item.ItemTypeId != (int)WeItemType.OrganizationModel)
          && (item.ItemTypeId != (int)WeItemType.ProjectFolderModel) 
          && (item.ItemTypeId != (int)WeItemType.RelativeFolderModel))
      ){
        throw new InvalidOperationException("Invalid node selected for adding Storytime Realm");
      }
      var newSubItem = await _mediator.Send(new AddRealmCommand(item.Id, name, description, tone));
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }


    public static async Task AddStory(this TreeView _tv, IMediator _mediator,
      int todoId, string name, string description, int povTypeId, int sceneCount
     ) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || (item.ItemTypeId != (int)WeItemType.RealmModel)
      ) {
        throw new InvalidOperationException("Invalid node selected for adding Storytime Story");
      }      
      var newSubItem = await _mediator.Send(new AddStoryCommand(item.Id, name, description, povTypeId, sceneCount, todoId));
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddScene(this TreeView _tv, IMediator _mediator,
      int todoId, string name, string description, int povTypeId, string entryState, string exitState) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || (item.ItemTypeId != (int)WeItemType.StoryModel)
      ) {
        throw new InvalidOperationException("Invalid node selected for adding Storytime Scene");
      }
      var newSubItem = await _mediator.Send(new AddSceneCommand(item.Id, name, description, entryState, exitState, todoId));
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddBeat(this TreeView _tv, IMediator _mediator,
      int todoId, string name, string description) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || (item.ItemTypeId != (int)WeItemType.SceneModel)
      ) {
        throw new InvalidOperationException("Invalid node selected for adding Storytime Beat");
      }
      var newSubItem = await _mediator.Send(new AddBeatCommand(item.Id, name, description, todoId));
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }
    public static async Task AddCharacter(this TreeView _tv, IMediator _mediator, string name, string description) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || (item.ItemTypeId != (int)WeItemType.SceneModel)
      ) {
        throw new InvalidOperationException("Invalid node selected for adding Storytime Character");
      }
      var newSubItem = await _mediator.Send(new AddCharacterCommand(item.Id, name, description));
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddCallSheet(this TreeView _tv, IMediator _mediator, int todoId, string name, string description) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || (item.ItemTypeId != (int)WeItemType.BeatModel)
      ) {
        throw new InvalidOperationException("Invalid node selected for adding Storytime Call Sheet. Needs to be a BeatModel.");
      }
      var newSubItem = await _mediator.Send(new AddCallSheetCommand(item.Id, name, description, todoId));
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddPerformance(this TreeView _tv, IMediator _mediator, string name, string description) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || (item.ItemTypeId != (int)WeItemType.SceneModel)
      ) {
        throw new InvalidOperationException("Invalid node selected for adding Storytime Performance");
      }
      var newSubItem = await _mediator.Send(new AddPerformanceCommand(item.Id, name));
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

    public static async Task AddObserved(this TreeView _tv, IMediator _mediator, int todoId, string name, string description) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || (item.ItemTypeId != (int)WeItemType.PerformanceModel)
      ) {
        throw new InvalidOperationException("Invalid node selected for adding Storytime Observed");
      }
      var newSubItem = await _mediator.Send(new AddObservationCommand(item.Id, name, description, todoId));
      if (newSubItem == null) { return; }
      _tv.AddNewItem(newSubItem);
    }

  }
}
