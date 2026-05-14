using MCPSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Service;
using Weavers.Core.Constants;
using System.ComponentModel;

namespace Weavers.Core.Tools {
  
  public class AppGraphFileTools {
    private static IAppGraphFileToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphFileToolsHandler>();

    [McpTool(Cx.CmdAddProjectRoot, "Adds a new project root to the graph. is considered a folder.")]
    public static Task<string> AddProjectRoot( string projectRootName) 
      => GetTools().AddProjectRoot(projectRootName);
    
    [McpTool(Cx.CmdAddSubFolder, "Adds a new subfolder to the specified parent folder. is considered a folder.")]
    public static Task<string> AddSubFolder(int itemId, string subFolderName) 
      => GetTools().AddSubFolder(itemId, subFolderName);

    [McpTool(Cx.CmdAddSolution, "Adds a new solution item under the specified folder.")]
    public static Task<string> AddSolution(int folderItemId, string solutionName)
      => GetTools().AddSolution(folderItemId, solutionName);

    [McpTool(Cx.CmdAddSolutionImport, "Adds a new solution import relation to the specified solution item.")]
    public static Task<string> AddSolutionImport(int solutionItemId, int importLibraryId)
      => GetTools().AddSolutionImport(solutionItemId, importLibraryId);

    [McpTool(Cx.CmdAddFile, "Adds a new file item under the specified folder, adds default ext to name as filename. default is .md")]
    public static Task<string> AddFile(int folderItemId, string fileName, string fileContent)
      => GetTools().AddFile(folderItemId, fileName, fileContent);

  }
}
