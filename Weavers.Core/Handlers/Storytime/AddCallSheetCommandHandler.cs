using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Storytime {
  public record AddCallSheetCommand(int beatId, string name, string description, int? todoId) : IMcpRequest, IRequest<ItemDto?>;
  public class AddCallSheetCommandHandler : IRequestHandler<AddCallSheetCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public AddCallSheetCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<ItemDto?> Handle(AddCallSheetCommand request, CancellationToken cancellationToken) {

      var beat = await _context.GetItemDtoById(request.beatId);
      if (beat == null) { throw new Exception($"Beat with id {request.beatId} not found"); }
      if (beat.ItemTypeId != (int)WeItemType.BeatModel) {
        throw new Exception($"Invalid item type {(WeItemType)beat.ItemTypeId} ");
      }      

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(beat.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.CallSheetModel, request.name, request.description, "{}"));

      if (newItem != null) {
        var addedByProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
        if (addedByProp != null && request.todoId > 0) {
          var attribution = await _mediator.Send(new ResolveAttributionQuery(request.todoId.Value));
          addedByProp.Value = attribution.PresenceModelKey;
          await addedByProp.SaveProp(newItem, _mediator);
        }
      }


      return newItem;
    }
  }
}
