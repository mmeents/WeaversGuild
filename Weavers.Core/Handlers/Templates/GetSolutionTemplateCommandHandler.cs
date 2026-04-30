using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;

namespace Weavers.Core.Handlers.Templates {
  public record GetSolutionTemplateCommand( int solutionItemId ) : IRequest<string?>;
  public class GetSolutionTemplateCommandHandler : IRequestHandler<GetSolutionTemplateCommand, string?> {
    private readonly FabricDbContext _context;
    public GetSolutionTemplateCommandHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<string?> Handle(GetSolutionTemplateCommand request, CancellationToken cancellationToken) {
      

      var item = await _context.GetItemDtoById(request.solutionItemId, cancellationToken);
      if (item == null || item.ItemTypeId != (int)WeItemType.SolutionModel ) return null;

      const string AppProjectWellKnownGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
      var slnGuidProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItSolutionGuid);
      var SolutionGuid = slnGuidProp != null && !string.IsNullOrEmpty(slnGuidProp.Value) ? slnGuidProp.Value : Guid.NewGuid().ToString("B").ToUpper();

      var solutionFilePathProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (solutionFilePathProp == null || string.IsNullOrEmpty(solutionFilePathProp.Value)) return null;
      var solutionDir = Path.GetDirectoryName(solutionFilePathProp.Value);
      string solutionDir2 = solutionDir != null ? solutionDir : WeaverExt.AppProjectsPath;
      StringBuilder cSb = new StringBuilder();
      StringBuilder sbGlobal = new StringBuilder();
      cSb.Append("\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n# Visual Studio Version 17\r\nVisualStudioVersion = 17.14.36221.1\r\nMinimumVisualStudioVersion = 10.0.40219.1\r\n");


      var solutionImports = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.SolutionImportModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value).ToList();

      foreach(var importId in solutionImports) {
        var importItem = await _context.GetItemDtoById(importId, cancellationToken);
        if (importItem != null) {
          var LibraryProp = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRegisterObject);
          if (LibraryProp != null && !string.IsNullOrEmpty(LibraryProp.Value)) {
            if (int.TryParse(LibraryProp.Value, out var libItemId)) {
              var libItem = await _context.GetItemDtoById(libItemId, cancellationToken);
              if (libItem != null) {
                var LibPathProp = libItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
                if (LibPathProp != null && !string.IsNullOrEmpty(LibPathProp.Value)) {
                  var libPath = LibPathProp.Value;
                  var relativePath = "";
                  if (libPath.StartsWith(solutionDir2, StringComparison.OrdinalIgnoreCase)) {
                    relativePath = libPath.Substring(solutionDir2.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);                    
                  } else {
                    relativePath = libPath; // Return absolute path if it's outside the solution directory
                  } 
                  var libGuidProp = libItem.Properties.FirstOrDefault(p => p.Name == Cx.ItProjectGuid);
                  var libGuidStr = libGuidProp != null && !string.IsNullOrEmpty(libGuidProp.Value) ? libGuidProp.Value : Guid.NewGuid().ToString("B").ToUpper();

                  cSb.Append($"Project(\"{AppProjectWellKnownGuid}\") = \"{libItem.Name}\", \"{relativePath}\", \"{libGuidStr}\"\r\nEndProject\r\n");

                  sbGlobal.AppendLine($"\t\t{libGuidStr}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                  sbGlobal.AppendLine($"\t\t{libGuidStr}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                  sbGlobal.AppendLine($"\t\t{libGuidStr}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                  sbGlobal.AppendLine($"\t\t{libGuidStr}.Release|Any CPU.Build.0 = Release|Any CPU");

                }
              }
            }
          }
        }
      }        
      cSb.Append("Global\r\n\tGlobalSection(SolutionConfigurationPlatforms) = preSolution\r\n\t\tDebug|Any CPU = Debug|Any CPU\r\n\t\tRelease|Any CPU = Release|Any CPU\r\n\tEndGlobalSection\r\n\tGlobalSection(ProjectConfigurationPlatforms) = postSolution\r\n");
      cSb.Append(sbGlobal.ToString());
      cSb.Append("\tEndGlobalSection\r\n" 
        + "\tGlobalSection(SolutionProperties) = preSolution\r\n\t\tHideSolutionNode = FALSE\r\n\tEndGlobalSection\r\n\tGlobalSection(ExtensibilityGlobals) = postSolution\r\n" 
        + "\t\tSolutionGuid = "+ SolutionGuid + "\r\n\tEndGlobalSection");
      cSb.Append(  "EndGlobal\r\n");

      return cSb.ToString();
    }
  }
}
