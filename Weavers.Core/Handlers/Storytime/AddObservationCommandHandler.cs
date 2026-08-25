using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Storytime {

  public record AddObservationCommand(int actorPerformanceId, string name, string description, int todoId) : IMcpRequest, IRequest<ItemDto?>;
  public class AddObservationCommandHandler : IRequestHandler<AddObservationCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public AddObservationCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<ItemDto?> Handle(AddObservationCommand request, CancellationToken cancellationToken) {
      var performanceItem = await _context.GetItemDtoById(request.actorPerformanceId);
      if (performanceItem == null) { throw new Exception($"Parent scene with id {request.actorPerformanceId  } not found"); }
      if (performanceItem.ItemTypeId != (int)WeItemType.PerformanceModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)performanceItem.ItemTypeId}; requires a {WeItemType.PerformanceModel} type {(int)WeItemType.PerformanceModel} parent.");
      }
      
      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(performanceItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.ObservationModel, request.name, request.description, "{}"));

      if (newItem != null) {
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
