using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Storytime {

  public record AddMusicAlbumCommand(int parentRealmId, string name, string description) : IMcpRequest, IRequest<ItemDto?>;
  public class AddMusicAlbumCommandHandler : IRequestHandler<AddMusicAlbumCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;

    public AddMusicAlbumCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<ItemDto?> Handle(AddMusicAlbumCommand request, CancellationToken cancellationToken) {
      var realmItem = await _context.GetItemDtoById(request.parentRealmId);
      if (realmItem == null) { throw new Exception($"Parent realm with id {request.parentRealmId} not found"); }
      if (realmItem.ItemTypeId != (int)WeItemType.RealmModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)realmItem.ItemTypeId}; requires a {WeItemType.MusicAlbumModel} type {(int)WeItemType.MusicAlbumModel} parent.");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(realmItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.MusicAlbumModel, request.name, request.description, "{}"));

      return newItem;
    }
  }
}
