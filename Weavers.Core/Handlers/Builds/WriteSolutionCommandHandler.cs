using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Templates;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Builds {

  public record WriteSolutionCommand( int SolutionItemId, bool ForceWrite ) : IRequest<BuildContext>;

  public class WriteSolutionCommandHandler : IRequestHandler<WriteSolutionCommand, BuildContext> {
    readonly FabricDbContext _context;
    readonly IMediator _mediator;
    public WriteSolutionCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<BuildContext> Handle(WriteSolutionCommand request, CancellationToken cancellationToken) {
      var buildContext = new BuildContext {
        ForceWrite = request.ForceWrite
      };

      var solutionItem = await _context.GetItemDtoById(request.SolutionItemId, cancellationToken);
      if (solutionItem == null) {
        return buildContext.Fail($"Solution item with ID {request.SolutionItemId} not found.");
      }

      var solutionFilePath = solutionItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value;
      if (solutionFilePath == null) { 
        return buildContext.Fail($"Solution item with ID {request.SolutionItemId} does not have a file path.");
      }

      //confirm folder exists and is writable
      try {
        var folder = Path.GetDirectoryName(solutionFilePath);
        if (folder == null) { return buildContext.Fail($"Get Path returned null for solution file path."); }
        if (!Directory.Exists(folder)) {
          Directory.CreateDirectory(folder);
          if (!Directory.Exists(folder)) {
            return buildContext.Fail($"Failed to create directory for solution file at path: {folder}");
          }
        } 
      } catch (Exception ex) { 
        return buildContext.Fail($"Error accessing solution file path: {ex.Message}");
      }

      bool shouldWrite = buildContext.ForceWrite || !File.Exists(solutionFilePath) 
        || solutionItem.WrittenAt == null || solutionItem.Established > solutionItem.WrittenAt;

      if (shouldWrite) {

        var solutionContent = await _mediator.Send(new GetSolutionTemplateCommand(request.SolutionItemId));
        if (solutionContent != null) {
          try {
            if (File.Exists(solutionFilePath)) {
              File.Delete(solutionFilePath);
            }
            await File.WriteAllTextAsync(solutionFilePath, solutionContent, cancellationToken);
            await _mediator.Send(new MarkItemWrittenCommand(solutionItem.Id), cancellationToken);
            buildContext.FilesWritten++;
            buildContext.LibItems[solutionItem.Id] = solutionItem;
          } catch (Exception ex) {
            return buildContext.Fail($"Error writing solution file: {ex.Message}");
          }
        } else { 
          return buildContext.Fail($"Failed to generate content for solution item with ID {request.SolutionItemId}.");
        }
      } else {
        buildContext.FilesSkipped++;
        buildContext.Warnings.Add("Skipped writing solution file, it is up to date and ForceWrite is false.");
      }

      var slnImportItemId = solutionItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.SolutionImportModel && r.RelatedItemId.HasValue)
        .Select (r => r.RelatedItemId!.Value)
        .ToList();

      foreach (var SlnImportId in slnImportItemId) {
        var slnImportItem = await _context.GetItemDtoById(SlnImportId, cancellationToken);
        if (slnImportItem != null) {
          var libId = slnImportItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRegisterObject)?.Value;
          if (libId == null) {
            buildContext.Warnings.Add($"Solution import item with ID {SlnImportId} does not have a library reference.");
            continue;
          } else {

            if (int.TryParse(libId, out int libItemId)) {
              var libBuildContext = await _mediator.Send(new WriteLibraryCommand(libItemId, request.ForceWrite), cancellationToken);
              if (libBuildContext != null) {
                buildContext.FilesWritten += libBuildContext.FilesWritten;
                buildContext.FilesSkipped += libBuildContext.FilesSkipped;
                buildContext.Errors.AddRange(libBuildContext.Errors);
                buildContext.Warnings.AddRange(libBuildContext.Warnings);
                foreach (var libItem in libBuildContext.LibItems) {
                  buildContext.LibItems[libItem.Key] = libItem.Value;
                }
              } else {
                buildContext.Warnings.Add($"Failed to build library with ID {libItemId} for solution import with ID {SlnImportId}.");
              }
            } else {
              buildContext.Warnings.Add($"Invalid library reference '{libId}' in solution import item with ID {SlnImportId}.");
            }

          }
        } else {
          buildContext.Warnings.Add($"Related solution import item with ID {SlnImportId} not found for solution item with ID {request.SolutionItemId}.");
        }
      }
      buildContext.Success = (buildContext.Errors.Count == 0);

      if (!buildContext.Success) {
        return buildContext;
      }

      var cmd = new ShellCompileSolutionCommand(request.SolutionItemId, buildContext);
      var compileBc = await _mediator.Send(cmd, cancellationToken);      
      buildContext.ShellOutput = compileBc.ShellOutput;
      return buildContext;
    }



  }
}
