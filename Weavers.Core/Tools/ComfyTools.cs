using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Service;

namespace Weavers.Core.Tools {
  public class ComfyTools {

    public static IComfyToolsHandler GetTools() => DiBridgeService.GetService<IComfyToolsHandler>();


    [McpTool(Cx.CmdListComfyWorkflows, Cx.CmdListComfyWorkflowsDesc)]
    public static Task<string> ListComfyWorkflows() => GetTools().ListComfyWorkflows();

    [McpTool(Cx.CmdAddComfyTodo, Cx.CmdAddComfyTodoDesc)]
    public static Task<string> AddComfyTodo(
      [Description("The ID of the parent item having type ComfyOperationsModel Id: 1154")] int parentId, 
      [Description("The name of the new Comfy Todo")] string name, 
      [Description("The ID of the workflow item to model this todo. It is an item with a type ComfyWorkflowTemplate Id: 1152;")] int workflowId
    ) => GetTools().AddComfyTodo(parentId, name, workflowId);




  }
}
