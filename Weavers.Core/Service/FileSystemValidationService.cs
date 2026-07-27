using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Service {

  public interface IFileSystemValidationService {
    public Task<FileSystemFilters> GetFileSystemFilters(CancellationToken cancellationToken = default);
  }
  public class FileSystemValidationService : IFileSystemValidationService {
    private readonly IAppSettingService _appSettings;

    public FileSystemValidationService(IAppSettingService appSettings) {
      _appSettings = appSettings;
    }

    public async Task<FileSystemFilters> GetFileSystemFilters(CancellationToken cancellationToken = default) {
      var settings = await _appSettings.GetAllAsDictionaryAsync(cancellationToken);

      var blockedFoldersSetting = settings.GetValueOrDefault("FileSystem.BlockedFolders", "bin,obj,node_modules,.git,.vs,.vscode,packages,Debug,Release,TestResults,.idea");
      var blockedExtensionsSetting = settings.GetValueOrDefault("FileSystem.BlockedExtensions", ".dll,.exe,.pdb,.cache,.suo,.user,.tmp,.temp,.log,.bak");
      var allowedExtensionsSetting = settings.GetValueOrDefault("FileSystem.AllowedExtensions", ".cs,.csproj,.sln,.json,.xml,.md,.txt,.config,.yml,.yaml,.js,.html,.ts,.jsx,.css,.tsx");
      var blockedFilesSetting = settings.GetValueOrDefault("FileSystem.BlockedFiles", ".DS_Store,Thumbs.db,desktop.ini");

      var blockedFolders = new HashSet<string>(
        (blockedFoldersSetting ?? string.Empty)
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(f => f.Trim()),
        StringComparer.OrdinalIgnoreCase);

      var blockedExtensions = new HashSet<string>(
        (blockedExtensionsSetting ?? string.Empty)
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(ext => ext.StartsWith(".") ? ext.Trim() : "." + ext.Trim()),
        StringComparer.OrdinalIgnoreCase);

      var allowedExtensions = new HashSet<string>(
        (allowedExtensionsSetting ?? string.Empty)
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(ext => ext.StartsWith(".") ? ext.Trim() : "." + ext.Trim()),
        StringComparer.OrdinalIgnoreCase);

      var blockedFiles = new HashSet<string>(
        (blockedFilesSetting ?? string.Empty)
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(f => f.Trim()),
        StringComparer.OrdinalIgnoreCase);

      return new FileSystemFilters(
        blockedFolders,
        blockedExtensions,
        allowedExtensions,
        blockedFiles
      );
    }


  }

  public record FileSystemFilters(
    HashSet<string> BlockedFolders,
    HashSet<string> BlockedExtensions,
    HashSet<string> AllowedExtensions,
    HashSet<string> BlockedFiles
  );


}
