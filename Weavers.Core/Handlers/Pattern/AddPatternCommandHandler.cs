using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Pattern {
  public record AddPatternCommand(int folderId, string name) : IMcpRequest, IRequest<ItemDto?>;
  public class AddPatternCommandHandler : IRequestHandler<AddPatternCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddPatternCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AddPatternCommand request, CancellationToken cancellationToken) {

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
          (int)WeItemType.PatternModel, request.name, "", "{}"));
      
      return newItem;
    }
  }
}
