using MCPSharp;
using System.ComponentModel;
using Weavers.Core.Constants;
using Weavers.Core.Service;


namespace Weavers.Core.Tools {
  public class AppGraphOrgTools {
    private static IAppGraphOrgToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphOrgToolsHandler>();

    [McpTool(Cx.CmdAddOrgDeskRole, "Adds a new desk role to the specified Org desk roles item. Note: Properties are empty by default.")]
    public static Task<string> AddOrgDeskRole(
         [Description("The Item Id of the OrgDeskRolesModel 1026 folder to add the role to.")] int orgDeskRolesId,
         [Description("The name of the new desk role.")] string? roleName)
         => GetTools().AddOrgDeskRole(orgDeskRolesId, roleName);

    [McpTool(Cx.CmdAddOrgDesk, "Adds a new Org desk to the specified Org chart. " +
      "Note: the desk's SystemPrompt and other Properties must be configured manually after the Add, " +
      "via the desk's SystemPrompt property using " + Cx.CmdUpdateItemProperty + ". " +
      "The SystemPrompt is a Scriban template rendered into the operator's instructions. " +
      "ex: {{ model.desk }} renders the desk name. Template model:\r\n" +
      "  desk - string, the desk name\r\n" +
      "  operator - string, the operator name\r\n" +
      "  role - string, the desk role name\r\n" +
      "  role_commands - list of:\r\n" +
      "    command_type - string\r\n" +
      "    command - string")]
    public static Task<string> AddOrgDesk(
  [Description("The Item Id of the OrgChartModel 1040 to add the desk to.")] int orgChartId,
  [Description("The name of the new desk.")] string deskName)
  => GetTools().AddOrgDesk(orgChartId, deskName);

    [McpTool(Cx.CmdAddDeskTodo, "Adds a new Todo to the specified Org desk.")]
    public static Task<string> AddDeskTodo(
      [Description("The Item Id of the Org desk to add the todo to.")] int orgDeskId,
      [Description("The name of the new todo.")] string todoName,
      [Description("The Item Id this todo references / operates on (e.g. the model or spec the work targets). Use 0 for none; target will be null in the template")] int refId,
      [Description("The prompt template rendered into the operator's instructions when the todo runs. " +
        "Follows Scriban syntax. (all references start with model) ex {{ model.todo.id }} gives the todo id number. {%{ }%} blocks escape template code. \r\nTodo Template model:\r\n" +
        "  todo - SummaryDto of the todo object\r\n" +
        "  target - SummaryDto of the refId target object")] string promptTemplate)
      => GetTools().AddDeskTodo(orgDeskId, todoName, refId, promptTemplate);

    [McpTool(Cx.CmdAddDigitalOperator, "Adds a digital operator to the specified DigitalOperatorPoolModel parentItem. Note: Properties need to be configured manually after the Add.")]
    public static Task<string> AddDigitalOperator(
      [Description("The Item Id of the DigitalOperatorPoolModel 1030 to add the operator to.")] int parentItemId,
      [Description("The name of the new digital operator.")] string operatorName)
      => GetTools().AddDigitalOperator(parentItemId, operatorName);

    [McpTool(Cx.CmdAddOrgFolder, "Adds a new Org folder to the specified parent Org folder.")]
    public static Task<string> AddOrgFolder(
      [Description("The Item Id of the parent Org folder to add the subfolder to.")] int parentItemId,
      [Description("The name of the new Org folder.")] string subFolderName)
      => GetTools().AddOrgFolder(parentItemId, subFolderName);

    [McpTool(Cx.CmdAddOrgFile, "Adds a new .md file item in the specified Org folder item, infra adds ext to name.")]
    public static Task<string> AddOrgFile(
      [Description("The Item Id of the Org folder to add the file in.")] int folderItemId,
      [Description("The file name without extension; infra adds the .md extension.")] string fileName,
      [Description("The markdown content of the file.")] string fileContent)
      => GetTools().AddOrgFile(folderItemId, fileName, fileContent);
  }
}
