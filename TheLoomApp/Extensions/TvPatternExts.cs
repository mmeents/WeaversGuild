using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Pattern;

namespace TheLoomApp.Extensions {
  public static class TvPatternExts {
    public static async Task AddPattern(this TreeView _tv, IMediator mediator, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (
        item.ItemTypeId != (int)WeItemType.OrganizationModel && item.ItemTypeId != (int)WeItemType.ProjectFolderModel && item.ItemTypeId != (int)WeItemType.RelativeFolderModel)
      ) { return; }
      var added = await mediator.Send(new AddPatternCommand(item.Id, name));
    }

    public static async Task AddPatDimension(this TreeView _tv, IMediator mediator, string name, string commaDelimOptions) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (
        item.ItemTypeId != (int)WeItemType.PatternModel)
      ) { return; }
      var added = await mediator.Send(new AddPatDimensionCommand(item.Id, name, commaDelimOptions));
    }

    public static async Task AddPatDimOption(this TreeView _tv, IMediator mediator, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (
        item.ItemTypeId != (int)WeItemType.PatternDimensionModel)
      ) { return; }
      var added = await mediator.Send(new AddPatDimOptionCommand(item.Id, name));
    }

    public static async Task GetNextDraw(this TreeView _tv, IMediator mediator, int? todoId) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (
        item.ItemTypeId != (int)WeItemType.PatternModel)
      ) { return; }
      var added = await mediator.Send(new GetNextDrawCommand(item.Id, todoId));
    }
  }
}
