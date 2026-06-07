using MediatR;
using Microsoft.Extensions.Logging;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Templates;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using Weavers.Core.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Weavers.Core.Handlers.Presence {

  public record RunTodoAttemptCommand(
    int TodoId,
    bool IsPreview    
  ) : IRequest<RunTodoAttemptResult>;

  public class RunTodoAttemptResult { 
    public RunTodoAttemptOutcome Status { get; set; }
    public int HarnessId { get; set; } = 0;
    public string HarnessName { get; set; } = string.Empty;
    public int OperatorId { get; set; } = 0;
    public string Operator { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public string UserPrompt { get; set; } = string.Empty;
    public int? TodoAttemptId { get; set; } = null;
    public string? ResponseText { get; set; } = null;
    public string? ErrorMessage { get; set; } = null;
  }

  public enum RunTodoAttemptOutcome {
    PreviewNotSent,
    SuccessWithResponse,
    NotConfigured,        // chain didn't resolve: no operator, no model, no gateway, no target
    InvocationFailed,     // gateway reached but call errored/timed out
    MaxAttemptsReached,    // todo already at desk.MaxAttempts — don't run again  
    RanWithoutClose       // operator ran attempt(s) without calling a close tool

  }

  public class RunTodoAttemptCommandHandler : IRequestHandler<RunTodoAttemptCommand, RunTodoAttemptResult> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<RunTodoAttemptCommandHandler> _logger;
    private readonly ICryptoService _cryptoService;
    private readonly ILmStudioService _lmService;
    private readonly IGatewayRunRegistry _runRegistry;
    public RunTodoAttemptCommandHandler(
      FabricDbContext context, 
      IMediator mediator, 
      ILogger<RunTodoAttemptCommandHandler> logger, 
      ICryptoService cryptoService,
      ILmStudioService lmService,
      IGatewayRunRegistry runRegistry) {
      _context = context;
      _mediator = mediator;
      _logger = logger;
      _cryptoService = cryptoService;
      _lmService = lmService;
      _runRegistry = runRegistry;
    }
    
    public async Task<RunTodoAttemptResult> Handle(RunTodoAttemptCommand request, CancellationToken cancellationToken) {
      var result = new RunTodoAttemptResult() { 
        Status =  RunTodoAttemptOutcome.NotConfigured,        
      };
      int? harnessId = null;
      var presencePrompt = string.Empty;
      var sysPromptTemplate = string.Empty;
      var sysPrompt = string.Empty;
      var userPromptTemplate = string.Empty;
      var userPrompt = string.Empty;
      var presenceModel = string.Empty;
      ItemDto? attempt = null;
      SemaphoreSlim? gate = null;
      int nextAttemptNumber = 0;
      int maxAttempts = 0;
      try {

        var todoItem = await _context.GetItemDtoById(request.TodoId, cancellationToken);  // todo
        if (todoItem == null) return result.CreateFailure($"Todo item with ID {request.TodoId} not found.");

        // check set todo in progress status if not started. 
        var todoStatusProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
        if (todoStatusProp == null || 
          (todoStatusProp.Value != ((int)WeItemType.TodoNotStarted).ToString() 
            && todoStatusProp.Value != ((int)WeItemType.TodoInProgress).ToString())) {
          return result.CreateFailure($"Status property not found or indicated not ready {request.TodoId}.");
        }       

        var existingAttempts = todoItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.TodoAttemptModel).ToList();
        nextAttemptNumber = existingAttempts.Count + 1;

        // user prompt from todo item
        result.UserPrompt = await todoItem.UserPrompt(_mediator, cancellationToken);        
        if (string.IsNullOrEmpty(result.UserPrompt)) return result.CreateFailure($"User prompt template not found or empty for Todo item with ID {request.TodoId}.");          

        // Desk from todo item        
        var desk = await _context.TryGetParentDesk(todoItem, cancellationToken);
        if (desk == null) return result.CreateFailure($"Desk not found for Todo item with ID {request.TodoId}.");
        var deskId = desk.Id;

        maxAttempts = desk.Properties.FirstOrDefault(p => p.Name == Cx.ItMaxAttempts)?.Value.AsInt() ?? 5;
        if (nextAttemptNumber > maxAttempts && request.IsPreview) return result.CreateFailure($"Maximum attempts reached for Todo item with ID {request.TodoId}.", RunTodoAttemptOutcome.MaxAttemptsReached);
        if (nextAttemptNumber > maxAttempts && !request.IsPreview) {
          // already over budget before we even ran (e.g. a prior pass advanced attempts). Infra gives up.
          await _mediator.Send(new FailTodoCommand(request.TodoId,
            $"Max attempts ({maxAttempts}) exceeded on desk {deskId}; infra failing forward."), CancellationToken.None);
          return result.CreateFailure(
            $"Maximum attempts reached for Todo item with ID {request.TodoId}.", RunTodoAttemptOutcome.MaxAttemptsReached);
        }

        // system prompt from desk
        sysPrompt = await desk.SystemPrompt(_mediator, cancellationToken);        
        if (string.IsNullOrEmpty(sysPrompt)) return result.CreateFailure($"System prompt template not found or empty for Desk with ID {deskId}.");        

        // Operator from desk
        var operatorId = desk.Properties.FirstOrDefault(p => p.Name == Cx.ItOperator)?.Value.AsInt();
        if (operatorId == null || operatorId == 0) return result.CreateFailure($"Operator not configured for Desk with ID {deskId}.");

        var operatorItem = await _context.GetItemDtoById(operatorId.Value, cancellationToken);
        if (operatorItem == null || operatorItem.ItemTypeId != (int)WeItemType.DigitalOperatorModel) {          
          return result.CreateFailure($"Operator with ID {operatorId.Value} not found.");
        }

        result.OperatorId = operatorItem.Id;
        result.Operator = operatorItem.Name;
        presencePrompt = operatorItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSystemPrompt)?.Value ?? string.Empty;
        result.SystemPrompt = presencePrompt + Environment.NewLine + sysPrompt;

        var presId = operatorItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPresence)?.Value.AsInt();
        if (presId == null || presId == 0) {
          return result.CreateFailure($"Presence not configured for Operator with ID {operatorId.Value}.");
        }
        // get presence name is model.
        var presenceItem = await _context.GetItemDtoById(presId.Value, cancellationToken);
        if (presenceItem == null || presenceItem.ItemTypeId != (int)WeItemType.PresModelLmStudioModel) {
          return result.CreateFailure($"Presence with ID {presId.Value} not found.");
        }
        
        presenceModel = presenceItem.Properties.FirstOrDefault(p => p.Name == Cx.ItModelName)?.Value ?? string.Empty;
        harnessId = presenceItem.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains)?.ItemId;
        result.HarnessName = presenceItem.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains)?.ItemName ?? string.Empty;
        if (harnessId == null || harnessId == 0) {
          return result.CreateFailure($"Harness not found for Presence with ID {presId.Value}.");
        }
        result.HarnessId = harnessId.Value;

        if (request.IsPreview) {
          result.Status = RunTodoAttemptOutcome.PreviewNotSent;
          return result;
        }

        //  Preview ends from here is to make call. -----------------------------------------------------
        var deskCurrentTodoProp = desk.Properties.FirstOrDefault(p => p.Name == Cx.ItCurrentTodo);
        if (deskCurrentTodoProp == null) {
          return result.CreateFailure($"Current Todo property not found for Desk with ID {deskId}.");
        } 

        var currentTodo = deskCurrentTodoProp.Value;
        if (!string.IsNullOrEmpty(currentTodo) && currentTodo != request.TodoId.ToString()) {                    
          return result.CreateFailure($"Desk with ID {deskId} is currently processing Todo with ID {currentTodo}.", RunTodoAttemptOutcome.InvocationFailed);
        } else {  // set current todo to this one.
          deskCurrentTodoProp.Value = request.TodoId.ToString();
          await deskCurrentTodoProp.SaveProp(desk, _mediator);
        }

        gate = _runRegistry.GetGate(result.HarnessId);
        await gate.WaitAsync(cancellationToken);

        if (todoStatusProp.Value == ((int)WeItemType.TodoNotStarted).ToString()) {
          todoStatusProp.Value = ((int)WeItemType.TodoInProgress).ToString();
          await todoStatusProp.SaveProp(todoItem, _mediator);
        }

        // add todo add
        attempt = await _mediator.Send(
         new CreateRelatedItemCommand(todoItem.Id, (int)WeRelationTypes.Contains,
           (int)WeItemType.TodoAttemptModel, $"Take {nextAttemptNumber}", "", "{}")).ConfigureAwait(false);

        if (attempt == null) {
          return result.CreateFailure($"Failed to create Todo Attempt item for Todo with ID {request.TodoId}.", RunTodoAttemptOutcome.InvocationFailed);
        }

        result.TodoAttemptId = attempt.Id;        
        await _mediator.SetProperty(attempt, Cx.ItSystemPrompt, result.SystemPrompt).ConfigureAwait(false);
        await _mediator.SetProperty(attempt, Cx.ItUserPrompt, result.UserPrompt).ConfigureAwait(false);
        await _mediator.SetProperty(attempt, Cx.ItOperator, result.OperatorId.ToString()).ConfigureAwait(false);


        // make the chat request 
        var chatRequest = new ChatRequest() {
          Model = presenceModel,
          Input = result.UserPrompt,
          SystemPrompt = result.SystemPrompt,
          Temperature = Cx.DefaultTemperature,
          ContextLength = Cx.DefaultLmStudioContextLength,
          Integrations = Cx.availableToolsList.ToIntegrations()
        };

        // make the call for infrence and get response.
        var chatResponse = await _lmService.ChatAsync(result.HarnessId, chatRequest, cancellationToken);

        result.ResponseText = chatResponse.GetText();
        await _mediator.SetProperty(attempt, Cx.ItResponse, result.ResponseText ?? string.Empty).ConfigureAwait(false);

        // The operator may have called a close tool (Complete/Reject/Fail) THROUGH MCP during inference,
        // since tools are passed in via chatRequest.Integrations. Reload the todo fresh to find out whether
        // it actually closed — the in-memory todoItem DTO is stale by now.
        var freshTodo = await _context.GetItemDtoById(request.TodoId, cancellationToken);
        var closed = freshTodo?.IsTerminal() ?? false;

        if (closed) {
          // operator closed it; the close handler already released the desk + linked ContinueTodo to this attempt.
          await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunCompleted.AsIntString()).ConfigureAwait(false);
          result.Status = RunTodoAttemptOutcome.SuccessWithResponse;
          return result;
        }

        // Ran and produced a response, but never called a close tool. The attempt completed; the TODO did not.
        if (freshTodo != null) {
          var freshAttempts = freshTodo.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.TodoAttemptModel).ToList();
          nextAttemptNumber = freshAttempts.Count + 1;
          if (nextAttemptNumber >= maxAttempts) {
            // last allowed attempt and still not closed → operator couldn't finish in its time-box. Infra fails forward.
            // Call FailTodo BEFORE marking the attempt done, so its LinkContinueTodo still finds this attempt as RunInProgress.
            await _mediator.Send(new FailTodoCommand(request.TodoId,
              $"Operator ran {nextAttemptNumber} attempt(s) without calling a close tool; infra failing forward."),
              CancellationToken.None);
            await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunCompleted.AsIntString()).ConfigureAwait(false);
            result.Status = RunTodoAttemptOutcome.MaxAttemptsReached;
            return result;
          }
        }
        // attempts remain → leave the todo InProgress and CurrentTodo set; next scheduler pass retries as Take N+1.
        await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunCompleted.AsIntString()).ConfigureAwait(false);
        result.Status = RunTodoAttemptOutcome.RanWithoutClose;
        return result;

      } catch (OperationCanceledException) {
        if (attempt != null) { 
          await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunFailed.AsIntString());
          await _mediator.SetProperty(attempt, Cx.ItResponse, "Canceled.").ConfigureAwait(false);
        }
      } catch (Exception ex) {
        result.Status = RunTodoAttemptOutcome.InvocationFailed;
        result.ErrorMessage = $"An error occurred while attempting to run the Todo item with ID {request.TodoId}. {ex.Message}";
        _logger.LogError(ex, "Error running Todo item with ID {TodoId}", request.TodoId);

        if (attempt != null) {
          await _mediator.SetProperty(attempt, Cx.ItResponse, result.ErrorMessage).ConfigureAwait(false);

          // attempt != null means we got past config + created the attempt — a genuine invocation failure,
          // not a setup/NotConfigured problem. If it was the last allowed attempt, infra gives up.
          // (FailTodo first, while the attempt is still RunInProgress, so ContinueTodo links; then mark RunFailed.)
          if (nextAttemptNumber >= maxAttempts) {
            await _mediator.Send(new FailTodoCommand(request.TodoId,
              $"Invocation failed on final attempt {nextAttemptNumber}: {ex.Message}"), CancellationToken.None);
          }
          await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunFailed.AsIntString()).ConfigureAwait(false);
        }
        return result;
      } finally {
        if (gate != null) { 
          gate.Release(); 
        }
      }

      return result;
    }
  }

  public static class ChatRequestExts {

    public static RunTodoAttemptResult CreateFailure(this RunTodoAttemptResult result, string errorMessage, RunTodoAttemptOutcome outcome = RunTodoAttemptOutcome.NotConfigured) {
      result.ErrorMessage = errorMessage;
      result.Status = outcome;
      return result;
    }

    public static List<Integration> ToIntegrations(this List<string> listIntegrations) {
      return listIntegrations.Select(i => (Integration)new PluginIntegration { Id = i }).ToList();
    }
  }
}
