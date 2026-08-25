using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Storytime {

  public record AddSceneCommand(int storyId, string name, string description, string entryState, string exitState, int todoId) : IMcpRequest, IRequest<ItemDto?>;
  public class AddSceneCommandHandler : IRequestHandler<AddSceneCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddSceneCommandHandler(IMediator mediator, FabricDbContext context) {
      this._mediator = mediator;
      this._context = context;
    }

    public async Task<ItemDto?> Handle(AddSceneCommand request, CancellationToken cancellationToken) {

      var storyItem = await _context.GetItemDtoById(request.storyId);
      if (storyItem == null) { throw new Exception($"Parent item with id {request.storyId} not found"); }
      if (storyItem.ItemTypeId != (int)WeItemType.StoryModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)storyItem.ItemTypeId}; requires a {WeItemType.StoryModel} type {(int)WeItemType.StoryModel} parent.");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(storyItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.SceneModel, request.name, request.description, "{}"));

      if (newItem != null) {

        var entryStateProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItEntryState);
        if (entryStateProp != null) {
          entryStateProp.Value = request.entryState;
          await entryStateProp.SaveProp(newItem, _mediator);
        }

        var exitStateProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItExitState);
        if (exitStateProp != null) {
          exitStateProp.Value = request.exitState;
          await exitStateProp.SaveProp(newItem, _mediator);
        }

        var addedByProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
        if (addedByProp != null && request.todoId > 0) {
          var attribution = await _mediator.Send(new ResolveAttributionQuery(request.todoId));
          addedByProp.Value = attribution.PresenceModelKey;
          await addedByProp.SaveProp(newItem, _mediator);
        }
      }
      return newItem;

    }
  }
}
