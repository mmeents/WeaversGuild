using MediatR;
using Weavers.Core.Handlers.Chess;


namespace Weavers.Api.Extensions {
  public static class ChessGameEndpoint {
    public static WebApplication MapChessGameEndpoints(this WebApplication app) {
      var group = app.MapGroup("/api/chess").WithTags("Chess Game Actions");

      group.MapPost("/addGame", async (IMediator mediator, AddChessGameCommand command) => {
        try {
          var result = await mediator.Send(command);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error adding chess game: {ex.Message}");
          return Results.BadRequest("Failed to add chess game.");
        }
      }).WithName("AddChessGame").WithDescription("Adds a new chess game to a game room.");

      group.MapGet("/game/{id}", async (IMediator mediator, int id) => {
        try {
          var result = await mediator.Send(new GetChessGameQuery( id ));
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error retrieving chess game: {ex.Message}");
          return Results.BadRequest("Failed to retrieve chess game.");
        }
      }).WithName("GetChessGame").WithDescription("Retrieves a chess game by its ID.");

      group.MapPost("/game/{id}/startGame", async (IMediator mediator, int id ) => {
        try {
          var result = await mediator.Send(new ChessStartGameCommand(id));
          return Results.Ok(result);  
        } catch (Exception ex) {
          Console.WriteLine($"Error starting chess game: {ex.Message}");
          return Results.BadRequest("Failed to start chess game.");
        }
      }).WithName("StartChessGame").WithDescription("Starts a new chess game, sends todo to black desk.");

      group.MapPost("/game/{id}/makeMove", async (IMediator mediator, int gameId, string move, int todoId) => {
        try {
          var command = new ChessMakeMoveCommand(gameId, move, todoId); 
          if (gameId != command.ChessGameId) {
            return Results.BadRequest("Chess game ID in the URL does not match the command.");
          }
          var result = await mediator.Send(command);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error making chess move: {ex.Message}");
          return Results.BadRequest("Failed to make chess move.");
        }
      }).WithName("MakeChessMove").WithDescription("Makes a move in an existing chess game, sends todo to opponents desk.");

      return app;
    }



  }
}
