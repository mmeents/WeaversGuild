using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Chess;

namespace TheLoomApp.Extensions {
  public static class TvChessExts {
    public static async Task AddGameRoomModel(this TreeView _tv, IMediator mediator, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidFolderParent()) { return; }
      var newSubItem = await mediator.Send(new AddGameRoomCommand(item.Id, name));
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddChessGameModel(this TreeView _tv, IMediator mediator, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.GameRoomModel) { return; }
      var newSubItem = await mediator.Send(new AddChessGameCommand(item.Id, name, 0, 0));
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }
  }
}
