using MCPSharp;
using Weavers.Core.Service;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  
  public class AppGraphFileTools {
    private static IAppGraphFileToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphFileToolsHandler>();

    [McpTool(Cx.CmdAddProjectRoot, "Adds a new project root to the graph.")]
    public static Task<string> AddProjectRoot( string projectName) 
      => GetTools().AddProjectRoot(projectName);
    
    [McpTool(Cx.CmdAddSubFolder, "Adds a new subfolder to the specified parent folder or project root.")]
    public static Task<string> AddSubFolder(int folderItemId, string subFolderName) 
      => GetTools().AddSubFolder(folderItemId, subFolderName);

    [McpTool(Cx.CmdAddSolution, "Adds a new solution item under the specified folder.")]
    public static Task<string> AddSolution(int folderItemId, string solutionName)
      => GetTools().AddSolution(folderItemId, solutionName);

    [McpTool(Cx.CmdAddSolutionImport, "Adds a new solution import relation to the specified solution item.")]
    public static Task<string> AddSolutionImport(int solutionItemId, int importLibraryId)
      => GetTools().AddSolutionImport(solutionItemId, importLibraryId);

    [McpTool(Cx.CmdAddMdFile, "Adds a new .md file item in the specified folder item, infra adds ext to name.")]
    public static Task<string> AddMdFile(int folderItemId, string fileName, string fileContent)
      => GetTools().AddMdFile(folderItemId, fileName, fileContent);

    [McpTool(Cx.CmdAddHtmlFile, "Adds a new .html file item in the specified folder item, infra adds ext to name.")]
    public static Task<string> AddHtmlFile(int folderItemId, string fileName, string fileContent)
      => GetTools().AddHtmlFile(folderItemId, fileName, fileContent);

    [McpTool(Cx.CmdAddConfigFile, "Adds a new .json file item in the specified folder item, infra adds ext to name.")]
    public static Task<string> AddConfigFile(int folderItemId, string fileName, string fileContent)
      => GetTools().AddConfigFile(folderItemId, fileName, fileContent);

  }
}
