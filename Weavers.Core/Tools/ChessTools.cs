using System;
using System.Threading.Tasks;
using MCPSharp;
using Weavers.Core.Service;
using Weavers.Core.Constants;
using Weavers.Core.Enums;

namespace Weavers.Core.Tools {
  public class ChessTools {
    private static IChessToolsHandler GetTools() => DiBridgeService.GetService<IChessToolsHandler>();

    [McpTool(Cx.CmdAddGameRoomModel, Cx.CmdAddGameRoomModelDesc)]
    public static Task<string> AddGameRoom(int parentId, string name)
      => GetTools().AddGameRoom(parentId, name);

    [McpTool(Cx.CmdAddChessGameModel, Cx.CmdAddChessGameModelDesc)]
    public static Task<string> AddChessGame(int gameRoomId, string name, int whiteDeskId, int blackDeskId)
      => GetTools().AddChessGame(gameRoomId, name, whiteDeskId, blackDeskId);

    [McpTool(Cx.CmdChessGetGame, Cx.CmdChessGetGameDesc)]
    public static Task<string> GetChessGame(int chessGameId)
      => GetTools().GetChessGame(chessGameId);

    [McpTool(Cx.CmdChessStartGame, Cx.CmdChessStartGameDesc)]
    public static Task<string> ChessStartGame(int chessGameId)
      => GetTools().ChessStartGame(chessGameId);

    [McpTool(Cx.CmdChessMakeMove, Cx.CmdChessMakeMoveDesc)]
    public static Task<string> ChessMakeMove(int chessGameId, string move, int todoId)
      => GetTools().ChessMakeMove(chessGameId, move, todoId);

  }
}
