using MediatR;
using Weavers.Core.Tools;
using Weavers.Core.Constants;



namespace Weavers.Api.Extensions {
  public static class GraphFiles {

    public static WebApplication MapGraphFileEndpoints(this WebApplication app) {

      var group = app.MapGroup("/api/graph/").WithTags("Graph Folders and Files");

      group.MapPost("addProjectRoot", async (IAppGraphFileToolsHandler handler, string projectRootName) => {
        try {
          var result = await handler.AddProjectRoot(projectRootName);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error adding project root: {ex.Message}");
          return Results.BadRequest("Failed to add project root.");
        }
      }).WithName("AddProjectRoot").WithDescription("Adds a new project root to the graph. is considered a folder.");


      group.MapPost("addSubFolder", async (IAppGraphFileToolsHandler handler, string parentFolderId, string subFolderName) => {
        try {
          if (!int.TryParse(parentFolderId, out int parentId)) {
            return Results.BadRequest("Invalid parent folder ID.");
          }
          var result = await handler.AddSubFolder(parentId, subFolderName);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error adding subfolder: {ex.Message}");
          return Results.BadRequest("Failed to add subfolder.");
        }
      }).WithName("AddSubFolder").WithDescription("Adds a new subfolder under the specified parent folder.");


      group.MapPost("addSolution", async (IAppGraphFileToolsHandler handler, string parentFolderId, string solutionName) => {
        try {
          if (!int.TryParse(parentFolderId, out int parentId)) {
            return Results.BadRequest("Invalid parent folder ID.");
          }
          var result = await handler.AddSolution(parentId, solutionName);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error adding solution: {ex.Message}");
          return Results.BadRequest("Failed to add solution.");
        }
      }).WithName("AddSolution").WithDescription("Adds a new solution under the specified parent folder.");


      group.MapPost("addSolutionImport", async (IAppGraphFileToolsHandler handler, string solutionId, string importLibraryId) => {
        try {
          if (!int.TryParse(solutionId, out int solId) || !int.TryParse(importLibraryId, out int libId)) {
            return Results.BadRequest("Invalid solution ID or import library ID.");
          }
          var result = await handler.AddSolutionImport(solId, libId);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error adding solution import: {ex.Message}");
          return Results.BadRequest("Failed to add solution import.");
        }
      }).WithName("AddSolutionImport").WithDescription("Adds an import library to the specified solution.");


      group.MapPost("addFile", async (IAppGraphFileToolsHandler handler, string parentFolderId, string fileName, string fileContent) => {
        try {
          if (!int.TryParse(parentFolderId, out int parentId)) {
            return Results.BadRequest("Invalid parent folder ID.");
          }
          var result = await handler.AddFile(parentId, fileName, fileContent);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error adding file: {ex.Message}");
          return Results.BadRequest("Failed to add file.");
        }
      }).WithName("AddFile").WithDescription("Adds a new file under the specified parent folder.");

      return app;

    }
  }
}
