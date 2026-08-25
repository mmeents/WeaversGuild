using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Storytime {

  public record ScheduleBeatWritersCommand(int StoryId, int handlerDeskId, int? fromTodoId) : IMcpRequest, IRequest<ScheduleBeatWriterResult>;
  public class ScheduleBeatWritersCommandHandler : IRequestHandler<ScheduleBeatWritersCommand, ScheduleBeatWriterResult> {
    private readonly IMediator _mediator;
    private readonly IAppGraphOrgService _appGraphOrgService;
    public ScheduleBeatWritersCommandHandler(IMediator mediator, IAppGraphOrgService appGraphOrgService) {
      _mediator = mediator;
      _appGraphOrgService = appGraphOrgService;
    }

    public async Task<ScheduleBeatWriterResult> Handle(ScheduleBeatWritersCommand request, CancellationToken cancellationToken) {

      List<int> ids = new List<int> { request.StoryId, request.handlerDeskId };
      var mainItems = await _mediator.Send(new GetItemsByIdsQuery(ids));

      var storyItem = mainItems.FirstOrDefault(i => i.Id == request.StoryId);
      if (storyItem == null) { throw new Exception($"item with id {request.StoryId} not found"); }
      if (storyItem.ItemTypeId != (int)WeItemType.StoryModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)storyItem.ItemTypeId}; requires a {WeItemType.StoryModel} type {(int)WeItemType.StoryModel} parent.");
      }
      var handlerDesk = mainItems.FirstOrDefault(i => i.Id == request.handlerDeskId);
      if (handlerDesk == null) { throw new Exception($"item with id {request.handlerDeskId} not found"); }
      if (handlerDesk.ItemTypeId != (int)WeItemType.DeskModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)handlerDesk.ItemTypeId}; requires a {WeItemType.DeskModel} type {(int)WeItemType.DeskModel} parent.");
      }
      var handlerEnabled = handlerDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItEnabled)?.Value.AsBoolean() ?? false;

      var result = new ScheduleBeatWriterResult {
        StoryId = request.StoryId,
        AddedTodoIds = new List<int>(),
        Errors = new List<string>()
      };
      
      if (storyItem.ItemTypeId != (int)WeItemType.StoryModel) {
        result.Errors.Add($"Story item {storyItem.Id} is not a StoryModel.");
        return result;
      }

      if (handlerDesk.ItemTypeId != (int)WeItemType.DeskModel) {
        result.Errors.Add($"Handler desk item {handlerDesk.Id} is not a Desk.");
        return result;
      }

      var scenes = storyItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.SceneModel && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank)
        .ToList();

      if (scenes.Count == 0) {
        result.Errors.Add($"Story does not appear to have any scenes. No work was done.");
        return result;
      }
      List<int> lookupIds = new List<int>();
      if (request.fromTodoId.HasValue && request.fromTodoId.Value > 0) {  // set overrides if a fromTodo was provided. if not leave original values in place.   
        lookupIds.Add(request.fromTodoId.Value);
      }
      foreach (var scene in scenes) {
        if (scene.RelatedItemId.HasValue && scene.RelatedItemId.Value > 0) {
          lookupIds.Add(scene.RelatedItemId.Value);
        }
      }
      var items = await _mediator.Send(new GetItemsByIdsQuery(lookupIds));

      ItemDto? fromTodo = null;
      if (request.fromTodoId.HasValue && request.fromTodoId.Value > 0) {  // set overrides if a fromTodo was provided. if not leave original values in place.   
        fromTodo = items.FirstOrDefault(i => i.Id == request.fromTodoId.Value);
        if (fromTodo == null) {
          result.Errors.Add($"fromTodoId {request.fromTodoId.Value} does not exist. No work was done.");
          return result;
        }
        if (fromTodo.ItemTypeId != (int)WeItemType.TodoModel) {
          result.Errors.Add($"fromTodoId {request.fromTodoId.Value} is not a Todo. No work was done.");
          return result;
        }
      }

      foreach (var scene in scenes) {
        try {
          var sceneId = scene.RelatedItemId.HasValue ? scene.RelatedItemId.Value : 0;
          if (sceneId > 0) {
            result.SceneIds.Add(sceneId);
            var sceneDto = items.FirstOrDefault(i => i.Id == sceneId);
            if (sceneDto == null) {
              result.Errors.Add($"Scene relation for storyId {storyItem.Id} has related item id {sceneId} but that item does not exist.");
              continue;
            }
            if (sceneDto != null && sceneDto.Relations.Any(r => r.RelatedItemTypeId == (int)WeItemType.BeatModel)) {
              result.Skipped.Add(sceneId);
              continue;
            }
            var beatWrittenProp = sceneDto!.Properties.FirstOrDefault(p => p.Name == Cx.ItBeatsRequested);
            if (beatWrittenProp != null) {
              if (beatWrittenProp.Value.AsBoolean()) {
                result.Skipped.Add(sceneId);
                continue;
              }
            }

            var todoName = $"Write Beats for storyId {storyItem.Id}, sceneId {sceneId}: {scene.RelatedItemName}";
            var promptTemplate = $"TodoId: {{{{model.todo.id}}}}: Write the Beats for storyId {storyItem.Id}, sceneId {sceneId}.";
            // Do the add to the desk 
            var addedTodo = await _appGraphOrgService.AddDeskTodo(handlerDesk, todoName, sceneId, promptTemplate);
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

              if (beatWrittenProp != null) {
                beatWrittenProp.Value = "1";
                await beatWrittenProp.SaveProp(sceneDto, _mediator);
              }

            } else {
              result.Errors.Add($"Add Desk Todo returned a null added item. target desk {handlerDesk.Id} for scene:{sceneId}");
            }

          } else {
            result.Errors.Add($"Scene relation for storyId {storyItem.Id} has no related item id.");
          }
        } catch (Exception ex) {
          result.Errors.Add($"Failed to schedule beat writers for sceneId {scene.RelatedItemId}: " + ex.Message);
        }
      }

      return result;
    }
  }
}
