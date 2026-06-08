using Weavers.Core.Tools;


namespace Weavers.Api.Extensions {
  public static class MarkTodosEndpoint {
    public static WebApplication MapMarkTodoEndpoint(this WebApplication app) {

      var group = app.MapGroup("/api/todo").WithTags("Todo Actions");

      group.MapPost("/markCompleted", async (ITodoToolsHandler handler, 
        int todoId, string todoNote, int? producedItemId) => {
        try {
          var result = await handler.CompletedTodo(todoId, todoNote, producedItemId);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error marking todo as completed: {ex.Message}");
          return Results.BadRequest("Failed to mark todo as completed.");
        }
      }).WithName("MarkTodoAsCompleted").WithDescription("Marks a todo item as completed.");

      group.MapPost("/reject", async (ITodoToolsHandler handler,
        int todoId, string reason) => {
          try {
            var result = await handler.RejectTodo(todoId, reason);
            return Results.Ok(result);
          } catch (Exception ex) {
            Console.WriteLine($"Error rejecting todo: {ex.Message}");
            return Results.BadRequest("Failed to reject todo.");
          }
        }).WithName("RejectTodo").WithDescription("Rejects a todo item with a reason.");

      group.MapPost("/reviewPass", async(ITodoToolsHandler handler, int todoId, string reviewNotes) => {
        try {
          var result = await handler.ReviewPass(todoId, reviewNotes);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error marking todo as review pass: {ex.Message}");
          return Results.BadRequest("Failed to mark todo as review pass.");
        }
      }).WithName("ReviewPassTodo").WithDescription("Marks a todo item as review pass.");

      group.MapPost("/reviewFail", async (ITodoToolsHandler handler, int todoId, string reviewNotes, string changeRequest) => {
        try {
          var result = await handler.ReviewFail(todoId, reviewNotes, changeRequest);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error marking todo as review fail: {ex.Message}");
          return Results.BadRequest("Failed to mark todo as review fail.");
        }
      }).WithName("ReviewFailTodo").WithDescription("Marks a todo item as review fail.");

      return app;
    }
  }
}
