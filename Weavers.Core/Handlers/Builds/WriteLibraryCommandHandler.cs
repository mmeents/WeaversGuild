using MediatR;
using System;
using System.Text;
using System.Collections.Concurrent;
using System.Text.Json;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Templates;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Builds {
  
  public record WriteLibraryCommand(int LibraryItemId, bool ForceWrite) : IRequest<BuildContext> { }

  public class WriteLibraryCommandHandler : IRequestHandler<WriteLibraryCommand, BuildContext?> {
    public readonly FabricDbContext _context;
    public readonly IMediator _mediator;
    private HashSet<WeItemType> _generatableTypes = WeItemTypeExtensions.GetGenerativeTypes();
    public WriteLibraryCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<BuildContext?> Handle(WriteLibraryCommand request, CancellationToken cancellationToken) {

      var buildContext = new BuildContext {
        ForceWrite = request.ForceWrite
      };
      var libraryItem = await _context.GetLibraryItemDtoById(request.LibraryItemId);
      if (libraryItem == null) return buildContext.Fail($" LibraryId:{request.LibraryItemId} item not found");      
            
      BuildDto? lastBuild = null;
      if (libraryItem.Builds.Any()) {
        lastBuild = libraryItem.Builds.OrderByDescending(b => b.StartedAt).FirstOrDefault();
        if (lastBuild != null && lastBuild.BuildStatus == BuildStatus.InProgress) {
          return buildContext.Fail($" LibraryId:{request.LibraryItemId} has build in progress.");
        }
      }

      var started = DateTime.UtcNow;
      var build = new Build {
        LibraryItemId = request.LibraryItemId,
        StartedAt = started,
        Status = (int)BuildStatus.Pending
      };
      _context.Builds.Add(build);
      await _context.SaveChangesAsync();

      buildContext = new BuildContext {
        BuildId = build.Id,
        ForceWrite = request.ForceWrite
      };

      buildContext = await WalkAnWrite(libraryItem.Id, build.Id, buildContext, cancellationToken);

      if (lastBuild != null && buildContext.Success) {

        // cleanup of previous build files that are no longer relevant.
        foreach (var buildFile in lastBuild.BuildFiles) {
          if (!buildContext.LibItems.Keys.Contains(buildFile.ItemId)) {

            var fileNamePath = buildFile.FilePath;
            if (File.Exists(fileNamePath)) {
              File.Delete(fileNamePath);
              buildContext.FilesRemoved++;
              await _context.MarkBuildFileRemoved(buildFile.Id, cancellationToken);
            }

          } else { // was there... check for differences in path, if different then remove the old named version.
            var bi = buildContext.LibItems[buildFile.ItemId];
            var biprop = bi.Properties.FirstOrDefault(p => p.Name == bi.ItemTypeId.GetFolderPropertyName());
            if (biprop != null) {
              if (buildFile.FilePath != (biprop.Value ?? "")) {
                var fileNamePath = buildFile.FilePath;
                if (File.Exists(fileNamePath)) {
                  File.Delete(fileNamePath);
                  buildContext.FilesRemoved++;
                  await _context.MarkBuildFileRemoved(buildFile.Id, cancellationToken);
                }
              }
            }              
          }
        }
      }

      // save status and output to build record.
      var result = buildContext.Ok($"{libraryItem.Name} build completed.");
      var sbOutput = new StringBuilder();
      sbOutput.AppendLine($"{DateTime.UtcNow} Utc, {DateTime.Now} Local, {libraryItem.Name} Sync to file system results.");
      foreach (var x in buildContext.Errors) {
        if (x!= null && x != "") {
          sbOutput.AppendLine($"Error: {x}");
        }        
      }
      foreach (var x in buildContext.Warnings) {
        if (x != null && x != "") {
          sbOutput.AppendLine($"Warning: {x}");
        }
      }
      sbOutput.AppendLine($"Written: {result.FilesWritten}");
      sbOutput.AppendLine($"Skipped: {result.FilesSkipped}");
      sbOutput.AppendLine($"Removed: {result.FilesRemoved}");
      build.BuildOutput = sbOutput.ToString();
      build.Status = (int)(buildContext.Success ? BuildStatus.Success : BuildStatus.Failed);
      await _context.SaveChangesAsync();

      return result;
    }


    private async Task<BuildContext> WalkAnWrite(
      int itemId,
      int buildId, 
      BuildContext buildContext, 
      CancellationToken cancellationToken
    ) { 
      var item = await _context.GetItemDtoById(itemId);  
      if (item == null) return buildContext.Fail($"ItemId {itemId} is null");
      var folderPropName = item.ItemTypeId.GetFolderPropertyName();
      if (folderPropName == "" ) { return buildContext.Fail($"Item {item.Name} not writeable."); }
      buildContext.LibItems[item.Id] = item;
      var folderProp = item.Properties.FirstOrDefault(p => p.Name == folderPropName);
      if (folderProp == null) { return buildContext.Fail($"Item {item.Name} path property not found."); }
      if (folderProp.Value == "") { return buildContext.Fail($"Item {item.Name} path property is empty."); }
      try {

        string fileNamePath = folderProp.Value ?? "";
        if (fileNamePath == "") { return buildContext.Fail($"Item {item.Name} path property is empty."); }
        var path = Path.GetDirectoryName(fileNamePath);
        if (!Directory.Exists(path)) {
          Directory.CreateDirectory(path!);  // except if it fails.
        }

        if (_generatableTypes.Contains((WeItemType)item.ItemTypeId)) {  // namespaces are not generatable.
          if (buildContext.ForceWrite || item.WrittenAt == null || item.Established > item.WrittenAt) {
            var bc = await _mediator.Send(new GenerateWriteItemByTypeCommand(item.Id, buildId, buildContext));
            buildContext.FilesWritten++;
          } else {
            buildContext.FilesSkipped++;
            buildContext.Warnings.Add($"Item {item.Name} path property is up to date.");
          }
        }

        var childIds = item.Relations
          .Where(r => r.RelatedItemId.HasValue && r.RelatedItemTypeId.IsLibraryType())
          .Select(r => r.RelatedItemId!.Value)
          .ToList();

        foreach (var childId in childIds) {
          await WalkAnWrite(childId, buildId, buildContext, cancellationToken);
        }

      } catch (Exception ex){
        return buildContext.Fail($"exception: {ex.Message}");
      }
      return buildContext.Ok($"{item.Name} completed.");
    }  

  }



  public class BuildContext {
    public bool Success { get; set; } = false;
    public int BuildId { get; init; }
    public bool ForceWrite { get; init; } = false;
    public ConcurrentDictionary<int, ItemDto> LibItems { get; init; } = new ConcurrentDictionary<int, ItemDto>();
    public List<string> Errors { get; } = new List<string>();
    public List<string> Warnings { get; } = new List<string>();
    public string ShellOutput { get; set; } = string.Empty;
    public int FilesWritten { get; set; } = 0;
    public int FilesSkipped { get; set; } = 0; 
    public int FilesRemoved { get; set; } = 0;

    public BuildContext Fail(string errorMessage) {      
      Errors.Add(errorMessage);
      return this;
    }

    public BuildContext Ok(string warningMessage) {
      Success = true;
      Warnings.Add(warningMessage);
      return this;
    }
  }
  
}
