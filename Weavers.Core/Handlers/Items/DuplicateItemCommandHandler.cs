using MediatR;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record DuplicateItemCommand(int ItemId) : IMcpRequest, IRequest<ItemDto?>;
  public class DuplicateItemCommandHandler : IRequestHandler<DuplicateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public DuplicateItemCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<ItemDto?> Handle(DuplicateItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.GetItemDtoById(request.ItemId, cancellationToken);
      if (item == null) { 
        throw new Exception($"Item with ID {request.ItemId} not found.");
      }

      var parentItemId = item.GetParentId();
      var parent = await _context.GetItemDtoById(parentItemId, cancellationToken);
      if (parent == null) { 
        throw new Exception($"Parent item with ID {parentItemId} not found.");
      }
      var ir = item.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains && r.ItemId == parentItemId);
      
      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItemId,
          ir.RelationTypeId, 
          item.ItemTypeId,       
          item.Name, 
          item.Description, 
          item.Data));

      if (newItem != null) { 
        foreach(var prop in item.Properties) {
          var itemProp = newItem.Properties.FirstOrDefault(p => p.Name == prop.Name);
          await _mediator.Send(new AddUpdateItemPropertyCommand(
              itemProp?.Id ?? 0, newItem.Id, prop.Name, prop.Value, prop.ValueDataTypeId, prop.EditorTypeId, prop.ReferenceItemTypeId));

        }
      }

      return await _context.GetItemDtoById(newItem.Id, cancellationToken);

    }
  }
}
