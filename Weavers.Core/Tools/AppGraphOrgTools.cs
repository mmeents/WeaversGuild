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
  [Description("The Item Id of the WorkGroupModel 1040 to add the desk to.")] int orgChartId,
  [Description("The name of the new desk.")] string deskName)
  => GetTools().AddOrgDesk(orgChartId, deskName);

    [McpTool(Cx.CmdAddDeskTodo, "Adds a new Todo to the specified Org desk. Note: promptTemplate follows Scriban syntax. "+
      "model being passed in has both Todo and Target ItemSummaryDto objects. ex: {{ model.todo.id }} {{ model.target.name }} would "+
      "render todo id and target name.  ")]
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

    [McpTool(Cx.CmdAddRssFolder, "Adds a new Rss folder to the specified parent Rss folder or Organization folder.")]
    public static Task<string> AddRssFolder(
      [Description("The Item Id of the parent Rss folder or Organization folder to add the subfolder to.")] int parentItemId,
      [Description("The name of the new Rss folder.")] string subFolderName)
      => GetTools().AddRssFolder(parentItemId, subFolderName);

    [McpTool(Cx.CmdAddRssChannel, "Adds a new Rss channel to the specified Rss folder.")]
    public static Task<string> AddRssChannel(
      [Description("The Item Id of the Rss folder to add the channel to.")] int rssFolderId,
      [Description("The name of the new Rss channel.")] string channelName,
      [Description("The url of the new Rss channel.")] string channelUrl)
      => GetTools().AddRssChannel(rssFolderId, channelName, channelUrl);

    [McpTool(Cx.CmdRssResyncChannel, "Resyncs the specified Rss channel.")]
    public static Task<string> RssResyncChannel(
      [Description("The Item Id of the Rss channel to resync.")] int rssChannelId)
      => GetTools().RssResyncChannel(rssChannelId);

    [McpTool(Cx.CmdRssResolveLink, "Resolves the specified Rss link to an Org file.")]
    public static Task<string> RssResolveLink(
      [Description("The Item Id of the Rss linked html to resolve.")] int rssLinkedHtmlItemId)
      => GetTools().RssResolveLink(rssLinkedHtmlItemId);

    [McpTool(Cx.CmdRssExtractLinks, "Extracts links from the specified Rss link.")]
    public static Task<string> RssExtractLinks(
      [Description("The Item Id of the Rss linked html to extract links from.")] int rssLinkedHtmlItemId)
      => GetTools().RssExtractLinks(rssLinkedHtmlItemId);

    [McpTool(Cx.CmdAppendGuildNote, "Appends a note to the specified GuildNote property. works with item types RssLinkedHtmlModel, RssItemModel, RssChannelModel, RssFolderModel")]
    public static Task<string> AppendGuildNote(
      [Description("The Item Id of the rss item to append the note to.")] int rssItemId,
      [Description("The content of the note to append.")] string noteContent)
      => GetTools().AppendGuildNote(rssItemId, noteContent);

    [McpTool(Cx.CmdUpdateGuildNote, "Updates a note in the specified GuildNote property. works with item types RssLinkedHtmlModel, RssItemModel, RssChannelModel, RssFolderModel")]
    public static Task<string> UpdateGuildNote(
      [Description("The Item Id of the rss item to update the note in.")] int rssItemId,
      [Description("The new content of the note.")] string noteContent)
      => GetTools().UpdateGuildNote(rssItemId, noteContent);

    [McpTool(Cx.CmdArchiveItem, "Archives the specified item, only items with type: TodoModel, TodoAttemptModel, RssLinkedHtmlModel, RssItemModel")]
    public static Task<string> ArchiveItem(
      [Description("The Item Id of the item to archive.")] int itemId)
      => GetTools().ArchiveItem(itemId);

    [McpTool(Cx.CmdUnarchiveItem, "Unarchives the specified item, only items with type: TodoModel, TodoAttemptModel, RssLinkedHtmlModel, RssItemModel")]
    public static Task<string> UnarchiveItem(
      [Description("The Item Id of the item to unarchive.")] int itemId)
      => GetTools().UnarchiveItem(itemId);

  }
}
