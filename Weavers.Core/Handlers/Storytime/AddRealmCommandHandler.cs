using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Storytime {

  public record AddRealmCommand(int folderId, string name, string details, string tone) : IMcpRequest, IRequest<ItemDto?>;
  public class AddRealmCommandHandler : IRequestHandler<AddRealmCommand, ItemDto?> {
    private readonly IMediator _mediator;  
    private readonly FabricDbContext _context;
    public AddRealmCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AddRealmCommand request, CancellationToken cancellationToken) {

      var parentItem = await _context.GetItemDtoById(request.folderId);
      if (parentItem == null) { throw new Exception($"Parent item with id {request.folderId} not found"); }
      if ((parentItem.ItemTypeId != (int)WeItemType.OrganizationModel)
          && (parentItem.ItemTypeId != (int)WeItemType.ProjectFolderModel)
          && (parentItem.ItemTypeId != (int)WeItemType.RelativeFolderModel)
        ) {
        throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId} valid types are {WeItemType.OrganizationModel}, {WeItemType.ProjectFolderModel}, {WeItemType.RelativeFolderModel}");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.RealmModel, request.name, request.details, "{}"));

      if (newItem != null) {
        var toneProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTone);
        if (toneProp != null) {
          toneProp.Value = request.tone;
          await toneProp.SaveProp(newItem, _mediator);
        }
      }
      return newItem;

    }
  }
}
