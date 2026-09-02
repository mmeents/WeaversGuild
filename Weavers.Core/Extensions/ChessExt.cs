using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Enums;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;
using System.Linq;
using System.Text;
using System.Text.Json;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Chess;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ChessExt {

    public static string RenderBoard(this IGame game) {
      var board = game.Pos.FenNotation.Split(' ')[0];
      var sb = new StringBuilder();

      sb.AppendLine($"  [ply {game.Pos.Ply}]");
      sb.AppendLine("   a b c d e f g h");

      var rank = 8;
      foreach (var row in board.Split('/')) {
        sb.Append($"{rank}  ");
        foreach (var c in row) {
          if (char.IsDigit(c))
            sb.Append(string.Concat(Enumerable.Repeat(". ", c - '0')));
          else
            sb.Append(c).Append(' ');
        }
        sb.Append($" {rank}");
        sb.AppendLine();
        rank--;
      }

      sb.AppendLine("   a b c d e f g h");
      return sb.ToString();
    }

    public static bool isMoveValid(this IPosition position, string moveInput, out Move move) {
      move = Move.EmptyMove;
      if (string.IsNullOrWhiteSpace(moveInput)) return false;

      foreach (var vm in position.GenerateMoves()) {
        if (string.Equals(vm.Move.ToString(), moveInput, StringComparison.OrdinalIgnoreCase)) {
          move = vm.Move;
          return true;
        }
      }
      move = default;
      return false;
    }

    public static IGame GetCurrentGame(this IPosition position, ItemDto item) {

      var fenData = new FenData(Fen.StartPositionFen); // new game.
      State _lastState = new();
      List<State> _states = new();
      position.Set(in fenData, ChessMode.Normal, _lastState);
      IGame game = new Game(position);
      
      var playerMovesRaw = item.Data;
      var playerMoves = JsonSerializer.Deserialize<List<MoveRecord>>(playerMovesRaw) ?? new List<MoveRecord>();
            
      if (playerMoves.Count > 0) {
        foreach (var moveRecord in playerMoves.OrderBy(r => r.Ply)) {
          if (!position.isMoveValid(moveRecord.Move, out Move move)) {
            throw new Exception($"invalid move attempted {moveRecord.Move}");
          }

          if (game.Pos.SideToMove.IsWhite) {
            if (moveRecord.PlayerToggle != (int)WeItemType.PlayerWhite) {
              throw new Exception($"invalid move attempted {moveRecord.Move} by player {moveRecord.PlayerToggle}");
            }
          } else {
            if (moveRecord.PlayerToggle != (int)WeItemType.PlayerBlack) {
              throw new Exception($"invalid move attempted {moveRecord.Move} by player {moveRecord.PlayerToggle}");
            }
          }
          var newState = new State();
          _states.Add(newState);
          position.MakeMove(move, newState);
        }
      }
      return game;
    }

  }
}
