using MCPSharp;
using Weavers.Core.Constants;
using Weavers.Core.Service;


namespace Weavers.Core.Tools {
  public class AppGraphOrgTools {
    private static IAppGraphOrgToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphOrgToolsHandler>();

    [McpTool(Cx.CmdAddOrgDeskRole, "Adds a new desk role to the specified Org desk roles item.")]
    public static Task<string> AddOrgDeskRole(int orgDeskRolesId, string? roleName)
      => GetTools().AddOrgDeskRole(orgDeskRolesId, roleName);

    [McpTool(Cx.CmdAddOrgDesk, "Adds a new Org desk to the specified Org chart.")]
    public static Task<string> AddOrgDesk(int orgChartId, string deskName)
      => GetTools().AddOrgDesk(orgChartId, deskName);

    [McpTool(Cx.CmdAddDeskTodo, "Adds a new Todo to the specified Org desk.")]
    public static Task<string> AddDeskTodo(int orgDeskId, string todoName, int refId, string promptTemplate)
      => GetTools().AddDeskTodo(orgDeskId, todoName, refId, promptTemplate);


    [McpTool(Cx.CmdAddDigitalOperator, "Adds a digital operator to the specified DigitalOperatorPoolModel parentItem.")]
    public static Task<string> AddDigitalOperator(int parentItemId, string operatorName)
      => GetTools().AddDigitalOperator(parentItemId, operatorName);

    [McpTool(Cx.CmdAddOrgFolder, "Adds a new Org folder to the specified parent Org folder.")]
    public static Task<string> AddOrgFolder(int parentItemId, string subFolderName)
      => GetTools().AddOrgFolder(parentItemId, subFolderName);

    [McpTool(Cx.CmdAddOrgFile, "Adds a new .md file item in the specified Org folder item, infra adds ext to name.")]
    public static Task<string> AddOrgFile(int folderItemId, string fileName, string fileContent)
      => GetTools().AddOrgFile(folderItemId, fileName, fileContent);
  }
}
