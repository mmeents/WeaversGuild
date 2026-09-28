using MCPSharp;
using System.ComponentModel;
using Weavers.Core.Constants;
using Weavers.Core.Service;

namespace Weavers.Core.Tools {
  
  public class AppGraphFileTools {
    private static IAppGraphFileToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphFileToolsHandler>();

    [McpTool(Cx.CmdAddProjectRoot, Cx.CmdAddProjectRootDesc)]
    public static Task<string> AddProjectRoot(
        [Description("The name of the new project root folder. Its result location is a child of the Organization root.")] string projectName)
        => GetTools().AddProjectRoot(projectName);

    [McpTool(Cx.CmdAddSubFolder, Cx.CmdAddSubFolderDesc)]
    public static Task<string> AddSubFolder(
      [Description("The Item Id of the parent folder or project root to add the subfolder to.")] int folderItemId,
      [Description("The name of the new subfolder.")] string subFolderName)
      => GetTools().AddSubFolder(folderItemId, subFolderName);


        
    [McpTool(Cx.CmdAddGithubRepo, Cx.CmdAddGithubRepoDesc)]
    public static Task<string> AddGithubRepo(
      [Description("The Item Id of the folder to add the GitHub repository item to.")] int folderItemId,
      [Description("The URL of the GitHub repository.")] string repoUrl)
      => GetTools().AddGithubRepo(folderItemId, repoUrl);

    [McpTool(Cx.CmdDoGitClone, Cx.CmdDoGitCloneDesc)]
    public static Task<string> DoGitClone(
      [Description("The Item Id of the GitHub repository item to clone.")] int repoItemId)
      => GetTools().DoCloneGithubRepoItem(repoItemId);

    [McpTool(Cx.CmdDoGitRefreshStatus, Cx.CmdDoGitRefreshStatusDesc)]
    public static Task<string> DoGitRefreshStatus(
      [Description("The Item Id of the GitHub repository item to refresh the status for.")] int repoItemId)
      => GetTools().DoGitRefreshStatus(repoItemId);

    [McpTool(Cx.CmdDoGitCheckout, Cx.CmdDoGitCheckoutDesc)]
    public static Task<string> DoGitCheckout(
      [Description("The Item Id of the repository branch item to checkout.")] int branchItemId)
      => GetTools().DoCheckoutBranch(branchItemId);



    [McpTool(Cx.CmdAddSolution, Cx.CmdAddSolutionDesc)]
    public static Task<string> AddSolution(
      [Description("The Item Id of the folder to add the solution under.")] int folderItemId,
      [Description("The name of the new solution.")] string solutionName)
      => GetTools().AddSolution(folderItemId, solutionName);

    [McpTool(Cx.CmdAddSolutionImport, Cx.CmdAddSolutionImportDesc)]
    public static Task<string> AddSolutionImport(
      [Description("The Item Id of the solution to add the import to.")] int solutionItemId,
      [Description("The Item Id of the library (LibraryModel 1200) to import into the solution.")] int importLibraryId)
      => GetTools().AddSolutionImport(solutionItemId, importLibraryId);

    [McpTool(Cx.CmdAddMdFile, Cx.CmdAddMdFileDesc)]
    public static Task<string> AddMdFile(
      [Description("The Item Id of the folder to add the file in.")] int folderItemId,
      [Description("The file name without extension; infra adds the .md extension.")] string fileName,
      [Description("The markdown content of the file.")] string fileContent)
      => GetTools().AddMdFile(folderItemId, fileName, fileContent);

    [McpTool(Cx.CmdAddHtmlFile, Cx.CmdAddHtmlFileDesc)]
    public static Task<string> AddHtmlFile(
      [Description("The Item Id of the folder to add the file in.")] int folderItemId,
      [Description("The file name without extension; infra adds the .html extension.")] string fileName,
      [Description("The HTML content of the file.")] string fileContent)
      => GetTools().AddHtmlFile(folderItemId, fileName, fileContent);

    [McpTool(Cx.CmdAddConfigFile, Cx.CmdAddConfigFileDesc)]
    public static Task<string> AddConfigFile(
      [Description("The Item Id of the folder to add the file in.")] int folderItemId,
      [Description("The file name without extension; infra adds the .json extension.")] string fileName,
      [Description("The JSON content of the file.")] string fileContent)
      => GetTools().AddConfigFile(folderItemId, fileName, fileContent);


  }
}
