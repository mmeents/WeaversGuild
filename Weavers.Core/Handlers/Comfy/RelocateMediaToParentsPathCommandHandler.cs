using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Constants;


namespace Weavers.Core.Handlers.Comfy {
  public record RelocateMediaToParentsPathCommand(int MediaItemId) : IRequest<bool>;
  public class RelocateMediaToParentsPathCommandHandler : IRequestHandler<RelocateMediaToParentsPathCommand, bool> {
    private readonly IServiceScopeFactory _scopeFactory;
    public RelocateMediaToParentsPathCommandHandler(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
    }
    public async Task<bool> Handle(RelocateMediaToParentsPathCommand request, CancellationToken cancellationToken) {

      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();

      var mediaItem = await context.GetItemDtoById(request.MediaItemId, cancellationToken);
      if (mediaItem == null) {
        throw new Exception($"Media item with ID {request.MediaItemId} not found.");
      }
      if (mediaItem.ItemTypeId != (int)WeItemType.ComfyMediaFileModel) { 
        throw new Exception($"Item with ID {request.MediaItemId} is not a media item.");
      }
      
      var parentPath = await mediator.Send(new GetParentItemPathPropertyQuery(mediaItem.Id), cancellationToken);
      if (string.IsNullOrEmpty(parentPath)) {
        throw new Exception($"Parent path for media item with ID {request.MediaItemId} not found.");
      }

      var existingPath = mediaItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value;
      if (string.IsNullOrEmpty(existingPath)) {
        throw new Exception($"Media item with ID {request.MediaItemId} does not have an existing file path.");
      } 

      var newName = mediaItem.Name.UrlSafe() + Path.GetExtension(existingPath);
      var newPath = Path.Combine(parentPath, newName);

      try {
        var newDirectory = Path.GetDirectoryName(newPath);
        if (newDirectory == null) {
          throw new Exception($"Failed to determine the directory for the new path '{newPath}'.");
        }
        if (!Directory.Exists(newDirectory)) {
          Directory.CreateDirectory(newDirectory);
        }
        // Move the file to the new path
        if (File.Exists(existingPath)) {
          File.Move(existingPath, newPath, true);
        } else {
          throw new Exception($"Existing file path '{existingPath}' does not exist.");
        }
        // Update the media item's file path property
        var filePathProperty = mediaItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (filePathProperty != null) {
          filePathProperty.Value = newPath;
          await filePathProperty.SaveProp(mediaItem, mediator);   
        } else {
          throw new Exception($"Media item with ID {request.MediaItemId} does not have a file path property.");
        }
        return true;
      } catch (Exception ex) {
        // Handle exceptions (e.g., log them)
        throw new Exception($"Error relocating media item with ID {request.MediaItemId}: {ex.Message}", ex);
      }


      throw new NotImplementedException();
    } 
  }
}
