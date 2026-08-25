using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Storytime {
  public record ScheduleBeatDirectorsCommand(int sceneId, int handlerDeskId, int? fromTodoId) : IRequest<ScheduleBeatDirectorResults>;
  public class ScheduleBeatDirectorsCommandHandler : IRequestHandler<ScheduleBeatDirectorsCommand, ScheduleBeatDirectorResults> {
    private readonly IMediator _mediator;    
    private readonly IAppGraphOrgService _appGraphOrgService;
    public ScheduleBeatDirectorsCommandHandler(IMediator mediator, IAppGraphOrgService appGraphOrgService) {
      _mediator = mediator;    
      _appGraphOrgService = appGraphOrgService;
    }
    public async Task<ScheduleBeatDirectorResults> Handle(ScheduleBeatDirectorsCommand request, CancellationToken cancellationToken) {

      List<int> ids = new List<int> { request.sceneId, request.handlerDeskId };
      if (request.fromTodoId.HasValue && request.fromTodoId.Value > 0) { 
        ids.Add(request.fromTodoId.Value); 
      }
      var mainItems = await _mediator.Send(new GetItemsByIdsQuery(ids));

      var sceneItem = mainItems.FirstOrDefault(i => i.Id == request.sceneId);
      if (sceneItem == null) { throw new Exception($"Parent scene with id {request.sceneId} not found"); }
      if (sceneItem.ItemTypeId != (int)WeItemType.SceneModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)sceneItem.ItemTypeId}; requires a {WeItemType.SceneModel} type {(int)WeItemType.SceneModel} parent.");
      }

      var handlerDesk = mainItems.FirstOrDefault(i => i.Id == request.handlerDeskId);
      if (handlerDesk == null) { throw new Exception($"item with id {request.handlerDeskId} not found"); }
      if (handlerDesk.ItemTypeId != (int)WeItemType.DeskModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)handlerDesk.ItemTypeId}; requires a {WeItemType.DeskModel} type {(int)WeItemType.DeskModel} parent.");
      }
      var handlerEnabled = handlerDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItEnabled)?.Value.AsBoolean() ?? false;

      var result = new ScheduleBeatDirectorResults {
        StoryId = sceneItem.GetParentId(),
        SceneId = sceneItem.Id,
        AddedTodoIds = new List<int>(),
        Errors = new List<string>()
      };     

      if (sceneItem.ItemTypeId != (int)WeItemType.SceneModel) {
        result.Errors.Add($"Story item {sceneItem.Id} is not a SceneModel.");
        return result;
      }

      if (handlerDesk.ItemTypeId != (int)WeItemType.DeskModel) {
        result.Errors.Add($"Handler desk item {handlerDesk.Id} is not a Desk.");
        return result;
      }

      var beats = sceneItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.BeatModel && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank)
        .ToList();

      if (beats.Count == 0) {
        result.Errors.Add($"Story does not appear to have any scenes. No work was done.");
        return result;
      }

      ItemDto? fromTodo = null;
      if (request.fromTodoId.HasValue && request.fromTodoId.Value > 0) {
        fromTodo = mainItems.FirstOrDefault(i => i.Id == request.fromTodoId.Value);
        if (fromTodo == null) {
          result.Errors.Add($"fromTodoId {request.fromTodoId.Value} does not exist. No work was done.");
          return result;
        }
        if (fromTodo.ItemTypeId != (int)WeItemType.TodoModel) {
          result.Errors.Add($"fromTodoId {request.fromTodoId.Value} is not a Todo. No work was done.");
          return result;
        }
      }
      var sceneBeatIds = new List<int>();
      foreach(var scenesBeat in beats) {
        if (scenesBeat.RelatedItemId.HasValue) {
          sceneBeatIds.Add(scenesBeat.RelatedItemId.Value);
        }
      }
      var sceneBeatItems = await _mediator.Send(new GetItemsByIdsQuery(sceneBeatIds));

      foreach (var scenesBeat in beats) {
        try {
          var beatId = scenesBeat.RelatedItemId.HasValue ? scenesBeat.RelatedItemId.Value : 0;
          if (beatId > 0) {
            result.BeatIds.Add(beatId);
            var beatDto = sceneBeatItems.FirstOrDefault(i => i.Id == beatId);
            if (beatDto == null) {
              result.Errors.Add($"Beat relation for sceneId {sceneItem.Id} has related item id {beatId} but that item does not exist.");
              continue;
            }
            if (beatDto != null && beatDto.Relations.Any(r => r.RelatedItemTypeId == (int)WeItemType.CallSheetModel)) {
              result.Skipped.Add(beatId);
              continue;
            }
            var callSheetWrittenProp = beatDto!.Properties.FirstOrDefault(p => p.Name == Cx.ItCallSheetRequested);
            if (callSheetWrittenProp != null) {
              if (callSheetWrittenProp.Value.AsBoolean()) {
                result.Skipped.Add(beatId);
                continue;
              }
            }

            // add the call sheet.
            var callSheet = await _mediator.Send(
              new CreateRelatedItemCommand(beatDto.Id, (int)WeRelationTypes.Contains,
                (int)WeItemType.CallSheetModel, $"CallSheet for BeatId {beatDto.Id}", "", "{}"));
            if (callSheet == null) {
              result.Errors.Add($"Failed to create CallSheet for beatId: {beatDto.Id}");
              continue;
            }

            var todoName = $"Direct the beat for beatId: {beatDto.Id}, sceneId {beatId}: {scenesBeat.RelatedItemName}";
            var promptTemplate = $"TodoId: {{{{model.todo.id}}}}:" + Environment.NewLine +
              $"Direct the beat for beatId: {beatDto.Id} within the sceneId {beatId};" + Environment.NewLine +
              $"The Call Sheet has already been created. Use Call Sheet Id: {callSheet.Id}.";

            var addedTodo = await _appGraphOrgService.AddDeskTodo(handlerDesk, todoName, beatId, promptTemplate); // Do the add to the desk 

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

              if (callSheetWrittenProp != null) {
                callSheetWrittenProp.Value = "1";
                await callSheetWrittenProp.SaveProp(beatDto, _mediator);
              } else {
                ItemPropertyDto newBeatWrittenProp = new ItemPropertyDto {
                  ItemId = beatDto.Id,
                  Name = Cx.ItCallSheetRequested,
                  Value = "1",
                  IsRequired = false,
                  IsReadOnly = false,
                  IsVisible = true,
                  ValueDataTypeId = (int)WeDataType.Boolean,
                  EditorTypeId = (int)WeEditorType.Boolean
                };
                await newBeatWrittenProp.SaveProp(beatDto, _mediator);
              }

            } else {
              result.Errors.Add($"Add Desk Todo returned a null added item. target desk {handlerDesk.Id} for beat:{beatId}");
            }

          } else {
            result.Errors.Add($"Scene relation for sceneId {sceneItem.Id} has no related item id.");
          }
        } catch (Exception ex) {
          result.Errors.Add($"Failed to schedule beat directors for beatId {scenesBeat.RelatedItemId}: " + ex.Message);
        }
      }

      return result;
    }
  }
}
