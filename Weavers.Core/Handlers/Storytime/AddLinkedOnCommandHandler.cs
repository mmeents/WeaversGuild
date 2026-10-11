using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Constants;

namespace Weavers.Core.Handlers.Storytime {

  public record AddLinkedOnCommand(int parentItemId, string linkName, string linkUrl, string linkEmbed, string linkIconUrl) : IMcpRequest, IRequest<ItemDto?>;
  public class AddLinkedOnCommandHandler : IRequestHandler<AddLinkedOnCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;

    public AddLinkedOnCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<ItemDto?> Handle(AddLinkedOnCommand request, CancellationToken cancellationToken) {
      var parentItem = await _context.GetItemDtoById(request.parentItemId);
      if (parentItem == null) { throw new Exception($"Parent item with id {request.parentItemId} not found"); }
      if (!parentItem.ItemTypeId.IsLinkedOnParent()) {
        throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId}; requires a valid linked-on parent type. (Realm, Storyrollup, Album or Track)");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.LinkedOnModel, request.linkName, "", "{}"));

      if (newItem == null) { 
        throw new Exception($"Failed to create new linked-on item for parent item with id {request.parentItemId}"); 
      }

      var itPublishedProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPublishedUrl);
      if (itPublishedProp != null) {        
        itPublishedProp.Value = request.linkUrl;
        await itPublishedProp.SaveProp(newItem, _mediator);
      }

      var itLinkAsEmbedProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLinkAsEmbed);
      if (itLinkAsEmbedProp != null) {
        itLinkAsEmbedProp.Value = request.linkEmbed;
        await itLinkAsEmbedProp.SaveProp(newItem, _mediator);
      }

      var itLinkIconUrlProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLinkIconUrl);
      if (itLinkIconUrlProp != null) {
        itLinkIconUrlProp.Value = request.linkIconUrl;
        await itLinkIconUrlProp.SaveProp(newItem, _mediator);
      }

      newItem = await _context.GetItemDtoById(newItem.Id);

      return newItem;
    }
  }
}
