using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Storytime {
  public record AddBeatCommand(int SceneId, string name, string description, int? TodoId) : IMcpRequest, IRequest<ItemDto?>;
  public class AddBeatCommandHandler : IRequestHandler<AddBeatCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddBeatCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AddBeatCommand request, CancellationToken cancellationToken) {

      var parentItem = await _context.GetItemDtoById(request.SceneId);
      if (parentItem == null) { throw new Exception($"Parent scene with id {request.SceneId} not found"); }
      if (parentItem.ItemTypeId != (int)WeItemType.SceneModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId}; requires a {WeItemType.SceneModel} type {(int)WeItemType.SceneModel} parent.");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.BeatModel, request.name, request.description, "{}"));

      if (newItem != null) {
        var addedByProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
        if (addedByProp != null && request.TodoId > 0) {
          var attribution = await _mediator.Send(new ResolveAttributionQuery(request.TodoId.Value));
          addedByProp.Value = attribution.PresenceModelKey;
          await addedByProp.SaveProp(newItem, _mediator);
        }
      }

      return newItem;
    }
  }
}
