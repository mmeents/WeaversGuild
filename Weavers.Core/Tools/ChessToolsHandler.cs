using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Weavers.Core;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Chess;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Weavers.Core.Tools {
  interface IChessToolsHandler {
    Task<string> AddGameRoom(int parentId, string name);
    Task<string> AddChessGame(int gameRoomId, string name, int whiteDeskId, int blackDeskId);
    Task<string> GetChessGame(int chessGameId);
    Task<string> ChessStartGame(int chessGameId);
    Task<string> ChessMakeMove(int chessGameId, string move, int todoId);
  }

  public class ChessToolsHandler : IChessToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ChessToolsHandler(IServiceScopeFactory serviceScopeFactory) {
      _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<string> AddGameRoom(int parentId, string name) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var results = await mediator.Send(new AddGameRoomCommand(parentId, name));
        if (results == null) {
          throw new Exception($"Failed to add game room with name '{name}' under parent ID {parentId}.");
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddGameRoomModel, context.ToSummary(results, false, CancellationToken.None));
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error adding game room model: {ex.Message}";
        var opResult = McpOpResult.CreateFailure(Cx.CmdAddGameRoomModel, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> AddChessGame(int gameRoomId, string name, int whiteDeskId, int blackDeskId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var results = await mediator.Send(new AddChessGameCommand(gameRoomId, name, whiteDeskId, blackDeskId));
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        if (results == null) {
          throw new Exception($"Failed to add chess game with name '{name}' under game room ID {gameRoomId}.");
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddChessGameModel, context.ToSummary(results, false, CancellationToken.None));
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error adding chess board model: {ex.Message}";
        var opResult = McpOpResult.CreateFailure(Cx.CmdAddChessGameModel, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> GetChessGame(int chessGameId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var results = await mediator.Send(new GetChessGameQuery(chessGameId));
        var opResult = McpOpResult.CreateSuccess(Cx.CmdChessGetGame, results);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error getting chess game: {ex.Message}";
        var opResult = McpOpResult.CreateFailure(Cx.CmdChessGetGame, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> ChessStartGame(int chessGameId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var results = "";
        var opResult = McpOpResult.CreateSuccess(Cx.CmdChessStartGame, results);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error starting chess game: {ex.Message}";
        var opResult = McpOpResult.CreateFailure(Cx.CmdChessStartGame, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> ChessMakeMove(int chessGameId, string move, int todoId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var results = await mediator.Send(new ChessMakeMoveCommand(chessGameId, move, todoId));
        var opResult = McpOpResult.CreateSuccess(Cx.CmdChessMakeMove, results);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        var errorMessage = $"Error making chess move: {ex.Message}";
        var opResult = McpOpResult.CreateFailure(Cx.CmdChessMakeMove, errorMessage, ex);
        return JsonSerializer.Serialize(opResult);
      }


    }
  }
}
