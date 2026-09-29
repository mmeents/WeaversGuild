using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Service;

namespace Weavers.Core.Tools {
  public class PatternTools {
    private static IPatternToolsHandler GetTools() => DiBridgeService.GetService<IPatternToolsHandler>();

    [McpTool(Cx.CmdAddPattern, Cx.CmdAddPatternDesc)]
    public static async Task<string> AddPattern(
      [Description("The ID of the parent folder.(Organization, ProjectRoot, RelativeFolder types)")] int parentId,
      [Description("The name of the pattern")] string name
    ) => await GetTools().AddPattern(parentId, name);

    [McpTool(Cx.CmdAddPatDimension, Cx.CmdAddPatDimensionDesc)]
    public static async Task<string> AddPatDimension(
      [Description("The ID of the pattern")] int patternId,
      [Description("The name of the pattern dimension")] string name,
      [Description("Comma delimited options for the dimension")] string commaDelimOptions
    ) => await GetTools().AddPatDimension(patternId, name, commaDelimOptions);

    [McpTool(Cx.CmdAddPatDimOption, Cx.CmdAddPatDimOptionDesc)]
    public static async Task<string> AddPatDimensionOption(
      [Description("The ID of the pattern dimension")] int patDimId,
      [Description("The name of the pattern dimension option")] string name
    ) => await GetTools().AddPatDimOption(patDimId, name);

    [McpTool(Cx.CmdGetNextDraw, Cx.CmdGetNextDrawDesc)]
    public static async Task<string> GetNextDraw(
      [Description("The ID of the pattern")] int patternId,
      [Description("The todoId you are working off for attribution. use 0 for none.")] int todoId
    ) => await GetTools().GetNextDraw(patternId, todoId);

    [McpTool(Cx.CmdRejectDraw, Cx.CmdRejectDrawDesc)]
    public static async Task<string> RejectDraw(
      [Description("The ID of the draw")] int drawId,
      [Description("The ID of the todo")] int todoId
    ) => await GetTools().RejectDraw(drawId, todoId);

    [McpTool(Cx.CmdAcceptDraw, Cx.CmdAcceptDrawDesc)]
    public static async Task<string> AcceptDraw(
      [Description("The ID of the draw")] int drawId,
      [Description("The ID of the reference item")] int refItemId
    ) => await GetTools().AcceptDraw(drawId, refItemId);

  }
}
