using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Templates;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {

  public static class TodoExts {
    public static bool IsTerminal(this ItemDto todo) {
      var s = todo.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus)?.Value;
      return s == ((int)WeItemType.TodoCompleteForward).ToString()
          || s == ((int)WeItemType.TodoAbortedPushBack).ToString()
          || s == ((int)WeItemType.TodoFailedForward).ToString();
    }

    public static async Task<string> UserPrompt(this ItemDto todo, IMediator _mediator, CancellationToken cancellationToken) {
      var promptProp = todo.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPromptTemplate);
      if (promptProp != null && !string.IsNullOrEmpty(promptProp.Value)) {
        var promptTemplate = promptProp.Value;
        var userPrompt = await _mediator.Send(new RenderFieldTemplate(promptProp), cancellationToken);
        return userPrompt ?? promptTemplate; // fallback to unrendered template if rendering fails
      }
      return "";
    }

    public static async Task<string> SystemPrompt(this ItemDto todo, IMediator _mediator, CancellationToken cancellationToken) {
      var promptProp = todo.Properties.FirstOrDefault(p => p.Name == Cx.ItSystemPromptTemplate);
      if (promptProp != null && !string.IsNullOrEmpty(promptProp.Value)) {
        var promptTemplate = promptProp.Value;
        var systemPrompt = await _mediator.Send(new RenderFieldTemplate(promptProp), cancellationToken);
        return systemPrompt ?? promptTemplate; // fallback to unrendered template if rendering fails
      }
      return "";
    }

    public static async Task<ItemDto?> TryGetParentDesk(this FabricDbContext _context, ItemDto todo, CancellationToken ct) {
      var parentId = todo.IncomingRelations.FirstOrDefault(r => r.ItemTypeId == (int)WeItemType.DeskModel)?.ItemId;   
      return parentId == null ? null : await _context.GetItemDtoById(parentId.Value, ct);
    }

    // Close reason holds the operator's WHY only. Provenance (which desk/todo it continued to)
    // lives on FromTodo + the attempt's ContinueTodo — not smeared into CloseReason. Avoids the
    // double-write that clobbered the note in the Complete draft.
    public static async Task TerminateTodo(this IMediator _mediator, ItemDto todo, WeItemType terminalStatus, string reason, string reasonLabel) {
      await _mediator.AppendCloseReason(todo, $"{reasonLabel}: " + reason);
      var statusProp = todo.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus)
        ?? throw new InvalidOperationException($"Todo {todo.Id} has no Status property; cannot terminate.");
      statusProp.Value = ((int)terminalStatus).ToString();
      await statusProp.SaveProp(todo, _mediator);
    }

    public static async Task AppendCloseReason(this IMediator _mediator, ItemDto todo, string line) {
      var closeProp = todo.Properties.FirstOrDefault(p => p.Name == Cx.ItCloseReason);
      if (closeProp == null) return;
      closeProp.Value = string.IsNullOrEmpty(closeProp.Value) ? line : closeProp.Value + Environment.NewLine + line;
      await closeProp.SaveProp(todo, _mediator);
    }

    public static async Task ReleaseDeskIfParked(this IMediator _mediator, ItemDto? desk, ItemDto todo) {
      if (desk == null) return;
      var currentProp = desk?.Properties.FirstOrDefault(p => p.Name == Cx.ItCurrentTodo);
      if (currentProp != null && currentProp.Value == todo.Id.ToString()) {
        currentProp.Value = "";
        await currentProp.SaveProp(desk, _mediator);
      }
    }

    public static async Task<ItemDto?> ResolveSendToDeskOrLog(this FabricDbContext _context, ItemDto? parentDesk, string sendToPropName, CancellationToken ct) {
      int? targetId = null;
      var sendToProp = parentDesk?.Properties.FirstOrDefault(p => p.Name == sendToPropName);
      if (sendToProp != null && int.TryParse(sendToProp.Value, out var parsed)) targetId = parsed;

      // fall through to the single log desk on ANY miss above (missing parent, missing prop, unparseable)
      if (targetId == null)
        targetId = _context.Items.FirstOrDefault(i => i.ItemTypeId == (int)WeItemType.DeskLogModel)?.Id;
      // SEAM: when multi-log-desk lands, pick the org-local one here instead of FirstOrDefault.

      return targetId == null ? null : await _context.GetItemDtoById(targetId.Value, ct);
    }

    public static async Task<ItemDto?> MintForwardTodo(this IMediator _mediator, FabricDbContext _context,
        ItemDto fromTodo, ItemDto targetDesk, string reason, string reasonLabel, CancellationToken ct) {
      var nextRank = await _mediator.Send(new GetNextItemRankQuery(targetDesk.Id)) + 1;
      var name = fromTodo.Name ?? $"Todo {nextRank}";
      var newTodo = await _mediator.Send(new CreateRelatedItemCommand(
        targetDesk.Id, (int)WeRelationTypes.Contains, (int)WeItemType.TodoModel, name, "", "{}"));
      if (newTodo == null) return null;

      await _mediator.SetProperty(newTodo, Cx.ItStatus, ((int)WeItemType.TodoNotStarted).ToString());

      // RefItem = the original WORK TARGET carried forward (not the previous todo).
      // Fallback to the previous todo only if the original had no ref at all.
      var oldRef = fromTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
      var newRef = newTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
      if (newRef != null) {
        if (oldRef != null && !string.IsNullOrEmpty(oldRef.Value)) {
          newRef.ReferenceItemTypeId = oldRef.ReferenceItemTypeId;
          newRef.Value = oldRef.Value;
        } else {
          newRef.ReferenceItemTypeId = (int)WeItemType.TodoModel;
          newRef.Value = fromTodo.Id.ToString();
        }
        await _mediator.SetProperty(newTodo, Cx.ItReferenceItem, newRef.Value);
      }

      // Prompt sourced from the TODO's own prompt (always exists), NOT the attempt —
      // a pre-run fail (NotStarted todo) has no attempt, and the old draft left such a
      // successor with a blank prompt while still reporting success.
      var newPrompt = newTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPromptTemplate);
      if (newPrompt != null) {
        var carried = await fromTodo.UserPrompt(_mediator, ct);        
        newPrompt.Value = "Original request: " + ScribanRaw(carried) + Environment.NewLine
                        + reasonLabel + ": " + ScribanRaw(reason);
        await newPrompt.SaveProp(newTodo, _mediator);
      }

      await _mediator.SetProperty(newTodo, Cx.ItFromTodo, fromTodo.Id.ToString());
      await _mediator.LinkContinueTodo(_context,fromTodo, newTodo.Id, ct);
      return newTodo;
    }

    private static async Task LinkContinueTodo(this IMediator _mediator, FabricDbContext _context, 
      ItemDto fromTodo, int newTodoId, CancellationToken ct) {
      var runInProgress = WeItemType.RunInProgress.AsIntString();
      foreach (var rel in fromTodo.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.TodoAttemptModel)) {
        var attemptId = rel?.RelatedItemId ?? 0;
        if (attemptId <= 0) continue;
        var attempt = await _context.GetItemDtoById(attemptId, ct);
        if (attempt?.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus)?.Value != runInProgress) continue;
        var contProp = attempt.Properties.FirstOrDefault(p => p.Name == Cx.ItContinueTodo);
        if (contProp != null) { 
          contProp.Value = newTodoId.ToString(); 
          await _mediator.SetProperty(attempt, Cx.ItContinueTodo, contProp.Value); 
        }
        break; // at most one in-progress attempt expected
      }
    }


    public static async Task TryEmergencyTerminate(this IMediator _mediator, FabricDbContext _context,
      ItemDto todo, string reason, Exception ex, CancellationToken ct) {
      try {
        var statusProp = todo.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
        if (statusProp != null) {
          statusProp.Value = ((int)WeItemType.TodoFailedForward).ToString();
          await statusProp.SaveProp(todo, _mediator);
        }
        await _mediator.AppendCloseReason(todo, $"EMERGENCY fail (handler threw): {reason} | {ex.Message}");
        await _mediator.ReleaseDeskIfParked(await _context.TryGetParentDesk(todo, ct), todo);
      } catch { /* swallow: nothing more we can safely do; outer result already reports failure */ }
    }

    private static string ScribanRaw(string s) => "{%{" + s + "%}}";
  }
}
