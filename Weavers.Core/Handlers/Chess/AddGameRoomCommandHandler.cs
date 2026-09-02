using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Chess {
  public record AddGameRoomCommand(int ParentId, string Name) : IMcpRequest, IRequest<ItemDto?> {
  }


  public class AddGameRoomCommandHandler : IRequestHandler<AddGameRoomCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddGameRoomCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<ItemDto?> Handle(AddGameRoomCommand request, CancellationToken cancellationToken) {
      var parentItem = await _context.GetItemDtoById(request.ParentId);
      if (parentItem == null) { throw new Exception($"Parent item with id {request.ParentId} not found"); }
      if ((parentItem.ItemTypeId != (int)WeItemType.OrganizationModel)
          && (parentItem.ItemTypeId != (int)WeItemType.ProjectFolderModel)
          && (parentItem.ItemTypeId != (int)WeItemType.RelativeFolderModel)
        ) {
        throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId} valid types are {WeItemType.OrganizationModel}, {WeItemType.ProjectFolderModel}, {WeItemType.RelativeFolderModel}");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.GameRoomModel, request.Name, "", "{}"));

      if (newItem == null) {
        return null;
      }

      return newItem;
    }

  }
}
