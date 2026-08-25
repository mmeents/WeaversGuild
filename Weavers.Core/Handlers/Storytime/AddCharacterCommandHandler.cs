using MediatR;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Storytime {
  public record AddCharacterCommand(int sceneId, string name, string description) : IMcpRequest, IRequest<ItemDto?>;
  public class AddCharacterCommandHandler : IRequestHandler<AddCharacterCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddCharacterCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AddCharacterCommand request, CancellationToken cancellationToken) {

      var parentItem = await _context.GetItemDtoById(request.sceneId);
      if (parentItem == null) { throw new Exception($"Parent scene with id {request.sceneId} not found"); }
      if (parentItem.ItemTypeId != (int)WeItemType.SceneModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId} ");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.CharacterModel, request.name, request.description, "{}"));
      return newItem;


    }
  }
}
