using LibGit2Sharp;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Enums;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using Weavers.Core.Handlers.Pipeline;

namespace Weavers.Core.Handlers.Chess {
  public record ChessMakeMoveCommand(int ChessGameId, string Move, int TodoId) : IMcpRequest, IRequest<ChessGameDto>; 
  public class ChessMakeMoveCommandHandler : IRequestHandler<ChessMakeMoveCommand, ChessGameDto> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    private readonly IServiceScopeFactory _scopeFactory;
    public ChessMakeMoveCommandHandler(IMediator mediator, FabricDbContext context, IServiceScopeFactory serviceScopeFactory) {
      _mediator = mediator;
      _context = context;
      _scopeFactory = serviceScopeFactory;
    }

    public async Task<ChessGameDto> Handle(ChessMakeMoveCommand request, CancellationToken cancellationToken) {
      if (request.ChessGameId <= 0) {
        throw new ArgumentException("ChessGameId must be greater than 0", nameof(request.ChessGameId));
      }
      if (request.TodoId <= 0) {
        throw new ArgumentException("TodoId must be greater than 0", nameof(request.TodoId));
      }


      // get the chess game items by id and validate.
      var paramIds = new List<int>() { request.ChessGameId, request.TodoId };
      var gameItems = await _mediator.Send(new GetItemsByIdsQuery(paramIds), cancellationToken);
      var gameItem = gameItems.FirstOrDefault(g => g.Id == request.ChessGameId);
      if (gameItem == null) {
        throw new Exception($"Chess game with id {request.ChessGameId} not found");
      }
      var todoItem = gameItems.FirstOrDefault(g => g.Id == request.TodoId);
      if (todoItem == null) {
        throw new Exception($"Todo item with id {request.TodoId} not found");
      }
      var referenceItemProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
      if (referenceItemProp != null && int.TryParse(referenceItemProp.Value, out var referenceItemId)) {
        if (referenceItemId != request.ChessGameId) { 
          throw new Exception("Todo item is not associated with the specified chess game.");
        }
      }

      // verify todo status is in progress.
      var todoStatusProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
      if (todoStatusProp == null ||
        (todoStatusProp.Value != ((int)WeItemType.TodoNotStarted).ToString()
          && todoStatusProp.Value != ((int)WeItemType.TodoInProgress).ToString())) {
        throw new Exception("Todo item is not in progress.");
      }

      // check if the game is in InProgress state.
      var gameResultStatus = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGameStatus)?.Value.AsInt() ?? 0;
      if (gameResultStatus != (int)WeItemType.GameInProgress) {
        throw new Exception($"Chess game with id {request.ChessGameId} is not in InProgress state. Current state: {gameResultStatus}");
      }

