using System;
using System.Threading.Tasks;
using MCPSharp;
using Weavers.Core.Service;
using Weavers.Core.Constants;
using Weavers.Core.Enums;

namespace Weavers.Core.Tools {
  public class ChessTools {
    private static IChessToolsHandler GetTools() => DiBridgeService.GetService<IChessToolsHandler>();

    [McpTool(Cx.CmdAddGameRoomModel, "Adds a new game room model. Game rooms can be added to the Org root or other game rooms.")]
    public static Task<string> AddGameRoom(int parentId, string name)
      => GetTools().AddGameRoom(parentId, name);

    [McpTool(Cx.CmdAddChessGameModel, "Adds a new chess game model.")]
    public static Task<string> AddChessGame(int gameRoomId, string name, int whiteDeskId, int blackDeskId)
      => GetTools().AddChessGame(gameRoomId, name, whiteDeskId, blackDeskId);

    [McpTool(Cx.CmdChessGetGame, "Gets a chess game model.")]
    public static Task<string> GetChessGame(int chessGameId)
      => GetTools().GetChessGame(chessGameId);

    [McpTool(Cx.CmdChessStartGame, "Starts a chess game.")]
    public static Task<string> ChessStartGame(int chessGameId)
      => GetTools().ChessStartGame(chessGameId);

    [McpTool(Cx.CmdChessMakeMove, "Makes a move in a chess game.")]
    public static Task<string> ChessMakeMove(int chessGameId, string move, int todoId)
      => GetTools().ChessMakeMove(chessGameId, move, todoId);

  }
}
