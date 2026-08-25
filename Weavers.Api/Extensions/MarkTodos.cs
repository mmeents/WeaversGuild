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

      group.MapPost("/scheduleBeatWriters", async(IStorytimeToolsHandler handler, int storyId, int handlerDeskId, int? fromTodoId) => {
        try {
          var result = await handler.ScheduleBeatWriters(storyId, handlerDeskId, fromTodoId);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error scheduling beat writers: {ex.Message}");
          return Results.BadRequest("Failed to schedule beat writers.");
        }
      }).WithName("ScheduleBeatWriters").WithDescription("Schedules beat writers.");

      group.MapPost("/scheduleBeatDirectors", async (IStorytimeToolsHandler handler, int sceneId, int handlerDeskId, int? fromTodoId) => {
        try {
          var result = await handler.ScheduleBeatDirectors(sceneId, handlerDeskId, fromTodoId);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error scheduling beat directors: {ex.Message}");
          return Results.BadRequest("Failed to schedule beat directors.");
        }
      }).WithName("ScheduleBeatDirectors").WithDescription("Schedules beat directors.");

      group.MapPost("/scheduleActorPerformances", async (IStorytimeToolsHandler handler, int performanceId, int handlerDeskId, int? fromTodoId) => {
        try {
          var result = await handler.ScheduleActorPerformances(performanceId, handlerDeskId, fromTodoId);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error scheduling actor performances: {ex.Message}");
          return Results.BadRequest("Failed to schedule actor performances.");
        }
      }).WithName("ScheduleActorPerformances").WithDescription("Schedules actor performances.");

      group.MapGet("/getPerformanceRollup", async (IStorytimeToolsHandler handler, int performanceItemId) => {
        try {
          var result = await handler.GetPerformanceRollup(performanceItemId);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error getting performance rollup: {ex.Message}");
          return Results.BadRequest("Failed to get performance rollup.");
        }
      }).WithName("GetPerformanceRollup").WithDescription("Gets the performance rollup for a given performance item.");

      group.MapGet("/getStoryRollup", async (IStorytimeToolsHandler handler, int storyId, string realm) => {
        try {
          var result = await handler.AddStoryRollup(storyId, realm, 0); // Assuming 0 for todoId as it's not provided
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error getting story rollup: {ex.Message}");
          return Results.BadRequest("Failed to get story rollup.");
        }
      }).WithName("GetStoryRollup").WithDescription("Gets the story rollup for a given story and realm.");

      return app;
    }
  }
}
