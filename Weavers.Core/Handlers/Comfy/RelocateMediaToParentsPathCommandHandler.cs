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
  public record RelocateMediaToParentsPathCommand(int MediaItemId) : IRequest<string>;

  public class RelocateMediaToParentsPathCommandHandler
      : IRequestHandler<RelocateMediaToParentsPathCommand, string> {

    private readonly IServiceScopeFactory _scopeFactory;
    public RelocateMediaToParentsPathCommandHandler(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task<string> Handle(RelocateMediaToParentsPathCommand request, CancellationToken ct) {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();

      // ---- Validate everything before touching the disk ----
      var mediaItem = await context.GetItemDtoById(request.MediaItemId, ct)
        ?? throw new InvalidOperationException($"Media item {request.MediaItemId} not found.");

      if (mediaItem.ItemTypeId != (int)WeItemType.ComfyMediaFileModel)
        throw new InvalidOperationException($"Item {request.MediaItemId} is not a media item.");

      var filePathProp = mediaItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)
        ?? throw new InvalidOperationException($"Media item {request.MediaItemId} has no file path property.");

      var existingPath = filePathProp.Value;
      if (string.IsNullOrEmpty(existingPath) || !File.Exists(existingPath))
        throw new FileNotFoundException($"Source file for media item {request.MediaItemId} not found.", existingPath);

      var parentPath = await mediator.Send(new GetParentItemPathPropertyQuery(mediaItem.Id), ct);
      if (string.IsNullOrEmpty(parentPath))
        throw new InvalidOperationException($"No parent path resolved for media item {request.MediaItemId}.");

      var baseName = mediaItem.Name.UrlSafe();
      if (string.IsNullOrWhiteSpace(baseName)) baseName = $"media-{mediaItem.Id}";
      var desiredPath = Path.Combine(parentPath, baseName + Path.GetExtension(existingPath));

      // Already there? Nothing to do.
      if (string.Equals(Path.GetFullPath(existingPath), Path.GetFullPath(desiredPath), StringComparison.OrdinalIgnoreCase))
        return existingPath;

      var newPath = GetAvailablePath(desiredPath);

      // ---- Side effects: move, then persist, compensate on failure ----
      await Task.Run(() => {
        Directory.CreateDirectory(parentPath);
        File.Move(existingPath, newPath);          // no overwrite
      }, ct);

      try {
        filePathProp.Value = newPath;
        await filePathProp.SaveProp(mediaItem, mediator);
      } catch {
        try { File.Move(newPath, existingPath); } catch { /* log: file stranded at newPath */ }
        throw;
      }

      return newPath;
    }

    private static string GetAvailablePath(string path) {
      if (!File.Exists(path)) return path;
      var dir = Path.GetDirectoryName(path)!;
      var name = Path.GetFileNameWithoutExtension(path);
      var ext = Path.GetExtension(path);
      for (int i = 2; ; i++) {
        var candidate = Path.Combine(dir, $"{name}-{i}{ext}");
        if (!File.Exists(candidate)) return candidate;
      }
    }
  }
}
