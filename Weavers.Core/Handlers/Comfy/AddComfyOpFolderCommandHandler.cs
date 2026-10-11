using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Comfy {
  public record AddComfyOpFolderCommand(int parentFolderId, string folderName) : IRequest<ItemDto?>;
  public class AddComfyOpFolderCommandHandler : IRequestHandler<AddComfyOpFolderCommand, ItemDto?> {
    private readonly FabricDbContext _dbContext;
    private readonly IMediator _mediator;
    public AddComfyOpFolderCommandHandler(FabricDbContext dbContext, IMediator mediator) {
      _dbContext = dbContext;
      _mediator = mediator;
    }
    public async Task<ItemDto?> Handle(AddComfyOpFolderCommand request, CancellationToken cancellationToken) {
      // Implementation for adding a ComfyOp folder goes here
      var parentItem = await _dbContext.GetItemDtoById(request.parentFolderId, cancellationToken);
      if (parentItem == null) {
        throw new ArgumentException($"Parent folder with ID {request.parentFolderId} does not exist.");
      }
      if (!parentItem.ItemTypeId.IsFolderType()) { // Assuming 1 is the ID for folder type
        throw new ArgumentException($"Parent item with ID {request.parentFolderId} is not a folder type.");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.ComfyOperationsModel, "ComfyOperations", "", "{}"), cancellationToken);

      return newItem;
    }



// end of class and namespace below.
  }
}
