using System.ComponentModel;
using Weavers.Core.Service;
using MCPSharp;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public class SummaryTools {
    private static ISummaryToolsHandler GetTools() => DiBridgeService.GetService<ISummaryToolsHandler>();

    [McpTool(Cx.CmdHelp, Cx.CmdHelpDesc)]
    public static async Task<string> Help() 
      => await GetTools().Help();

    [McpTool(Cx.CmdListProjects, Cx.CmdListProjectsDesc)]
    public static async Task<string> ListProjects() 
      => await GetTools().ListProjects();

    [McpTool(Cx.CmdSearch, Cx.CmdSearchDesc)]
    public static async Task<string> Search(
      [Description("The search query")] string query,
      [Description("The type of items to search for, 0 means all types")] int byType = 0,
      [Description("Maximum number of results to return")] int maxResults = 10
    ) => await GetTools().Search(query, byType, maxResults);

    [McpTool(Cx.CmdGetSummaryById, Cx.CmdGetSummaryByIdDesc)]
    public static async Task<string> GetSummaryById(
      [Description("Item Id to get")] int id,
      [Description("Include all child nodes")] bool nodesUp = false,
      [Description("Include item properties")] bool includeProps = true
    ) => await GetTools().GetSummaryDtoById(id, nodesUp, includeProps);

    [McpTool(Cx.CmdGetTypeDetails, Cx.CmdGetTypeDetailsDesc)]
    public static async Task<string> GetTypeDetails(
      [Description("Item Type Id to lookup")] int itemTypeId = 0
    ) => await GetTools().GetTypeDetails(itemTypeId);

    [McpTool(Cx.CmdUpdateItemName, Cx.CmdUpdateItemNameDesc)]
    public static async Task<string> UpdateItemName(
      [Description("Item Id to update")] int id,
      [Description("New name for the item")] string newName
    ) => await GetTools().UpdateItemName(id, newName);

    [McpTool(Cx.CmdUpdateItemContent, Cx.CmdUpdateItemContentDesc)]
    public static async Task<string> UpdateItemContent(
      [Description("Item Id to update")] int id,
      [Description("Updated content for the item")] string content
    ) => await GetTools().UpdateItemContent(id, content);

    [McpTool(Cx.CmdAppendItemContent, Cx.CmdAppendItemContentDesc)]
    public static async Task<string> AppendItemContent(
      [Description("Item Id to update")] int id,
      [Description("Content to append to the item")] string content
    ) => await GetTools().AppendItemContent(id, content);

    [McpTool(Cx.CmdUpdateItemProperty, Cx.CmdUpdateItemPropertyDesc)]
    public static async Task<string> UpdateItemProperty(
      [Description("Item Property Id to update")] int itemPropertyId,
      [Description("New value for the property")] string propertyValue
    ) => await GetTools().UpdateItemProperty(itemPropertyId, propertyValue);

    [McpTool(Cx.CmdDuplicateItem, Cx.CmdDuplicateItemDesc)]
    public static async Task<string> DuplicateItem(
      [Description("Item Id to duplicate")] int id
    ) => await GetTools().DuplicateItem(id);



  }
}
