using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCPSharp;
using Weavers.Core.Constants;
using Weavers.Core.Service;


namespace Weavers.Core.Tools {
  public class TodoTools {
    private static ITodoToolsHandler GetTools() => DiBridgeService.GetService<ITodoToolsHandler>();

    [McpTool(Cx.CmdCompleteTodo, "Marks a todo item as completed with an note and produced item. use zero for no produced item.")]
    public static Task<string> CompleteTodo(int todoId, string todoNote, int producedItemId) 
      => GetTools().CompletedTodo(todoId, todoNote, producedItemId);

    [McpTool(Cx.CmdRejectTodo, "Rejects a todo item with a reason.")]
    public static Task<string> RejectTodo(int todoId, string reason) 
      => GetTools().RejectTodo(todoId, reason);


    [McpTool(Cx.CmdReviewPass, "Marks a todo item as passed review with optional review notes.")]
    public static Task<string> ReviewPass(int todoId, string reviewNotes) 
      => GetTools().ReviewPass(todoId, reviewNotes);


    [McpTool(Cx.CmdReviewFail, "Marks a todo item as failed review with review notes and a change request.")]
    public static Task<string> ReviewFail(int todoId, string reviewNotes, string changeRequest)
      => GetTools().ReviewFail(todoId, reviewNotes, changeRequest);

  }
}
