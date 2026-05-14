
using MCPSharp;
using Weavers.Core.Service;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public class AppGraphEntityTools {
    private static IAppGraphEntityToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphEntityToolsHandler>();

    [McpTool(Cx.CmdAddEntityClass, "Adds a new entity class model, with config and primary Id. adds ref to dbContext.")]
    public static Task<string> AddEntityClassModel(
      int parentItemId, 
      string className, 
      string entityDbTableName
    ) {
      return GetTools().AddEntityClassModel(parentItemId, className, entityDbTableName);
    }


    [McpTool(Cx.CmdAddEntityProperty, "Adds a new entity property model to an existing entity class.")]
    public static Task<string> AddEntityPropertyModel(int entityClassId, string propertyName) {
      return GetTools().AddEntityPropertyModel(entityClassId, propertyName);
    }

  }
}
