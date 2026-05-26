using MediatR;
using System.Diagnostics;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Builds {
  public record ShellCompileSolutionCommand(int SolutionItemId, BuildContext buildContext) : IRequest<BuildContext>;

  public class ShellCompileSolutionCommandHandler : IRequestHandler<ShellCompileSolutionCommand, BuildContext> {
    private readonly FabricDbContext _context;
    public ShellCompileSolutionCommandHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<BuildContext> Handle(ShellCompileSolutionCommand request, CancellationToken cancellationToken) {
      var bc = request.buildContext;
      if (request.SolutionItemId <= 0) { return bc.Fail("Invalid item id."); }
      var item = await _context.GetItemDtoById(request.SolutionItemId);
      if (item == null) { 
        return bc.Fail("Item not found."); 
      }
      if ((WeItemType)item.ItemTypeId != WeItemType.SolutionModel) {
        return bc.Fail($"Item {item.Name} at {request.SolutionItemId} not a Solution.");
      }            
      
      string fileNamePath = item.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? "";
      if (fileNamePath == "") { return bc.Fail("Item path property not found."); }      
      if (!File.Exists(fileNamePath)) return bc.Fail("File not found at path: " + fileNamePath);

      var psi = new ProcessStartInfo {
        FileName = "dotnet",
        Arguments = $"build \"{fileNamePath}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = false
      };
      try { 
        using var process = new Process { StartInfo = psi };
        if (!process.Start())
          return bc.Fail("Failed to start dotnet process.");

        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var output = await outputTask;
        var error = await errorTask;
        var combined = string.IsNullOrEmpty(error) ? output : output + "\n" + error;
        bc.Success = process.ExitCode == 0;
        bc.ShellOutput = combined;
        return bc;
      } catch (Exception ex) { 
        return bc.Fail("Error starting process: " + ex.Message); 
      }
    }
  }


}
