using MCPSharp;
using Weavers.Core.Service;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public class AppGraphLibraryTools {
    private static IAppGraphLibraryToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphLibraryToolsHandler>();

    [McpTool(Cx.CmdAddLibrary, "Adds a new library to the graph.")]
    public static async Task<string> AddLibrary(int folderItemId, string? libraryName) {
      var tools = GetTools();
      return await tools.AddLibrary(folderItemId, libraryName);
    }

    [McpTool(Cx.CmdAddNamespace, "Adds a new namespace to the specified parent item.")]
    public static async Task<string> AddNamespace(int parentItemId, string? namespaceName) {
      var tools = GetTools();
      return await tools.AddNamespace(parentItemId, namespaceName);
    }
     

  }
}
