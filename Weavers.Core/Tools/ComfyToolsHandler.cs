using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Comfy;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.ItemTypes;
using Weavers.Core.Models;
using MediatR;
using Weavers.Core.Handlers.ItemSummaries;


namespace Weavers.Core.Tools {

  public interface IComfyToolsHandler {
    Task<string> ListComfyWorkflows();
    Task<string> AddComfyTodo(int parentId, string name, int workflowId);    

  }

  public class ComfyToolsHandler : IComfyToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ComfyToolsHandler> _logger;

    public ComfyToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<ComfyToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    public async Task<string> ListComfyWorkflows() {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var workflows = await mediator.Send(new GetItemsByItemTypeQuery((int)WeItemType.ComfyWorkflowTemplate));
        var opResult = McpOpResult.CreateSuccess(Cx.CmdListComfyWorkflows, workflows);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdListComfyWorkflows, 0, $"Failed to list Comfy Workflows {ex.Message}");
      }
    }

    public async Task<string> AddComfyTodo(int parentId, string name, int workflowId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var newItem = await mediator.Send(new AddComfyTodoCommand(parentId, name, workflowId));
        if (newItem == null) {
          return _logger.DefaultAddEmptyMessage(Cx.CmdAddComfyTodo, 0);
        }
        var returnObj = await mediator.Send(new GetSummaryByIdQuery(newItem.Id, true, true));
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddComfyTodo, returnObj);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddComfyTodo, 0, $"Failed to add Comfy Todo {name} {ex.Message}");
      }
    }



  }
}
