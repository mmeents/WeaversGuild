using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Weavers.Core.Handlers.Documentation;
using Weavers.Core.Models;
using Microsoft.Extensions.Logging;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public interface ITodoToolsHandler {
    Task<string> CompletedTodo(int todoId, string todoNote, int? producedItemId);
    Task<string> RejectTodo(int todoId, string reason);

    Task<string> ReviewPass(int todoId, string reviewNotes);
    Task<string> ReviewFail(int todoId, string reviewNotes, string changeRequest);
    Task<string> SetTodoReady(int todoId);
  }

  public class TodoToolsHandler : ITodoToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<TodoToolsHandler> _logger;


    public TodoToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<TodoToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }


    public async Task<string> CompletedTodo(int todoId, string todoNote, int? producedItemId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var helpText = await mediator.Send(new CompleteTodoCommand(todoId, todoNote, producedItemId));
        var opResult = McpOpResult.CreateSuccess(Cx.CmdCompleteTodo, helpText);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error completing todo for Id {todoId}: {ex.Message}";
        _logger.LogError(ex, errorMessage);
        var opResult = McpOpResult.CreateFailure(Cx.CmdCompleteTodo, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> RejectTodo(int todoId, string reason) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var helpText = await mediator.Send(new RejectTodoCommand(todoId, reason));
        var opResult = McpOpResult.CreateSuccess(Cx.CmdRejectTodo, helpText);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error rejecting todo for Id {todoId}: {ex.Message}";
        _logger.LogError(ex, errorMessage);
        var opResult = McpOpResult.CreateFailure(Cx.CmdRejectTodo, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> ReviewPass(int todoId, string reviewNotes) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var helpText = await mediator.Send(new ReviewPassCommand(todoId, reviewNotes));
        var opResult = McpOpResult.CreateSuccess(Cx.CmdReviewPass, helpText);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error reviewing pass for Id {todoId}: {ex.Message}";
        _logger.LogError(ex, errorMessage);
        var opResult = McpOpResult.CreateFailure(Cx.CmdReviewPass, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }
    public async Task<string> ReviewFail(int todoId, string reviewNotes, string changeRequest) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var helpText = await mediator.Send(new ReviewFailCommand(todoId, reviewNotes, changeRequest));
        var opResult = McpOpResult.CreateSuccess(Cx.CmdReviewFail, helpText);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error reviewing fails for Id {todoId}: {ex.Message}";
        _logger.LogError(ex, errorMessage);
        var opResult = McpOpResult.CreateFailure(Cx.CmdReviewFail, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> SetTodoReady(int todoId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var itemDto = await mediator.Send(new SetTodoReadyCommand(todoId));
        if (itemDto == null) {
          var opResultNotFound = McpOpResult.CreateFailure(Cx.CmdSetTodoReady, $"Todo item with Id {todoId} not found.");
          return JsonSerializer.Serialize(opResultNotFound);
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdSetTodoReady, $"Todo item with Id {todoId} is now marked as ready.");
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error setting todo ready for Id {todoId}: {ex.Message}";
        _logger.LogError(ex, errorMessage);
        var opResult = McpOpResult.CreateFailure(Cx.CmdSetTodoReady, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }


  }
}
