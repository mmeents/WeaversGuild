using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Enums;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;
using System.Text.Json;
using System.Text.Json.Serialization;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Chess {
  public record GetChessGameQuery(int ChessGameId) : IMcpRequest, IRequest<ChessGameDto?>;

  public class GetChessGameQueryHandler : IRequestHandler<GetChessGameQuery, ChessGameDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    private readonly IServiceScopeFactory _scopeFactory;          
    
    public GetChessGameQueryHandler(IMediator mediator, FabricDbContext context, IServiceScopeFactory scopeFactory) {      
      _mediator = mediator;
      _context = context;       
      _scopeFactory = scopeFactory;
    }

    public async Task<ChessGameDto?> Handle(GetChessGameQuery request, CancellationToken cancellationToken) {

      using var scope = _scopeFactory.CreateScope();
      var _position = scope.ServiceProvider.GetRequiredService<IPosition>();
      var fenData = new FenData(Fen.StartPositionFen); // new game.
      State _lastState = new();
      List<State> _states = new();
      _position.Set(in fenData, ChessMode.Normal, _lastState);
      IGame _game = new Game(_position);

      if (_game == null) {
        throw new Exception($"Chess game with id {request.ChessGameId} could not be initialized");
      }

      var gameItem = await _context.GetItemDtoById(request.ChessGameId, cancellationToken);
      if (gameItem == null) {
        throw new Exception($"Chess game with id {request.ChessGameId} not found");
      }

      var gameResultStatus = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGameStatus)?.Value.AsInt() ?? 0;
      var gameResult = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGameResult)?.Value ?? "";
      var whiteDeskId = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItWhiteDesk)?.Value.AsInt() ?? 0;
      var blackDeskId = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItBlackDesk)?.Value.AsInt() ?? 0;

      List<int> deskIds = new List<int>();      
      if (whiteDeskId > 0) { deskIds.Add(whiteDeskId); }
      if (blackDeskId > 0) { deskIds.Add(blackDeskId); }
      ItemDto? whiteDesk = null;      
      ItemDto? blackDesk = null;      
      ResolveAttributionQueryResult? resWhite = null;
      ResolveAttributionQueryResult? resBlack = null;
      string errorMessage = "";     
      

      if (deskIds.Count > 0) {  // current attribution state of the desks. 
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
        errorMessage = $"Warning: Chess game with id {request.ChessGameId} has no desks assigned for players";
      }     

      // load moves to advance state.
      var playerMovesRaw = gameItem.Data;
      var playerMoves = JsonSerializer.Deserialize<List<MoveRecord>>(playerMovesRaw) ?? new List<MoveRecord>();
      string moveHistory = "";
      var gameResults = "";
      int gameStatus = (int)WeItemType.GameNotStarted;
      MoveList? legalMoves = null;
      bool isWhiteToPlay = true;
      if (playerMoves.Count > 0) {
        gameStatus = (int)WeItemType.GameInProgress;        
        foreach (var moveRecord in playerMoves.OrderBy(r => r.Ply)) {          
          if (!_position.isMoveValid(moveRecord.Move, out Move move)) {
            throw new Exception($"invalid move attempted {moveRecord.Move}");
          }
          
          if ( _game.Pos.SideToMove.IsWhite) {
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

          var newState = new State();
          _states.Add(newState);
          _position.MakeMove(move, newState);
          
        }
      }
      legalMoves = _position.GenerateMoves();
      gameResults = legalMoves.Length == 0
          ? (_position.InCheck ? $"{(isWhiteToPlay ? "White" : "Black")} says Checkmate" : "Stalemate") : "";
      gameStatus = legalMoves.Length == 0
          ? (int)WeItemType.GameCompleted : (int)WeItemType.GameInProgress;

      moveHistory = string.Join(";", playerMoves.OrderBy(r => r.Ply).Select(r => $" {r.Ply}: {r.Move.ToString()}")).Trim();

      var playerWhite = "Unknown";
      if (resWhite != null) {
        playerWhite = resWhite.PresenceModelKey;
      }

      var playerBlack = "Unknown";
      if (resBlack != null) {
        playerBlack = resBlack.PresenceModelKey;
      }

      var dto = new ChessGameDto {
        Id = request.ChessGameId,
        Name = gameItem.Name,
        PlayerWhite = playerWhite,
        PlayerBlack = playerBlack,
        GameStatus = ((WeItemType)gameStatus).Description(),
        FEN = _position.FenNotation,
        GameBoard = _game.RenderBoard(),
        SideToMove = _position.SideToMove.IsWhite ? "White" : "Black",
        LegalMoves = legalMoves != null ? string.Join(",", legalMoves.Select(m => m.Move.ToString())) : "",
        MoveHistory = moveHistory,
        GameResult = gameResults ?? ""
      };

      return dto;
    }
  }

  public class ChessGameDto {
    public int Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TodoId { get; set; } = null;
    public string Name { get; set; } = "";
    public string PlayerWhite { get; set; } = "";
    public string PlayerBlack { get; set; } = "";
    public string GameStatus { get; set; } = "";
    public string FEN { get; set; } = "";
    public string GameBoard { get; set; } = "";
    public string SideToMove { get; set; } = "";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LegalMoves { get; set; } = null;

    public string MoveHistory { get; set; } = "";
    public string GameResult { get; set; } = "";
  }


  public class MoveRecord {
    public int Ply { get; set; }
    public string Move { get; }
    public string AddedBy { get; set; } = "";
    public int PlayerToggle { get; set; }
    public MoveRecord(int ply, string move, string addedBy, int playerToggle) {
      Ply = ply;      // not sure why they didn't name it playNumber, but it's the number of moves in the game. first move is ply 1, second move is ply 2, etc.  Each player has a turn, so ply 1 is white's first move, ply 2 is black's first move, ply 3 is white's second move, etc.
      Move = move;   // the move, will be validated against list of possible moves in the game.
      AddedBy = addedBy;  // model stamp for attribution from todoId and desk.
      PlayerToggle = playerToggle;  // 306: WeItemType.PlayerWhiteModel, 307: WeItemType.PlayerBlackModel stamped from game on add.
    }
  }




}
