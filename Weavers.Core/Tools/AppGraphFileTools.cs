using MCPSharp;
using System.ComponentModel;
using Weavers.Core.Constants;
using Weavers.Core.Service;

namespace Weavers.Core.Tools {
  
  public class AppGraphFileTools {
    private static IAppGraphFileToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphFileToolsHandler>();

    [McpTool(Cx.CmdAddProjectRoot, "Adds a new project root to the graph.")]
    public static Task<string> AddProjectRoot(
        [Description("The name of the new project root folder. Its result location is a child of the Organization root.")] string projectName)
        => GetTools().AddProjectRoot(projectName);

    [McpTool(Cx.CmdAddSubFolder, "Adds a new project subfolder to the specified parent folder or project root.")]
    public static Task<string> AddSubFolder(
      [Description("The Item Id of the parent folder or project root to add the subfolder to.")] int folderItemId,
      [Description("The name of the new subfolder.")] string subFolderName)
      => GetTools().AddSubFolder(folderItemId, subFolderName);

    [McpTool(Cx.CmdAddSolution, "Adds a new solution item under the specified folder.")]
    public static Task<string> AddSolution(
      [Description("The Item Id of the folder to add the solution under.")] int folderItemId,
      [Description("The name of the new solution.")] string solutionName)
      => GetTools().AddSolution(folderItemId, solutionName);

    [McpTool(Cx.CmdAddSolutionImport, "Adds a new solution import relation to the specified solution item.")]
    public static Task<string> AddSolutionImport(
      [Description("The Item Id of the solution to add the import to.")] int solutionItemId,
      [Description("The Item Id of the library (LibraryModel 1200) to import into the solution.")] int importLibraryId)
      => GetTools().AddSolutionImport(solutionItemId, importLibraryId);

    [McpTool(Cx.CmdAddMdFile, "Adds a new .md file item in the specified folder item, infra adds ext to name.")]
    public static Task<string> AddMdFile(
      [Description("The Item Id of the folder to add the file in.")] int folderItemId,
      [Description("The file name without extension; infra adds the .md extension.")] string fileName,
      [Description("The markdown content of the file.")] string fileContent)
      => GetTools().AddMdFile(folderItemId, fileName, fileContent);

    [McpTool(Cx.CmdAddHtmlFile, "Adds a new .html file item in the specified folder item, infra adds ext to name.")]
    public static Task<string> AddHtmlFile(
      [Description("The Item Id of the folder to add the file in.")] int folderItemId,
      [Description("The file name without extension; infra adds the .html extension.")] string fileName,
      [Description("The HTML content of the file.")] string fileContent)
      => GetTools().AddHtmlFile(folderItemId, fileName, fileContent);

    [McpTool(Cx.CmdAddConfigFile, "Adds a new .json file item in the specified folder item, infra adds ext to name.")]
    public static Task<string> AddConfigFile(
      [Description("The Item Id of the folder to add the file in.")] int folderItemId,
      [Description("The file name without extension; infra adds the .json extension.")] string fileName,
      [Description("The JSON content of the file.")] string fileContent)
      => GetTools().AddConfigFile(folderItemId, fileName, fileContent);


  }
}
