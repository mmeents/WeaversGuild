using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Service;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public class BaseTools {
    private static IStorytimeToolsHandler GetTools() => DiBridgeService.GetService<IStorytimeToolsHandler>();



    [McpTool(Cx.CmdGetById, "Gets an item by Id (nouns characterId, storyId, sceneId, etc.)")]
    public static async Task<string> GetItemById(
      [Description("the Item Id to get")] int id
    ) => await GetTools().GetItemById(id);

    [McpTool(Cx.CmdGetSubgraph, "Gets the item and related items out to depth levels items deep.")]
    public static async Task<string> GetSubgraph(
      [Description("the Item Id to start from")] int itemId,
      [Description("how many levels of relations to include?")] int depth = 3
    ) => await GetTools().GetSubgraph(itemId, depth);

    
    [McpTool(Cx.CmdAddRelationItem, "create a related item on the parent item, returns the parent with new related item.")]
    public static async Task<string> CreateRelatedItem(
      [Description("Id of the parent Item")]
      int parentItemId,
      [Description(Cx.ValidRelationTypes)]
      int relationTypeId,
      [Description(Cx.ValidItemTypes)]
      int itemTypeId,
      [Description("Item's name")]
      string name,
      [Description("Item's Description")]
      string description = ""      
    ) => await GetTools().CreateRelatedItem(parentItemId, relationTypeId, itemTypeId, name, description, "{}");

    [McpTool(Cx.CmdAddItem, "create new item")]
    public static async Task<string> CreateItem(
      [Description("The name")]
      string name, 
      [Description(Cx.ValidItemTypes)]
      int itemTypeId, 
      [Description("The description")]
      string description       
    ) => await GetTools().CreateItem(name, itemTypeId, description, "{}");


    [McpTool(Cx.CmdUpdateItem, "update existing item")]
    public static async Task<string> UpdateItem(
      [Description("Item's ID")]
      int id,
      [Description("Item's name")]
      string name, 
      [Description(Cx.ValidItemTypes)]
      int itemTypeId, 
      [Description("Item's Description")]
      string description = "" 
    ) => await GetTools().UpdateItem(id, name, itemTypeId, description, "{}");

    [McpTool(Cx.CmdGetRelationById, "Gets a relation by id")]
    public static async Task<string> GetRelationById(int id
    ) => await GetTools().GetRelationById(id);

    [McpTool(Cx.CmdAddRelation, "create a relation")]
    public static async Task<string> CreateRelation(
      [Description("Id of the source Item")]
      int fromItemId, 
      [Description("Id of the target Item")]
      int toItemId, 
      [Description(Cx.ValidRelationTypes)]
      int relationTypeId       
    ) => await GetTools().CreateRelation(fromItemId, toItemId, relationTypeId);

    [McpTool(Cx.CmdUpdateRelation, "update a relation")]
    public static async Task<string> UpdateRelation(
      [Description("Id of the Relation to update")]
      int relationId, 
      [Description("Id of the source Item")]
      int fromItemId, 
      [Description("Id of the target Item")]
      int toItemId, 
      [Description(Cx.ValidRelationTypes)]
      int relationTypeId,
      [Description("The rank of the relation")]
      int rank
    ) => await GetTools().UpdateRelation(relationId, fromItemId, toItemId, relationTypeId, rank);
    


  }
}
