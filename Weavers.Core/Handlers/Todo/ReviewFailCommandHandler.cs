using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Todo {
  public record ReviewFailCommand(int TodoId, string ReviewNotes, string ChangeRequest) : IRequest<ReviewFailCmdResult>;

  public class ReviewFailCmdResult {
    public bool Success { get; set; }
    public string Message { get; set; } = "";
  }

  public class ReviewFailCommandHandler : IRequestHandler<ReviewFailCommand, ReviewFailCmdResult> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public ReviewFailCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<ReviewFailCmdResult> Handle(ReviewFailCommand request, CancellationToken cancellationToken) {
      var result = new ReviewFailCmdResult();

      // 1 load verify the todo item, ensure it's in progress, and get the parent desk and onPushbackDesk.
      var todoItem = await _context.GetItemDtoById(request.TodoId, cancellationToken);
      if (todoItem == null) return result.CreateFailure("Todo item not found.");

      var todoStatusProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
      if (todoStatusProp == null || todoStatusProp.Value != ((int)WeItemType.TodoInProgress).ToString()) {
        return result.CreateFailure("Todo item is not in progress.");
      }

      var parentId = todoItem.IncomingRelations.FirstOrDefault(r => r.ItemTypeId == (int)WeItemType.DeskModel)?.ItemId;
      if (parentId == null) return result.CreateFailure("Parent item not found for the todo item.");

      var parentDesk = await _context.GetItemDtoById(parentId.Value, cancellationToken);
      if (parentDesk == null) return result.CreateFailure("Parent desk not found for the todo item.");
      if (parentDesk.ItemTypeId != (int)WeItemType.DeskModel) return result.CreateFailure("Parent item is not a desk.");

      var currentDeskTodo = parentDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItCurrentTodo && p.Value == todoItem.Id.ToString());
      if (currentDeskTodo == null) {
        return result.CreateFailure("The parent desk does not have a current todo property matching the todo item.");
      }

      var OnPushbackProp = parentDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItOnPushbackSendTo);
      if (OnPushbackProp == null) return result.CreateFailure("OnPushbackSendTo property not found on the parent desk.");

      var onPushbackDeskId = int.TryParse(OnPushbackProp.Value, out var parsedId) ? parsedId : (int?)null;
      if (onPushbackDeskId == null) return result.CreateFailure("Invalid OnPushbackSendTo property value.");

      var onPushbackDesk = await _context.GetItemDtoById(onPushbackDeskId.Value, cancellationToken);
      if (onPushbackDesk == null) return result.CreateFailure("OnPushbackSendTo desk not found.");

      // update the todo item save the note, status at end.
      var todoCloseReasonProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCloseReason);
      if (todoCloseReasonProp != null) {
        todoCloseReasonProp.Value = request.ReviewNotes;
        await todoCloseReasonProp.SaveProp(todoItem, _mediator);
      }

      // create new todo on the onPushbackDesk with the same note and link it to the rejected todo item.
      var name = "";
      if (todoItem.Name != null) {
        name = todoItem.Name + $" fromId:{todoItem.Id}";
      } else {
        var nextRank = await _mediator.Send(new GetNextItemRankQuery(onPushbackDesk.Id)) + 1;
        name = $"Todo {nextRank} fromId:{todoItem.Id}";
      }
      var newTodoItem = await _mediator.Send(
        new CreateRelatedItemCommand(onPushbackDesk.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.TodoModel, name, "", "{}"));
      if (newTodoItem == null) {
        return result.CreateFailure("Failed to create new todo item on the OnPushbackSendTo desk.");
      }

      // find inprogress TodoAttempt relation and update it's ItContinueTodo property.
      string runInProgressType = WeItemType.RunInProgress.AsIntString();
      ItemDto? inProgressTodoAttempt = null;
      foreach (var rel in todoItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.TodoAttemptModel)) {
        var attemptId = rel?.RelatedItemId ?? 0;
        if (attemptId > 0) {
          var attemptItem = await _context.GetItemDtoById(attemptId, cancellationToken);
          if (attemptItem != null) {
            var attemptStatusStr = attemptItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus)?.Value;
            if (attemptStatusStr != null && attemptStatusStr == runInProgressType) {
              inProgressTodoAttempt = attemptItem; // found.
              break;
            }
          }
        }
      }

      if (inProgressTodoAttempt != null) {
        var itContinueTodoProp = inProgressTodoAttempt.Properties.FirstOrDefault(p => p.Name == Cx.ItContinueTodo);
        if (itContinueTodoProp != null) {
          itContinueTodoProp.Value = newTodoItem.Id.ToString();
          await itContinueTodoProp.SaveProp(inProgressTodoAttempt, _mediator);
        }

        var newTodoPromptProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPromptTemplate);
        if (newTodoPromptProp != null) {
          var originalPrompt = inProgressTodoAttempt.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPrompt)?.Value ?? "";
          newTodoPromptProp.Value = "Original request: " + originalPrompt + Environment.NewLine +
            "Review Notes: " + request.ReviewNotes+Environment.NewLine+
            "Change Request: " + request.ChangeRequest;
          await newTodoPromptProp.SaveProp(newTodoItem, _mediator);
        }
      } else {
        var newTodoPromptProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPromptTemplate);
        if (newTodoPromptProp != null) {
          var originalPrompt = await todoItem.UserPrompt(_mediator, CancellationToken.None);
          newTodoPromptProp.Value = "Original request: " + originalPrompt + Environment.NewLine +
            "Review Notes: " + request.ReviewNotes + request.ReviewNotes + Environment.NewLine +
            "Change Request: " + request.ChangeRequest;
          await newTodoPromptProp.SaveProp(newTodoItem, _mediator);
        }
      }

      // finish filling out properties on new todo item.
      var newTodoRefItemIdProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
      if (newTodoRefItemIdProp != null) {
        var todoRefItemProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
        if (todoRefItemProp != null) {
          newTodoRefItemIdProp.Value = todoRefItemProp.Value;
          newTodoRefItemIdProp.ReferenceItemTypeId = todoRefItemProp.ReferenceItemTypeId;
        } else {
          newTodoRefItemIdProp.ReferenceItemTypeId = (int)WeItemType.TodoModel;
          newTodoRefItemIdProp.Value = todoItem.Id.ToString();
        }
        await newTodoRefItemIdProp.SaveProp(newTodoItem, _mediator);
      }

      var itFromTodoProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFromTodo);
      if (itFromTodoProp != null) {
        itFromTodoProp.Value = todoItem.Id.ToString();
        await itFromTodoProp.SaveProp(newTodoItem, _mediator);
      }

      // finally, update the original todo item status to completed.      
      if (todoStatusProp != null) {
        todoStatusProp.Value = ((int)WeItemType.TodoAbortedPushBack).ToString();
        await todoStatusProp.SaveProp(todoItem, _mediator);
      }

      var currentDeskTodoProp = parentDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItCurrentTodo && p.Value == todoItem.Id.ToString());
      if (currentDeskTodoProp != null) {
        currentDeskTodoProp.Value = ""; // clear current todo on the parent desk.
        await currentDeskTodoProp.SaveProp(parentDesk, _mediator);
      }

      return result.CreateSuccess(); 
    }
  }

  public static class ReviewFailCmdResultExts {
    public static ReviewFailCmdResult CreateSuccess(this ReviewFailCmdResult result) {
      result.Success = true;
      result.Message = "Review failed op completed successfully.";
      return result;
    }

    public static ReviewFailCmdResult CreateFailure(this ReviewFailCmdResult result, string errorMessage) {
      result.Success = false;
      result.Message = errorMessage;
      return result;
    }
  }

}
