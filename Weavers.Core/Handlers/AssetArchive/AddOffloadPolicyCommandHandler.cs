using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Enums;

namespace Weavers.Core.Handlers.AssetArchive {

  public record AddOffloadPolicyCommand(int ParentItemId, string Name) : IMcpRequest, IRequest<ItemDto?>;
  public class AddOffloadPolicyCommandHandler : IRequestHandler<AddOffloadPolicyCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public AddOffloadPolicyCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<ItemDto?> Handle(AddOffloadPolicyCommand request, CancellationToken cancellationToken) {
      var parentItem = _context.Items.FirstOrDefault(i => i.Id == request.ParentItemId);
      if (parentItem == null) {
        throw new ArgumentException($"Parent item with ID {request.ParentItemId} not found.");
      }
      if (parentItem.ItemTypeId != (int)WeItemType.AssetArchiveModel) {
        throw new ArgumentException($"Parent item with ID {request.ParentItemId} is not of type AssetArchiveModel.");
      }
      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.OffloadPolicyModel, request.Name, "", "{}"));
      return newItem;
    }
  }
}
