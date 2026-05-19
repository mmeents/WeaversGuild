using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Enums;


namespace Weavers.Core.Handlers.Templates {

  public record GetLibraryTemplateCommand(int LibraryItemId) : IRequest<string>;

  public class GetLibraryTemplateCommandHandler : IRequestHandler<GetLibraryTemplateCommand, string?> {
    private readonly FabricDbContext _context;
    public GetLibraryTemplateCommandHandler(FabricDbContext context) { 
      _context = context;
    }

    public async Task<string?> Handle(GetLibraryTemplateCommand request, CancellationToken ct) {

      var item = await _context.GetItemDtoById(request.LibraryItemId, ct);
      if (item == null) return null;

      var targetFramework = item.Properties.FirstOrDefault(p => p.Name == Cx.ItTargetFramework)?.Value ?? "net9.0";
      var isTestLibrary = item.Properties.FirstOrDefault(p => p.Name == Cx.ItIsTestLibrary)?.Value.AsBoolean() ?? false;
      var ImplicitUsings = item.Properties.FirstOrDefault(p => p.Name == Cx.ItImplicitUsing)?.Value.AsBoolean() ?? true;
      var isNullable = item.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? true;
      var version = item.Properties.FirstOrDefault(p => p.Name == Cx.ItVersion)?.Value ?? "0.0.1";
      var namespaceName = item.ResolveItemsNamespace("MissingNamespace");
      var libraryName = item.Name;

      string sdk = isTestLibrary ? Cx.DefaultTestSDK : Cx.DefaultSDK;  // switch if test library.
      string implicitUsingsClause = ImplicitUsings ? "enable" : "disable";
      string nullableClause = isNullable ? "enable" : "disable";
      string repoUrl = "";

      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"<Project Sdk=\"{sdk}\">" + Environment.NewLine);
      sb.AppendLine( "  <PropertyGroup>");
      sb.AppendLine($"    <TargetFramework>{targetFramework}</TargetFramework >");
      sb.AppendLine($"    <ImplicitUsings>{implicitUsingsClause}</ImplicitUsings >");
      sb.AppendLine($"    <Nullable>{nullableClause}</Nullable>");
      sb.AppendLine($"    <Version>{version}</Version>");
      sb.AppendLine($"    <Title>{libraryName}</Title>");
      sb.AppendLine($"    <Authors>$(AssemblyName)</Authors>");      
      sb.AppendLine($"    <Copyright> Copyright © {DateTime.Now.Year} {Cx.AppName}</Copyright>");
      if (repoUrl != "") {
        sb.AppendLine($"    <RepositoryUrl>{repoUrl}</RepositoryUrl>");
      }
      if (isTestLibrary) {
        sb.AppendLine($"    <UseVSTest>true</UseVSTest>");
      }
      sb.AppendLine( "  </PropertyGroup>" + Environment.NewLine);

      var packageIdList = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.LibPackageRefModel)
        .Select(r => r.RelatedItemId).Where(i => i.HasValue).Select(i => i!.Value).ToList();
      if (packageIdList.Any()) {
        sb.AppendLine($"  <ItemGroup>");
        foreach (var packageId in packageIdList) { 
          var packageItem = await _context.GetItemDtoById(packageId, ct);
          if (packageItem != null) {
            var packageInclude = packageItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPackageInclude)?.Value ?? "";
            var packageVersion = packageItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPackageVersion)?.Value ?? "";
            var privateAssets = packageItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPrivateAssets)?.Value ?? "";
            var includeAssets = packageItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIncludeAssets)?.Value ?? "";
            string refLine = $"    <PackageReference Include=\"{packageInclude}\" Version=\"{packageVersion}\"";
            var hasBody = false;
            string bodyStr = string.Empty;
            if (privateAssets != null && privateAssets.Length > 0) {
              bodyStr = $"      <IncludeAssets>{privateAssets}</IncludeAssets>"+Environment.NewLine;
              hasBody = true;
            }
            if (includeAssets != null && includeAssets.Length > 0) {
              bodyStr += $"      <PrivateAssets>{includeAssets}</PrivateAssets>"+Environment.NewLine;
              hasBody = true;
            }
            if (hasBody) {              
              refLine += ">" + Environment.NewLine + bodyStr + "    </PackageReference>";
            } else {
              refLine += " />";
            }
            sb.AppendLine(refLine);
          }
        }
        sb.AppendLine($"  </ItemGroup>");
      }

      var libraryRefIdList = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.LibLibraryRefModel)
        .Select(r => r.RelatedItemId).Where(i => i.HasValue).Select(i => i!.Value).ToList();
      if (libraryRefIdList.Count > 0) {        
        StringBuilder projectRefsStr = new StringBuilder();  
        foreach (var packageId in libraryRefIdList) {
          var packageItem = await _context.GetItemDtoById(packageId, ct);
          if (packageItem != null) {
            var refInclude = packageItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLibraryInclude)?.Value ?? "";
            var refIncId = int.TryParse(refInclude, out var refout) ? refout : 0;
            if (refIncId > 0) { 
              var refItem = await _context.GetItemDtoById(refIncId, ct);
              if (refItem != null) { 
                var refProjectPath = refItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? "";
                if (refProjectPath != null) {
                  projectRefsStr.AppendLine($"    <ProjectReference Include=\"{refProjectPath}\" />");                  
                }
              }
            }
          }
        }
        var projectRefsStrVal = projectRefsStr.ToString();
        if (projectRefsStrVal.Length > 0 ) {
          sb.AppendLine($"  <ItemGroup>");
          sb.Append($"{projectRefsStr}");          
          sb.AppendLine($"  </ItemGroup>");
        }        
      }

      sb.AppendLine( "</Project>");

      return sb.ToString();

    }
  }
}
