using System.ComponentModel;
using Weavers.Core.Service;
using MCPSharp;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public class SummaryTools {
    private static ISummaryToolsHandler GetTools() => DiBridgeService.GetService<ISummaryToolsHandler>();

    [McpTool(Cx.CmdListProjects, "Lists all root level projects.")]
    public static async Task<string> ListProjects() 
      => await GetTools().ListProjects();

    [McpTool(Cx.CmdSearch, "Searches for items")]
    public static async Task<string> Search(
      [Description("The search query")] string query,
      [Description("The type of items to search for, 0 means all types")] int byType = 0,
      [Description("Maximum number of results to return")] int maxResults = 10
    ) => await GetTools().Search(query, byType, maxResults);

    /// <summary>
    ///Retrieves a summary description for the specified item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item for which to retrieve the summary.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string with the summary of the
    [McpTool(Cx.CmdGetSummaryById, "Gets a summary of the item")]
    public static async Task<string> GetSummaryById(
      [Description("Item Id to get")] int id,
      [Description("Include all child nodes")] bool nodesUp = false,
      [Description("Include item properties")] bool includeProps = true
    ) => await GetTools().GetSummaryDtoById(id, nodesUp, includeProps);

    [McpTool(Cx.CmdGetTypeDetails, "Lookup details of an item type id.")]
    public static async Task<string> GetTypeDetails(
      [Description("Item Type Id to lookup")] int itemTypeId = 0
    ) => await GetTools().GetTypeDetails(itemTypeId);

    [McpTool(Cx.CmdUpdateItemName, "Update the name of an item by its ID.")]
    public static async Task<string> UpdateItemName(
      [Description("Item Id to update")] int id,
      [Description("New name for the item")] string newName
    ) => await GetTools().UpdateItemName(id, newName);


    [McpTool(Cx.CmdUpdateItemProperty, "Update a property of an item by its property ID.")]
    public static async Task<string> UpdateItemProperty(
      [Description("Item Property Id to update")] int itemPropertyId,
      [Description("New value for the property")] string propertyValue
    ) => await GetTools().UpdateItemProperty(itemPropertyId, propertyValue);

  }
}
