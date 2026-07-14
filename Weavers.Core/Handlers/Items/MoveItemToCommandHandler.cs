using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Enums;


namespace Weavers.Core.Handlers.Items {

  public record MoveItemToCommand(int ItemId, int NewParentId) : IRequest<ItemDto?>;

  public class MoveItemToCommandHandler : IRequestHandler<MoveItemToCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;

    public MoveItemToCommandHandler(FabricDbContext context, IMediator mediator) { 
      _context = context;
      _mediator = mediator;
    }

    public async Task<ItemDto?> Handle(MoveItemToCommand request, CancellationToken cancellationToken) {
      if (request.ItemId == request.NewParentId) {
        throw new ArgumentException("Item cannot be moved to itself.");
      }
      if (request.NewParentId == 0 || request.ItemId == 0) { 
        throw new ArgumentException("parameters cannot be zero.");
      }
      var item = await _context.GetItemDtoById(request.ItemId, cancellationToken);             
      if (item == null ) {
        throw new ArgumentException("Item not found.");
      }
      WeItemType itemType = (WeItemType)item.ItemTypeId;
      var validParentTypesByItem = itemType.GetValidParentTypesByItem();

      var newParent = await _context.GetItemDtoById(request.NewParentId, cancellationToken);
      if (newParent == null) {
        throw new ArgumentException("New parent not found.");
      }
      if (!validParentTypesByItem.Contains((WeItemType)newParent.ItemTypeId)) {
        throw new ArgumentException("Invalid parent type.");
      }

      var linkToParent = item.IncomingRelations.Where(r => validParentTypesByItem.Contains((WeItemType)r.ItemTypeId));
      var relation = linkToParent.FirstOrDefault();
      if (relation == null) {
        throw new ArgumentException("No valid parent relation found for the item.");
      }
      
      var updateRelationCmd = new UpdateRelationCommand(
        relation.Id, 
        request.NewParentId, 
        relation.RelationTypeId, 
        relation.RelatedItemId ?? 0, 
        relation.Rank);

      await _mediator.Send(updateRelationCmd, cancellationToken);

      var resultItem = await _context.GetItemDtoById(request.ItemId, cancellationToken);
      
      return resultItem;
    }
  }
}