      List<int> deskIds = new List<int>();
      var whiteDeskId = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItWhiteDesk)?.Value.AsInt() ?? 0;
      var blackDeskId = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItBlackDesk)?.Value.AsInt() ?? 0;
      if (whiteDeskId > 0) { deskIds.Add(whiteDeskId); }
      if (blackDeskId > 0) { deskIds.Add(blackDeskId); }
      ItemDto? whiteDesk = null;
      ItemDto? blackDesk = null;
      ResolveAttributionQueryResult? resWhite = null;
      ResolveAttributionQueryResult? resBlack = null;      
      if (deskIds.Count > 0) {  
        var deskItems = await _mediator.Send(new GetItemsByIdsQuery(deskIds), cancellationToken);
        whiteDesk = deskItems.FirstOrDefault(d => d.Id == whiteDeskId);
        if (whiteDesk != null) {
          resWhite = await _mediator.Send(new ResolveDeskAttributionQuery(whiteDesk.Id), cancellationToken);
        }
        blackDesk = deskItems.FirstOrDefault(d => d.Id == blackDeskId);
        if (blackDesk != null) {
          resBlack = await _mediator.Send(new ResolveDeskAttributionQuery(blackDesk.Id), cancellationToken);
        }
      } else {
        throw new Exception($"Warning: Chess game with id {request.ChessGameId} has no desks assigned for players");
      }

      // solve for todo desk here, used at very bottom to clear the current todo on the desk.
      ItemDto? todoDesk = null;
      var todoParentId = todoItem.GetParentId();
      todoDesk = blackDesk;
      if (todoParentId == whiteDeskId) {      
        todoDesk = whiteDesk;
      } else if (todoParentId != blackDeskId) { 
        throw new Exception($"Todo item with id {request.TodoId} is not associated with either the White or Black desk for this game.");
      }

      // get player attribution for the todo id.
      var playerTodo = gameItems.FirstOrDefault(g => g.Id == request.TodoId);
      var playerAttribution = await _mediator.Send(new ResolveAttributionQuery(request.TodoId), cancellationToken);
      var addedBy = playerAttribution?.PresenceModelKey ?? string.Empty;

      var playerMovesRaw = gameItem.Data;
      var playerMoves = JsonSerializer.Deserialize<List<MoveRecord>>(playerMovesRaw) ?? new List<MoveRecord>();
      List<MoveRecord> _gameMoveRecords = new List<MoveRecord>();
            
      var gameResults = "";

      // set up the starting chess position and game state.
      using var scope = _scopeFactory.CreateScope();
      var _position = scope.ServiceProvider.GetRequiredService<IPosition>();
      var fenData = new FenData(Fen.StartPositionFen); // new game.
      State _lastState = new();
      List<State> _states = new();
      _position.Set(in fenData, ChessMode.Normal, _lastState);
      IGame _game = new Game(_position);

      MoveList? legalMoves = null;
      bool isWhiteToPlay = true;

      // replay all previous moves. 
      if (playerMoves.Count > 0) {              
        foreach (var moveRecord in playerMoves.OrderBy(r => r.Ply)) {
          if (!_position.isMoveValid(moveRecord.Move, out Move move)) {
            throw new Exception($"invalid move attempted {moveRecord.Move}");
          }

          if (_game.Pos.SideToMove.IsWhite) {
            isWhiteToPlay = true;
            if (moveRecord.PlayerToggle != (int)WeItemType.PlayerWhite) {
              throw new Exception($"invalid move attempted {moveRecord.Move} by player {moveRecord.PlayerToggle}");
            }
          } else {
            isWhiteToPlay = false;
            if (moveRecord.PlayerToggle != (int)WeItemType.PlayerBlack) {
              throw new Exception($"invalid move attempted {moveRecord.Move} by player {moveRecord.PlayerToggle}");
            }
          }

          var gState = new State();
          _states.Add(gState);
          _position.MakeMove(move, gState);
          _gameMoveRecords.Add(
            new MoveRecord(_position.Ply, move.ToString(), moveRecord.AddedBy, moveRecord.PlayerToggle));
        }
      }

      // relative to the current position, validate the new toggle and move.
      if (_position.SideToMove.IsWhite) {
        isWhiteToPlay = true;
        if (whiteDeskId != resWhite?.DeskId) {
          throw new Exception($"It's White's turn to move, but desk the TodoId {request.TodoId} is not the White player.");
        }
      } else {
        isWhiteToPlay = false;
        if (blackDeskId != resBlack?.DeskId) {
          throw new Exception($"It's Black's turn to move, but desk the TodoId {request.TodoId} is not the Black player.");
        }
      }

      legalMoves = _position.GenerateMoves();
      Move? newMove = null; 
      foreach (var lm in legalMoves) {
        if (string.Equals(lm.Move.ToString(), request.Move, StringComparison.OrdinalIgnoreCase)) {
          newMove = lm.Move;
          break;
        }
      }
      if (newMove == null) { 
        throw new Exception($"invalid move attempted {request.Move}");
      }

      // make the new move and update the game state.
      var newState = new State();
      _states.Add(newState);
      _position.MakeMove(newMove.Value, newState);
      _gameMoveRecords.Add(
        new MoveRecord(_position.Ply, newMove.Value.ToString(), addedBy, isWhiteToPlay ? (int)WeItemType.PlayerWhite : (int)WeItemType.PlayerBlack));

      // save new move off to game item.       
      gameItem.Data = JsonSerializer.Serialize(_gameMoveRecords);
      var updated = await _mediator.Send(
        new UpdateItemCommand(
          gameItem.Id,
          gameItem.ItemTypeId,
          gameItem.Name,
          gameItem.Description,
          gameItem.Data,
          gameItem.IsActive,
          DateTime.UtcNow), cancellationToken);


      // check game status and prep opponents todo.
      var newLegalMoves = _position.GenerateMoves();     

      var gameStatus = newLegalMoves.Length == 0 ? (int)WeItemType.GameCompleted : (int)WeItemType.GameInProgress;
      ItemDto? newTodoItem = null;
      if (gameStatus != gameResultStatus) {  
        // only update if the game status has changed away from InProgress to stopped.
        // no folloup todo as game is over.
        var gameStatusProp = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGameStatus);
        if (gameStatusProp != null) {
          gameStatusProp.Value = gameStatus.ToString();
          await gameStatusProp.SaveProp(gameItem, _mediator);
        }
        var gameResultProp = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGameResult);        
        if (gameResultProp != null) {
          gameResults = newLegalMoves.Length == 0
            ? (_position.InCheck ? $"{(isWhiteToPlay ? "White" : "Black")} says Checkmate" : "Stalemate") : "";
          gameResultProp.Value = gameResults;
          await gameResultProp.SaveProp(gameItem, _mediator);
        }
      } else {   // set up opponents todo.

        var opponentDeskId = isWhiteToPlay ? blackDeskId : whiteDeskId;
        var opponentDesk = isWhiteToPlay ? blackDesk : whiteDesk;
        var isOpDeskActive = opponentDesk!.Properties.FirstOrDefault(p => p.Name == Cx.ItEnabled)?.Value.AsBoolean() ?? false;

        
        // setup todo on opponent's desk. isWhiteToPlay last move so next is switched.
        var playTodoName = $"gameId: {request.ChessGameId} Play new {(isWhiteToPlay ? "Black" : "White")} next move.";

        newTodoItem = await _mediator.Send(
          new CreateRelatedItemCommand(opponentDeskId, (int)WeRelationTypes.Contains,
            (int)WeItemType.TodoModel, playTodoName, "", "{}"), cancellationToken);
        if (newTodoItem == null) {
          throw new Exception("Failed to create new todo item on opponent's desk.");
        }

        bool newTodoPromptSet = false;
        var newTodoPromptProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPromptTemplate);
        if (newTodoPromptProp != null) {
          newTodoPromptProp.Value =
            "TodoId: {{model.todo.id}} {{model.todo.name}}" + Environment.NewLine + playTodoName;
          await newTodoPromptProp.SaveProp(newTodoItem, _mediator);
          newTodoPromptSet = true;
        }

        // finish filling out properties on new todo item.
        bool newTodoRefItemSet = false;
        var newTodoRefItemIdProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
        if (newTodoRefItemIdProp != null) {
          newTodoRefItemIdProp.ReferenceItemTypeId = (int)WeItemType.ChessGameModel;
          newTodoRefItemIdProp.Value = request.ChessGameId.ToString();
          await newTodoRefItemIdProp.SaveProp(newTodoItem, _mediator);
          newTodoRefItemSet = true;
        }

        var newTodoStatusProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
        if (newTodoStatusProp != null) {
          newTodoStatusProp.Value = ((int)WeItemType.TodoNotStarted).ToString();
          await newTodoStatusProp.SaveProp(newTodoItem, _mediator);
        }

        var itFromTodoProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFromTodo);
        if (itFromTodoProp != null) {
          itFromTodoProp.Value = todoItem.Id.ToString();
          await itFromTodoProp.SaveProp(newTodoItem, _mediator);
        }

        if (isOpDeskActive && newTodoPromptSet && newTodoRefItemSet) {
          var isReadyProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItConfirmedReady);
          if (isReadyProp != null) {
            isReadyProp.Value = "1";
            await isReadyProp.SaveProp(newTodoItem, _mediator);
          }
        }

        var itTodoDepthProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth);
        if (itTodoDepthProp != null) {
          var parentTodoDepth = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoDepth)?.Value;
          int newDepth = 1;
          if (parentTodoDepth != null && int.TryParse(parentTodoDepth, out var parsedDepth)) {
            newDepth = parsedDepth + 1;
          }
          itTodoDepthProp.Value = newDepth.ToString();
          await itTodoDepthProp.SaveProp(newTodoItem, _mediator);
        }

      }

      // mark existing todo complete.
      // update the todo item save the note, status at end.
      var todoCloseReasonProp = todoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCloseReason);
      if (todoCloseReasonProp != null) {
        todoCloseReasonProp.Value = $"{addedBy} moved {request.Move}";
        await todoCloseReasonProp.SaveProp(todoItem, _mediator);
      }

      // find inprogress TodoAttempt relation and update it's ItContinueTodo property.
      string runInProgressType = WeItemType.RunInProgress.AsIntString();
      ItemDto? inProgressTodoAttempt = null;
      List<int> attemptIds = new List<int>();
      foreach (var rel in todoItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.TodoAttemptModel)) {
        var attemptId = rel?.RelatedItemId ?? 0;
        if (attemptId > 0) {
          attemptIds.Add(attemptId);
        }
      }
      List<ItemDto> attemptItems = new List<ItemDto>();
      if (attemptIds.Count != 0) {
        attemptItems = await _mediator.Send(new GetItemsByIdsQuery(attemptIds), cancellationToken);
      }
      foreach (var attemptItem in attemptItems) {
        if (attemptItem != null) {
          var attemptStatusStr = attemptItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus)?.Value;
          if (attemptStatusStr != null && attemptStatusStr == runInProgressType) {
            inProgressTodoAttempt = attemptItem; // found.
            break;
          }
        }
      }      
      if (inProgressTodoAttempt != null && newTodoItem != null) {
        var itContinueTodoProp = inProgressTodoAttempt.Properties.FirstOrDefault(p => p.Name == Cx.ItContinueTodo);
        if (itContinueTodoProp != null) {
          itContinueTodoProp.Value = newTodoItem.Id.ToString();
          await itContinueTodoProp.SaveProp(inProgressTodoAttempt, _mediator);
        }
      }

      // finally, update the original todo item status to completed.      
      if (todoStatusProp != null) {
        todoStatusProp.Value = ((int)WeItemType.TodoCompleteForward).ToString();
        await todoStatusProp.SaveProp(todoItem, _mediator);
      }

      var currentDeskTodoProp = todoDesk!.Properties.FirstOrDefault(p => p.Name == Cx.ItCurrentTodo && p.Value == todoItem.Id.ToString());
      if (currentDeskTodoProp != null) {
        currentDeskTodoProp.Value = ""; // clear current todo on the parent desk.
        await currentDeskTodoProp.SaveProp(todoDesk, _mediator);
      }

      // get the player model names for the dto.
      var playerWhite = "Unknown";
      if (resWhite != null) {
        playerWhite = resWhite.PresenceModelKey;
      }

      var playerBlack = "Unknown";
      if (resBlack != null) {
        playerBlack = resBlack.PresenceModelKey;
      }

      var moveHistory = string.Join(";", _gameMoveRecords.OrderBy(r => r.Ply).Select(r => $" {r.Ply}: {r.Move}")).Trim();

      var dto = new ChessGameDto {
        Id = request.ChessGameId,
        TodoId = newTodoItem?.Id,
        Name = gameItem.Name,
        PlayerWhite = playerWhite,
        PlayerBlack = playerBlack,
        GameStatus = ((WeItemType)gameStatus).Description(),
        FEN = _position.FenNotation,
        GameBoard = _game.RenderBoard(),
        SideToMove = _position.SideToMove.IsWhite ? "White" : "Black",
        LegalMoves = null,
        MoveHistory = moveHistory,
        GameResult = gameResults ?? ""
      };
      return dto;
    }




  }
}
