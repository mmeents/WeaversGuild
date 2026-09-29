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

    [McpTool(Cx.CmdSetTodoReady, Cx.CmdSetTodoReadyDesc)]
    public static Task<string> SetTodoReady(int todoId)
      => GetTools().SetTodoReady(todoId);

    [McpTool(Cx.CmdCompleteTodo, Cx.CmdCompleteTodoDesc)]
    public static Task<string> CompleteTodo(int todoId, string todoNote, int producedItemId)
      => GetTools().CompletedTodo(todoId, todoNote, producedItemId);

    [McpTool(Cx.CmdRejectTodo, Cx.CmdRejectTodoDesc)]
    public static Task<string> RejectTodo(int todoId, string reason)
      => GetTools().RejectTodo(todoId, reason);


    [McpTool(Cx.CmdReviewPass, Cx.CmdReviewPassDesc)]
    public static Task<string> ReviewPass(int todoId, string reviewNotes)
      => GetTools().ReviewPass(todoId, reviewNotes);


    [McpTool(Cx.CmdReviewFail, Cx.CmdReviewFailDesc)]
    public static Task<string> ReviewFail(int todoId, string reviewNotes, string changeRequest)
      => GetTools().ReviewFail(todoId, reviewNotes, changeRequest);


  }
}
