using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Presence;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Comfy {
  public record RunComfyTodoAttemptCommand(int TodoId,
    bool IsPreview) : IRequest<RunComfyTodoAttemptResult>;

  public class RunComfyTodoAttemptResult {
    public RunComfyTodoAttemptOutcome Status { get; set; }
    public int HarnessId { get; set; } = 0;
    public string HarnessName { get; set; } = string.Empty;
    public string ResolvedPromptJson { get; set; } = string.Empty;
    public string TodoName { get; set; } = string.Empty;
    public int TodoId { get; set; } = 0;
    public int? AttemptId { get; set; } = null;
    public string? PromptId { get; set; } = null;
    public string? ResponseText { get; set; } = null;
    public string? ErrorMessage { get; set; } = null;
  }

  public enum RunComfyTodoAttemptOutcome {
    SuccessWithResponse,
    NotConfigured,      // chain didn't resolve: no template, no service, no gateway, no harness
    InvocationFailed,   // gateway reached but call errored/timed out
    GenerationFailed,   // Comfy accepted the prompt but execution errored (status_str != "success")
    TimedOut            // /history never populated within the poll window
  }

  public class RunComfyTodoAttemptCommandHandler(
    FabricDbContext context,
    IMediator mediator,
    ILogger<RunComfyTodoAttemptCommandHandler> logger,
    IHttpClientFactory httpClientFactory,
    IAppSessionService sessionService,
    IGatewayRunRegistry runRegistry) : IRequestHandler<RunComfyTodoAttemptCommand, RunComfyTodoAttemptResult> {

    private readonly FabricDbContext _context = context;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<RunComfyTodoAttemptCommandHandler> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IGatewayRunRegistry _runRegistry = runRegistry;
    private readonly IAppSessionService _sessionService = sessionService;

    private const int HistoryPollOneSecinMs = 1000;  // 1 second in ms.
    private const int HistoryPollMaxAttempts = 120; // ~2 min; generation-time headroom, not LLM-token headroom

    public async Task<RunComfyTodoAttemptResult> Handle(RunComfyTodoAttemptCommand request, CancellationToken cancellationToken) {
      var result = new RunComfyTodoAttemptResult { Status = RunComfyTodoAttemptOutcome.NotConfigured };
      ItemDto? attempt = null;
      SemaphoreSlim? gate = null;
      ItemDto? todoItem = null;
      ItemPropertyDto? statusProp = null;
      var gateHeld = false;

      try {
        todoItem = await _context.GetItemDtoById(request.TodoId, cancellationToken);
        if (todoItem == null) return result.CreateFailure($"Comfy todo {request.TodoId} not found.");
        result.TodoId = todoItem.Id;
        result.TodoName = todoItem.Name;

        var timeoutSec = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTimeoutSec)?.Value.AsInt() ?? HistoryPollMaxAttempts;
        var confirmedReady = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItConfirmedReady)?.Value.AsBoolean() ?? false;        

        statusProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
        if (statusProp == null ||
            (statusProp.Value != ((int)WeItemType.TodoNotStarted).ToString()
              && statusProp.Value != ((int)WeItemType.TodoInProgress).ToString())) {
          return result.CreateFailure($"Comfy todo {request.TodoId} not in a runnable state.");
        }        

        // Workflow template holds enabled/disabled flag as a template wide cut off switch.
        // it's also garenteed to be child of comfyWorkflowFolder child of ComfyServiceModel,
        // so if we use that route then workflows can be anywhere. 
        var wfTemplateId = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItWfTemplate)?.Value.AsInt();
        if (wfTemplateId == null || wfTemplateId == 0) return result.CreateFailure($"No workflow template set on todo {request.TodoId}.");        

        var wfTemplateItem = await _context.GetItemDtoById(wfTemplateId.Value, cancellationToken);
        if (wfTemplateItem == null) {
          return result.CreateFailure($"Workflow template {wfTemplateId.Value} not found.");
        }
        var wfEnabled = wfTemplateItem.Properties.FirstOrDefault(p => p.Name == Cx.ItEnabled)?.Value.AsBoolean() ?? false;


        // Chain up to the Comfy service (URL) — mirrors the get-query's join path.
        var opsFolderId = wfTemplateItem.IncomingRelations.FirstOrDefault(r => r.ItemTypeId == (int)WeItemType.ComfyWorkflowFolderModel)?.ItemId;
        if (opsFolderId == null) return result.CreateFailure($"ComfyOperationsModel parent not found for todo {request.TodoId}.");

        var opsFolderItem = await _context.GetItemDtoById(opsFolderId.Value, cancellationToken);
        var serviceId = opsFolderItem?.IncomingRelations.FirstOrDefault(r => r.ItemTypeId == (int)WeItemType.ComfyServiceModel)?.ItemId;
        if (serviceId == null) return result.CreateFailure($"ComfyServiceModel not found for ops folder {opsFolderId.Value}.");

        var serviceItem = await _context.GetItemDtoById(serviceId.Value, cancellationToken);
        var baseUrl = serviceItem?.Properties.FirstOrDefault(p => p.Name == Cx.ItUrlBase)?.Value;
        var outputFolder = serviceItem?.Properties.FirstOrDefault(p => p.Name == Cx.ItServiceOutput)?.Value ?? "";

        var harnessGatewayId = serviceItem!.IncomingRelations.FirstOrDefault(r => r.ItemTypeId == (int)WeItemType.HarnessGatewaysModel)?.ItemId;
        var harnessGatewayItem = harnessGatewayId == null ? null : await _context.GetItemDtoById(harnessGatewayId.Value, cancellationToken);
        var harnessId = harnessGatewayId == null ? null : harnessGatewayItem?.IncomingRelations
          .FirstOrDefault(r => r.ItemTypeId == (int)WeItemType.HarnessAppModel)?.ItemId;
        if (harnessId == null) return result.CreateFailure($"Harness not resolved for Comfy service {serviceId.Value}.");
        var harnessName = harnessGatewayId == null ? null : harnessGatewayItem?.IncomingRelations
          .FirstOrDefault(r => r.ItemTypeId == (int)WeItemType.HarnessAppModel)?.ItemName;
        result.HarnessId = harnessId.Value;
        result.HarnessName = harnessName ?? string.Empty;

        // Op params — children of the todo, each carrying a section/object/prop path + override.
        var opParamIds = todoItem.Relations
          .Where(r => r.RelatedItemTypeId == (int)WeItemType.ComfyOpParamModel)          
          .Select(r => r.RelatedItemId.HasValue ? r.RelatedItemId.Value : 0)
          .ToList();
        var opParams = await _mediator.Send(new GetItemsByIdsQuery(opParamIds));

        // Get the workflow template from the Todo's data field.
        var workflow = JsonNode.Parse(todoItem.Data)!.AsObject();

        // Apply overrides from the todo's op params to the workflow template.
        foreach (var paramItem in opParams) {
          if (paramItem == null) continue;
          // read properties
          var sectionKey = paramItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSectionKey)?.Value; // usually id to lookup first. 
          var objectKey = paramItem.Properties.FirstOrDefault(p => p.Name == Cx.ItObjectKey)?.Value; // per secion the name of object with a prop to override          
          var propKey = paramItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropKey)?.Value;
          var overrideType = paramItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOverrideType)?.Value.AsInt();
          var propValue = paramItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropValue)?.Value ?? string.Empty;

          // validate that the section/object/prop path exists in the workflow template, and that the override type is valid.
          if (string.IsNullOrEmpty(objectKey)
            || string.IsNullOrEmpty(sectionKey)
            || string.IsNullOrEmpty(propKey)
            || workflow[sectionKey]?[objectKey] is not JsonObject targetSection) { 
            throw new InvalidOperationException($"Op param {paramItem.Id} has invalid section/object/prop path: section '{sectionKey}', object '{objectKey}', prop '{propKey}'");
          }

          // Apply the override based on the type.
          if (overrideType == (int)WeItemType.CtOverrideSeed) {
            targetSection[propKey] = Random.Shared.NextInt64(0, long.MaxValue);
          } else if (overrideType == (int)WeItemType.CtOverrideFilePath && File.Exists(propValue)) {
            targetSection[propKey] = propValue;
          } else if (overrideType == (int)WeItemType.CtOverrideInt) {
            if (!string.IsNullOrEmpty(propValue)) {
              targetSection[propKey] = int.Parse(propValue);
            }
          } else if (overrideType == (int)WeItemType.CtOverrideDecimal) {
            if (!string.IsNullOrEmpty(propValue)) {
              targetSection[propKey] = decimal.Parse(propValue);
            }
          } else {
            if (!string.IsNullOrEmpty(propValue)) {
              targetSection[propKey] = propValue;
            }
          }
        } // foreach op param

        // Build the payload to send to ComfyUI. The workflow template is now fully resolved with any overrides applied.
        var clientId = Guid.NewGuid().ToString();
        var payload = new JsonObject { ["prompt"] = workflow, ["client_id"] = clientId };
        result.ResolvedPromptJson = payload.ToJsonString();
                
        if (request.IsPreview) {    
          result.Status = RunComfyTodoAttemptOutcome.SuccessWithResponse;
          return result; // caller can inspect ResolvedPromptJson for preview
        }

        // Preview ends from here is make call ------------------------
        if (!confirmedReady) return result.CreateFailure($"Comfy todo {request.TodoId} not confirmed ready for execution.");
        if (string.IsNullOrWhiteSpace(baseUrl)) return result.CreateFailure($"No UrlBase configured on ComfyServiceModel {serviceId.Value}.");        
        if (!wfEnabled) return result.CreateFailure($"Workflow template {wfTemplateId.Value} is disabled.");
        if (_sessionService.HarnessId != result.HarnessId) return result.CreateFailure($"Harness mismatch: session harness {_sessionService.HarnessId} does not match todo harness {result.HarnessId}.");

        // Mark taken since everything checks out and not preview.
        if (statusProp.Value == ((int)WeItemType.TodoNotStarted).ToString()) {
          statusProp.Value = ((int)WeItemType.TodoInProgress).ToString();
          await statusProp.SaveProp(todoItem, _mediator);
        }

        // Create the attempt, stash the resolved (post-override) submission JSON on its Data field — mirrors
        // the workflow template's own use of Data for JSON, and matches what your Data-field editor already shows.
        var nextRank = 1;
        nextRank = await _mediator.Send(new GetNextItemRankQuery(todoItem.Id)) + 1;
        attempt = await _mediator.Send(
          new CreateRelatedItemCommand(todoItem.Id, (int)WeRelationTypes.Contains,
            (int)WeItemType.ComfyOpTodoAttemptModel, $"Attempt {nextRank}", "", result.ResolvedPromptJson), cancellationToken);
        if (attempt == null) {
          if (statusProp != null && todoItem != null) {
            statusProp.Value = ((int)WeItemType.TodoFailedForward).ToString();
            await statusProp.SaveProp(todoItem, _mediator);
          }
          return result.CreateFailure($"Failed to create attempt for todo {request.TodoId}.", RunComfyTodoAttemptOutcome.InvocationFailed);
        }
        result.AttemptId = attempt.Id;

        gate = _runRegistry.GetGate(result.HarnessId);  // ensures only one Comfy run at a time per harness
        await gate.WaitAsync(cancellationToken); 
        gateHeld = true;

        // Submit the prompt to ComfyUI — the /prompt endpoint returns a prompt_id.
        var http = _httpClientFactory.CreateClient();
        http.BaseAddress = new Uri(baseUrl);        
        var submitResponse = await http.PostAsJsonAsync("/prompt", payload, cancellationToken);
        submitResponse.EnsureSuccessStatusCode();
        var submitResult = await submitResponse.Content.ReadFromJsonAsync<JsonObject>(cancellationToken: cancellationToken);
        var promptId = submitResult?["prompt_id"]?.GetValue<string>();
        if (string.IsNullOrEmpty(promptId)) {
          if (statusProp != null && todoItem != null) {
            statusProp.Value = ((int)WeItemType.TodoFailedForward).ToString();
            await statusProp.SaveProp(todoItem, _mediator);
          }
          return result.CreateFailure("Comfy /prompt returned no prompt_id.", RunComfyTodoAttemptOutcome.InvocationFailed);
        }
        result.PromptId = promptId;

        // Poll the /history endpoint for the prompt_id until we get a populated history entry or timeout.
        JsonObject? historyEntry = null;
        for (var i = 0; i < timeoutSec; i++) {
          await Task.Delay(HistoryPollOneSecinMs, cancellationToken);
          var history = await http.GetFromJsonAsync<JsonObject>($"/history/{promptId}", cancellationToken);
          if (history != null && history.ContainsKey(promptId)) { 
            historyEntry = history[promptId]!.AsObject(); break; 
          }
        }
        if (historyEntry == null) {
          if (statusProp != null && todoItem != null) {
            statusProp.Value = ((int)WeItemType.TodoFailedForward).ToString();
            await statusProp.SaveProp(todoItem, _mediator);
          }
          await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunFailed.AsIntString());
          return result.CreateFailure($"Comfy history never populated for prompt {promptId}.", RunComfyTodoAttemptOutcome.TimedOut);
        }
        result.ResponseText = historyEntry.ToJsonString(); // outputs/asset refs live in here; binary bytes never travel through this response
        await _mediator.SetProperty(attempt, Cx.ItResponse, result.ResponseText);

        // ComfyUI can return a populated history entry for a FAILED run — check status_str, don't infer success from presence alone.
        var statusStr = historyEntry["status"]?["status_str"]?.GetValue<string>();
        if (!string.Equals(statusStr, "success", StringComparison.OrdinalIgnoreCase)) {
          result.ResponseText = historyEntry.ToJsonString();
          await _mediator.SetProperty(attempt, Cx.ItResponse, result.ResponseText);
          await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunFailed.AsIntString());
          if (statusProp != null && todoItem != null) {
            statusProp.Value = ((int)WeItemType.TodoFailedForward).ToString();
            await statusProp.SaveProp(todoItem, _mediator);
          }
          return result.CreateFailure($"Comfy execution failed for prompt {promptId}.", RunComfyTodoAttemptOutcome.GenerationFailed);
        }

        // ComfyUI run succeeded, so create ComfyMediaFileModel items for each output file and set their properties.
        IEnumerable<(string FileName, string SubFolder, string Type)> files = ChatRequestExts.GetOutputFiles(historyEntry);
        foreach (var (FileName, SubFolder, Type) in files) {
          var filePath = Path.Combine( outputFolder, SubFolder, FileName);
          var fileItem = await _mediator.Send(new CreateRelatedItemCommand(attempt.Id, (int)WeRelationTypes.Contains,
            (int)WeItemType.ComfyMediaFileModel, FileName, "", "{}"), cancellationToken);
          if (fileItem == null) {
            _logger.LogWarning("Failed to create ComfyMediaFileModel item for {FilePath} on attempt {AttemptId}", filePath, attempt.Id);
          } else {
            await _mediator.SetProperty(fileItem, Cx.ItFilePath, filePath);
            await _mediator.SetProperty(fileItem, Cx.ItMediaType, Type);
            await _mediator.SetProperty(fileItem, Cx.ItFromAttempt, attempt.Id.ToString());
            await _mediator.SetProperty(fileItem, Cx.ItFromTodo, todoItem.Id.ToString());
          }
        }

        // Mark the attempt
        await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunCompleted.AsIntString());
        
        // Mark the todo as completed.
        statusProp.Value = ((int)WeItemType.TodoCompleteForward).ToString();
        await statusProp.SaveProp(todoItem, _mediator);
        

        // Single-try policy: no chaining, no retry scheduling here
        result.Status = RunComfyTodoAttemptOutcome.SuccessWithResponse;
        return result;

      } catch (OperationCanceledException) {
        if (attempt != null) {
          await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunFailed.AsIntString());
          await _mediator.SetProperty(attempt, Cx.ItResponse, "Canceled.");
        }
        if (statusProp != null && todoItem != null) {
          statusProp.Value = ((int)WeItemType.TodoFailedForward).ToString();
          await statusProp.SaveProp(todoItem, _mediator);
        }
        return result;
      } catch (Exception ex) {
        result.Status = RunComfyTodoAttemptOutcome.InvocationFailed;
        result.ErrorMessage = $"Error running Comfy todo {request.TodoId}: {ex.Message}";
        _logger.LogError(ex, "Error running Comfy todo {TodoId}", request.TodoId);
        if (attempt != null) {
          await _mediator.SetProperty(attempt, Cx.ItResponse, result.ErrorMessage);
          await _mediator.SetProperty(attempt, Cx.ItStatus, WeItemType.RunFailed.AsIntString());
        }
        if (statusProp != null && todoItem != null) {
          statusProp.Value = ((int)WeItemType.TodoFailedForward).ToString();
          await statusProp.SaveProp(todoItem, _mediator);
        }
        return result;
      } finally {
        if (gateHeld) {
          gate?.Release();
        }
      }
    }
  }

  public static class ChatRequestExts {

    public static RunComfyTodoAttemptResult CreateFailure(this RunComfyTodoAttemptResult result, string errorMessage, RunComfyTodoAttemptOutcome outcome = RunComfyTodoAttemptOutcome.NotConfigured) {
      result.ErrorMessage = errorMessage;
      result.Status = outcome;
      return result;
    }

    public static IEnumerable<(string FileName, string SubFolder, string Type)> GetOutputFiles(JsonObject historyEntry) {
      if (historyEntry["outputs"] is not JsonObject outputs) yield break;
      foreach (var (_, nodeOut) in outputs) {
        if (nodeOut is not JsonObject byKind) continue;
        foreach (var (_, arr) in byKind) {                // images / gifs / audio / whatever a node emits
          if (arr is not JsonArray items) continue;
          foreach (var it in items.OfType<JsonObject>()) { // skips e.g. "animated":[false], text outputs
            var fn = it["filename"]?.GetValue<string>();
            if (string.IsNullOrEmpty(fn)) continue;
            yield return (fn, it["subfolder"]?.GetValue<string>() ?? "", it["type"]?.GetValue<string>() ?? "output");
          }
        }
      }
    }
  }

}
