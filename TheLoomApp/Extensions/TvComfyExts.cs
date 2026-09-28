using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLoomApp.Models;
using Weavers.Core.Service;
using Weavers.Core.Handlers.Comfy;
using Weavers.Core.Extensions;
using MediatR;

namespace TheLoomApp.Extensions {
  public static class TvComfyExts {

    public static async Task AddComfyWorkflow(this TreeView _tv, IMediator mediator, string name, string? comfyExportApiForWorkflowFilePath) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null) { throw new ArgumentException("Selected node or item is null"); }
      var newSubItem = await mediator.Send(new AddComfyWorkflowCommand(item.Id, name, comfyExportApiForWorkflowFilePath));
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddComfyWfParam(this TreeView _tv, IMediator mediator, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null) { throw new ArgumentException("Selected node or item is null"); }
      var newSubItem = await mediator.Send(new AddComfyWfParamCommand(item.Id, name));
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddComfyTodo(this TreeView _tv, IMediator mediator, string name, int workflowId) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null) { throw new ArgumentException("Selected node or item is null"); }
      var newSubItem = await mediator.Send(new AddComfyTodoCommand(item.Id, name, workflowId));
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

  }
}
