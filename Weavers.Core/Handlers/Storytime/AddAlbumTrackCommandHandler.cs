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
  public record AddAlbumTrackCommand(int parentItemId, string name, string description) : IMcpRequest, IRequest<ItemDto?>;
  public class AddAlbumTrackCommandHandler : IRequestHandler<AddAlbumTrackCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;

    public AddAlbumTrackCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<ItemDto?> Handle(AddAlbumTrackCommand request, CancellationToken cancellationToken) {
      var albumItem = await _context.GetItemDtoById(request.parentItemId);
      if (albumItem == null) { throw new Exception($"Parent album with id {request.parentItemId} not found"); }
      if (albumItem.ItemTypeId != (int)WeItemType.MusicAlbumModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)albumItem.ItemTypeId}; requires a {WeItemType.MusicAlbumModel} type {(int)WeItemType.MusicAlbumModel} parent.");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(albumItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.AlbumTrackModel, request.name, request.description, "{}"));

      return newItem;
    }
  }

}
