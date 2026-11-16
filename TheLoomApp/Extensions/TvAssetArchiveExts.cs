using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.AssetArchive;


namespace TheLoomApp.Extensions {
  public static class TvAssetArchiveExts {

    public static async Task AddAssetArchiveModel(this TreeView _tv, IMediator mediator, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await mediator.Send(new AddAssetArchiveCommand(item.Id, name));
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddReplicaPolicyModel(this TreeView _tv, IMediator mediator, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null) { return; }
      var newSubItem = await mediator.Send(new AddReplicaPolicyCommand(item.Id, name));
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddOffloadPolicyModel(this TreeView _tv, IMediator mediator, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null) { return; }
      var newSubItem = await mediator.Send(new AddOffloadPolicyCommand(item.Id, name));
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

  }
}
