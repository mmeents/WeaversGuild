using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Handlers.Todo {

  public record FailTodoCommand(int TodoId, string Reason) : IRequest<FailTodoCmdResult>;
  public class FailTodoCmdResult {   
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int UpdatedTodoId { get; set; }
  }

  public class FailTodoCommandHandler : IRequestHandler<FailTodoCommand, FailTodoCmdResult> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public FailTodoCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<FailTodoCmdResult> Handle(FailTodoCommand request, CancellationToken cancellationToken) {
      var result = new FailTodoCmdResult();

      // 1 load verify the todo item, ensure it's in progress, and get the parent desk and onSuccessDesk.
      var todoItem = await _context.GetItemDtoById(request.TodoId, cancellationToken);
      if (todoItem == null) return result.CreateFailure("Todo item not found.");
      result.UpdatedTodoId = todoItem.Id;

      if (todoItem.IsTerminal()) {
        await _mediator.AppendCloseReason(todoItem, $"Fail called on already-terminal todo. Reason: {request.Reason}");
        result.Success = true; // already terminal == nothing left to do
        result.Message = "Todo already terminal; reason recorded, no successor minted.";
        return result;
      }

      try {
        // fetch parent desk ONCE, best-effort (null is tolerated everywhere below)
        var theDesk = await _context.TryGetParentDesk(todoItem, cancellationToken);

        // TERMINATE FIRST — inverted vs Complete.
        // For success you flip status LAST (don't advance unless the mint is clean).
        // For fail the priority reverses: ending the broken todo and freeing the desk is
        // non-negotiable and must not be held hostage to minting a successor.
        await _mediator.TerminateTodo(todoItem, WeItemType.TodoFailedForward, request.Reason, "Fail Reason");
        await _mediator.ReleaseDeskIfParked(theDesk, todoItem);

        // resolve target — NEVER hard-fail, fall through to the log desk on any miss
        var targetDesk = await _context.ResolveSendToDeskOrLog(theDesk, Cx.ItOnFailSendTo, cancellationToken);
        if (targetDesk == null) {
          // truly nowhere to land (no OnFail, no log desk). Todo is already terminal + desk freed,
          // so the floor is unblocked; we just couldn't mint a successor. Partial, but not stuck.
          result.Success = true;
          result.Message = "Todo failed and desk released, but no OnFail/Log desk found for successor.";
          return result;
        }

        var newTodo = await _mediator.MintForwardTodo(_context, todoItem, targetDesk, request.Reason, "Fail details", cancellationToken);
        result.Success = true;
        result.Message = newTodo == null
          ? "Todo failed and desk released, but successor mint failed."
          : $"Todo failed forward to {targetDesk.Id} {targetDesk.Name}, new todo {newTodo.Id}.";
        return result;
      } catch (Exception ex) {
        await _mediator.TryEmergencyTerminate(_context, todoItem, request.Reason, ex, cancellationToken);
        return result.CreateFailure($"Fail handler threw; emergency-terminated todo {todoItem.Id}: {ex.Message}");
      }

    }
  }

  public static class FailTodoCommandHandlerExtensions {
    public static FailTodoCmdResult CreateFailure(this FailTodoCmdResult result, string message) {
      result.Success = false;
      result.Message = message;
      return result;
    }
  }

}
