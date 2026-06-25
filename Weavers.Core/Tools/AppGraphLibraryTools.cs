using MCPSharp;
using System.ComponentModel;
using Weavers.Core.Constants;
using Weavers.Core.Service;

namespace Weavers.Core.Tools {
  public class AppGraphLibraryTools {
    private static IAppGraphLibraryToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphLibraryToolsHandler>();

    [McpTool(Cx.CmdAddLibrary, "Adds a new library to the graph.")]
    public static async Task<string> AddLibrary(
      [Description("The Item Id of the folder to add the library to. (either project root or relative folder)")] int folderItemId, 
      [Description("The name of the new library.")] string libraryName
    ) {
      var tools = GetTools();
      return await tools.AddLibrary(folderItemId, libraryName);
    }

    [McpTool(Cx.CmdAddNamespace, "Adds a new namespace to the specified parent item.")]
    public static async Task<string> AddNamespace(
      [Description("The Item Id of the parent item to add the namespace to. (either a library or another namespace node.)")] int parentItemId, 
      [Description("The name of the new namespace.")] string namespaceName
    ) {
      var tools = GetTools();
      return await tools.AddNamespace(parentItemId, namespaceName);
    }
     

  }
}
