using MediatR;
using System.Text.Json;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Storytime {
  public record ScheduleActorPerformancesCommand(int performanceId, int handlerDeskId, int? fromTodoId) : IMcpRequest, IRequest<ScheduleActorPerformanceResults>;
  public class ScheduleActorPerformancesCommandHandler : IRequestHandler<ScheduleActorPerformancesCommand, ScheduleActorPerformanceResults> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    private readonly IAppGraphOrgService _appGraphOrgService;

    public ScheduleActorPerformancesCommandHandler(IMediator mediator, FabricDbContext context, IAppGraphOrgService appGraphOrgService) {
      _mediator = mediator;
      _context = context;
      _appGraphOrgService = appGraphOrgService;
    }

    public async Task<ScheduleActorPerformanceResults> Handle(ScheduleActorPerformancesCommand request, CancellationToken cancellationToken) {

      List<int> ids = new List<int> { request.performanceId, request.handlerDeskId };
      if (request.fromTodoId.HasValue && request.fromTodoId.Value > 0) {
        ids.Add(request.fromTodoId.Value);
      }
      var mainItems = await _mediator.Send(new GetItemsByIdsQuery(ids));

      var performanceItem = mainItems.FirstOrDefault(i  => i.Id == request.performanceId);
      if (performanceItem == null) { throw new Exception($"item with id {request.performanceId} not found"); }
      if (performanceItem.ItemTypeId != (int)WeItemType.PerformanceModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)performanceItem.ItemTypeId}; requires a {WeItemType.PerformanceModel} type {(int)WeItemType.PerformanceModel} parent.");
      }
      var handlerDesk = mainItems.FirstOrDefault(i => i.Id == request.handlerDeskId);
      if (handlerDesk == null) { throw new Exception($"item with id {request.handlerDeskId} not found"); }
      if (handlerDesk.ItemTypeId != (int)WeItemType.DeskModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)handlerDesk.ItemTypeId}; requires a {WeItemType.DeskModel} type {(int)WeItemType.DeskModel} parent.");
      }
      var handlerEnabled = handlerDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItEnabled)?.Value.AsBoolean() ?? false;

      var result = new ScheduleActorPerformanceResults {
        SceneId = performanceItem.GetParentId(),
        PerformanceId = performanceItem.Id,
        AddedTodoIds = new List<int>(),
        Errors = new List<string>()
      };                     

      var sceneId = performanceItem.GetParentId();
      var sceneItem = await _context.GetItemDtoById(sceneId);
      if (sceneItem == null) {
        result.Errors.Add($"Performance items require a Scene type parent.");
        return result;
      }

      var script = string.IsNullOrWhiteSpace(performanceItem.Data) || performanceItem.Data == "{}" ?
       new PerformanceScript() :
       JsonSerializer.Deserialize<PerformanceScript>(performanceItem.Data) ?? new PerformanceScript();

      var actorPerfs = performanceItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.ActorPerformanceModel && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank)
        .ToList();

      var actorNames = new Dictionary<int, string>();
      foreach (var scriptNode in script.Entries.OrderBy(e => e.Rank)) { // add missing actor performances for each script node. if it already exists, skip it.
        ItemDto? newPerf = null;
        if (scriptNode != null && scriptNode.Type == Cx.ActionType) {   // got to here 

          var characterId = scriptNode.CharacterId;
          var rank = scriptNode.Rank;
          var actorPerfName = $"Rank: {rank}; Character {scriptNode.CharacterName};";

          var existingPerf = actorPerfs.FirstOrDefault(ap => ap.RelatedItemName == actorPerfName);
          if (existingPerf == null) {

            newPerf = await _mediator.Send(
              new CreateRelatedItemCommand(performanceItem.Id, (int)WeRelationTypes.Contains,
                (int)WeItemType.ActorPerformanceModel, actorPerfName, "", "{}"));

            if (newPerf != null) {
              result.ActorPerformanceIds.Add(newPerf.Id);
              actorNames[newPerf.Id] = actorPerfName;

              var characterProp = newPerf.Properties.FirstOrDefault(p => p.Name == Cx.ItCharacter);
              if (characterProp != null) {
                characterProp.Value = characterId.ToString();
                await characterProp.SaveProp(newPerf, _mediator);
              }

              var rankProp = newPerf.Properties.FirstOrDefault(p => p.Name == Cx.ItRank);
              if (rankProp != null) {
                rankProp.Value = rank.ToString();
                await rankProp.SaveProp(newPerf, _mediator);
              }

              var InstructionsProp = newPerf.Properties.FirstOrDefault(p => p.Name == Cx.ItInstructions);
              if (InstructionsProp != null) {
                InstructionsProp.Value = scriptNode.Text;
                await InstructionsProp.SaveProp(newPerf, _mediator);
              }
            }

          } else { // already exists             
            result.ActorPerformanceIds.Add(existingPerf.RelatedItemId!.Value); // add for the counts.
            result.Skipped.Add(existingPerf.RelatedItemId!.Value);  // skip if it exists already. we don't want to create duplicates.
          }
        }
      }

      if (result.ActorPerformanceIds.Count == 0) {
        result.Errors.Add($"Story does not appear to have any actor performances.");
        return result;
      }

      ItemDto? fromTodo = null;
      if (request.fromTodoId.HasValue && request.fromTodoId.Value > 0) {  // set overrides if a fromTodo was provided. if not leave original values in place.   
        fromTodo = mainItems.FirstOrDefault(i => i.Id == request.fromTodoId.Value);
        if (fromTodo == null) {
          result.Errors.Add($"fromTodoId was specified but {request.fromTodoId.Value} does not exist. No work was done.");
          return result;
        }
        if (fromTodo.ItemTypeId != (int)WeItemType.TodoModel) {
          result.Errors.Add($"fromTodoId was specified but {request.fromTodoId.Value} is not a Todo. No work was done.");
          return result;
        }
      }

      foreach (var actorPerfId in result.ActorPerformanceIds) {
        try {
          if (actorPerfId > 0) {

            if (result.Skipped.Contains(actorPerfId)) {
              continue;  // was called skipped above.
            }

            var todoName = $"Act for actorPerformanceId: {actorPerfId}; {actorNames[actorPerfId]}";
            var promptTemplate = $"TodoId: {{{{model.todo.id}}}}:" + Environment.NewLine +
              $"{todoName}" + Environment.NewLine +
              $"Note: This is a component of the parent performanceId: {performanceItem.Id};";

            var addedTodo = await _appGraphOrgService.AddDeskTodo(handlerDesk, todoName, actorPerfId, promptTemplate); // Do the add to the desk 

            if (addedTodo != null) {

              if (fromTodo != null) {
                var fromTodoProp = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItFromTodo);
                if (fromTodoProp != null) {
                  fromTodoProp.Value = fromTodo.Id.ToString();
                  await fromTodoProp.SaveProp(addedTodo, _mediator);
                }

                var existingDepth = int.TryParse(fromTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth)?.Value ?? "0", out var depth) ? depth : 0;
                var addedDepthProp = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth);
                if (addedDepthProp != null) {
                  addedDepthProp.Value = (existingDepth + 1).ToString();
                  await addedDepthProp.SaveProp(addedTodo, _mediator);
                }
              }


              try {

                var isReadyProperty = addedTodo.Properties.FirstOrDefault(p => p.Name == Cx.ItConfirmedReady);
                if (isReadyProperty != null && !isReadyProperty.Value.AsBoolean() && handlerEnabled) {
                  isReadyProperty.Value = "1";
                  await isReadyProperty.SaveProp(addedTodo, _mediator);
                }

              } catch (Exception ex) {
                result.Errors.Add($"Failed to set todo ready for todoId {addedTodo.Id}: " + ex.Message);
              }

              result.AddedTodoIds.Add(addedTodo.Id);

            } else {
              result.Errors.Add($"Add Desk Todo returned a null added item. target desk {handlerDesk.Id} for actor performance:{actorPerfId}");
            }

          } else {
            result.Errors.Add($"performance relation to actor performance {actorPerfId} has no related item id.");
          }
        } catch (Exception ex) {
          result.Errors.Add($"Failed to schedule actor performances Id: {actorPerfId}: " + ex.Message);
        }
      }

      return result;
    }
  }
}
