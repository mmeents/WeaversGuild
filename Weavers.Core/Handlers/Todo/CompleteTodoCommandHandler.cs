using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Todo {
  public record CompleteTodoCommand(int TodoId, string TodoNote, int? producedItemId) : IRequest<CompleteTodoCmdResult?>;

  public class CompleteTodoCmdResult {
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public ItemDto? UpdatedTodo { get; set; }
  }


  public class CompleteTodoCommandHandler {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public CompleteTodoCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<CompleteTodoCmdResult?> Handle(CompleteTodoCommand request, CancellationToken cancellationToken) {
      var result = new CompleteTodoCmdResult();

      // 1 load verify the todo item, ensure it's in progress, and get the parent desk and onSuccessDesk.
      var todoItem = await _context.GetItemDtoById(request.TodoId, cancellationToken);
      if (todoItem == null) return result.CreateFailure("Todo item not found.");

      var todoStatusProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
      if (todoStatusProp == null || 
        (todoStatusProp.Value != ((int)WeItemType.TodoNotStarted).ToString() 
          && todoStatusProp.Value != ((int)WeItemType.TodoInProgress).ToString())) { 
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

      var OnSuccessProp = parentDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItOnSuccessSendTo);
      if (OnSuccessProp == null) return result.CreateFailure("OnSuccessSendTo property not found on the parent desk.");

      var onSuccessDeskId = int.TryParse(OnSuccessProp.Value, out var parsedId) ? parsedId : (int?)null;
      if (onSuccessDeskId == null) return result.CreateFailure("Invalid OnSuccessSendTo property value.");

      var onSuccessDesk = await _context.GetItemDtoById(onSuccessDeskId.Value, cancellationToken);
      if (onSuccessDesk == null) return result.CreateFailure("OnSuccessSendTo desk not found.");

      int producedItemTypeId = 0;
      if (request.producedItemId != null) {
        var producedItem = await _context.GetItemDtoById(request.producedItemId.Value, cancellationToken);
        if (producedItem == null) return result.CreateFailure("Produced item not found.");
        producedItemTypeId = producedItem.ItemTypeId;
      }

      // update the todo item save the note, status at end.
      var todoCloseReasonProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCloseReason);
      if (todoCloseReasonProp != null) {
        todoCloseReasonProp.Value = request.TodoNote;
        await todoCloseReasonProp.SaveProp(todoItem, _mediator);
      }          

      // create new todo on the onSuccessDesk with the same note and link it to the completed todo item.
      var nextRank = await _mediator.Send(new GetNextItemRankQuery(onSuccessDesk.Id)) + 1;
      var name = todoItem.Name == null ? $"Todo {nextRank}" : todoItem.Name;
      var newTodoItem = await _mediator.Send(
        new CreateRelatedItemCommand(onSuccessDesk.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.TodoModel, name, "", "{}"));
      if (newTodoItem == null) {
        return result.CreateFailure("Failed to create new todo item on the OnSuccessSendTo desk.");
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
            (request.producedItemId != null ? "Produced item Id: " + request.producedItemId + Environment.NewLine : "") +
            "Notes taken: " + request.TodoNote;
          await newTodoPromptProp.SaveProp(newTodoItem, _mediator);
        }
      } else {
        var newTodoPromptProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPromptTemplate);
        if (newTodoPromptProp != null) {
          var originalPrompt = await todoItem.UserPrompt(_mediator, CancellationToken.None);
          newTodoPromptProp.Value = "Original request: " + originalPrompt + Environment.NewLine +
            (request.producedItemId != null ? "Produced item Id: " + request.producedItemId + Environment.NewLine : "") +
            "Notes taken: " + request.TodoNote;
          await newTodoPromptProp.SaveProp(newTodoItem, _mediator);
        }
      }

      // finish filling out properties on new todo item.
      var newTodoStatusProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
      if (newTodoStatusProp != null) {
        newTodoStatusProp.Value = ((int)WeItemType.TodoNotStarted).ToString();
        await newTodoStatusProp.SaveProp(newTodoItem, _mediator);
      }

      var newTodoRefItemIdProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
      if (newTodoRefItemIdProp != null) {
        if (request.producedItemId != null) {
          newTodoRefItemIdProp.ReferenceItemTypeId = producedItemTypeId;
          newTodoRefItemIdProp.Value = request.producedItemId.ToString();
        } else {
          var todoRefItemProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
          if (todoRefItemProp != null) {
            newTodoRefItemIdProp.Value = todoRefItemProp.Value;
            newTodoRefItemIdProp.ReferenceItemTypeId = todoRefItemProp.ReferenceItemTypeId;
          } else {
            newTodoRefItemIdProp.ReferenceItemTypeId = (int)WeItemType.TodoModel;
            newTodoRefItemIdProp.Value = todoItem.Id.ToString();
          }          
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
        todoStatusProp.Value = ((int)WeItemType.TodoCompleteForward).ToString();
        await todoStatusProp.SaveProp(todoItem, _mediator);
      }

      return result.CreateSuccess(todoItem);

    }
  }



  public static class CompleteTodoCmdResultExts {
    public static CompleteTodoCmdResult CreateSuccess(this CompleteTodoCmdResult result, ItemDto updatedTodo) {
      result.Success = true;
      result.Message = "Todo item completed successfully.";
      result.UpdatedTodo = updatedTodo;
      return result;
    }

    public static CompleteTodoCmdResult CreateFailure(this CompleteTodoCmdResult result, string errorMessage) {
      result.Success = false;
      result.Message = errorMessage;
      result.UpdatedTodo = null;
      return result;
    }
  }


}
