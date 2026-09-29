using System.ComponentModel;
using Weavers.Core.Service;
using MCPSharp;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public class StorytimeTools {
    private static IStorytimeToolsHandler GetTools() => DiBridgeService.GetService<IStorytimeToolsHandler>();

    [McpTool(Cx.CmdAddRealm, Cx.CmdAddRealmDesc)]
    public static async Task<string> AddRealm(
     [Description("Id of the parent folder item.(valid parent types: Organization 1000, ProjectFolder 1100 or RelativeFolder 1110)")]
      int folderId,
     [Description("Name of the new realm")]
      string name,
     [Description("Details of the new realm")]
      string details,
     [Description("Tone stories should have in the new realm")]
      string tone
   ) => await GetTools().AddRealm(folderId, name, details, tone);


    [McpTool(Cx.CmdAddStory, Cx.CmdAddStoryDesc)]
    public static async Task<string> AddStory(
      [Description("Id of the parent realm item")]
      int realmId,
      [Description("Name of the story")]
      string name,
      [Description("Details of the story")]
      string details,
      [Description("Default Point of view type id for the story and scenes. (PovUndefined = 291,PovFirstPerson = 292,PovThirdPersonLimited = 294,PovThirdPersonOmniscient = 295)")]
      int povTypeId,
      [Description("Target number of scenes for the story")]
      int targetSceneCount,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId 
    ) => await GetTools().AddStory(realmId, name, details, povTypeId, targetSceneCount, todoId);


    [McpTool(Cx.CmdAddScene, Cx.CmdAddSceneDesc)]
    public static async Task<string> AddScene(
      [Description("Id of the parent story item")]
      int storyId,
      [Description("Name of the new scene")]
      string name,
      [Description("Entry state of the new scene")]
      string entryState,
      [Description("Exit state of the new scene")]
      string exitState,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddScene(storyId, name, "", entryState, exitState, todoId);

    
    [McpTool(Cx.CmdScheduleBeatWriters, Cx.CmdScheduleBeatWritersDesc)]
    public static async Task<string> ScheduleBeatWriters(
      [Description("Id of the story item")]
      int storyId,
      [Description("Id of the handler desk")]
      int handlerDeskId,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int fromTodoId 
    ) => await GetTools().ScheduleBeatWriters(storyId, handlerDeskId, fromTodoId);
    

    [McpTool(Cx.CmdAddBeat, Cx.CmdAddBeatDesc)]
    public static async Task<string> AddBeat(
      [Description("Id of the parent scene item")]
      int sceneId,
      [Description("Name of the new beat")]
      string name,
      [Description("Details of the new beat")]
      string details,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddBeat(sceneId, name, details, todoId);


    [McpTool(Cx.CmdAddCharacter, Cx.CmdAddCharacterDesc)]
    public static async Task<string> AddCharacter(
      [Description("Id of the parent scene item to add to")]
      int sceneId,
      [Description("Name of the new character")]
      string name,
      [Description("Details of the new character")]
      string details
    ) => await GetTools().AddCharacter(sceneId, name, details);


    [McpTool(Cx.CmdScheduleBeatDirectors, Cx.CmdScheduleBeatDirectorsDesc)]
    public static async Task<string> ScheduleBeatDirectors(
      [Description("Id of the scene item")]
      int sceneId,
      [Description("Id of the handler desk")]
      int handlerDeskId,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int fromTodoId
    ) => await GetTools().ScheduleBeatDirectors(sceneId, handlerDeskId, fromTodoId);


    [McpTool(Cx.CmdAddCallSheet, Cx.CmdAddCallSheetDesc)]
    public static async Task<string> AddCallSheet(
      [Description("Id of the parent beat item")]
      int beatId,
      [Description("Name of the new call sheet")]
      string name,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddCallSheet(beatId, name, "", todoId);

    
    [McpTool(Cx.CmdAddCallSheetNarration, Cx.CmdAddCallSheetNarrationDesc)]
    public static async Task<string> AddCallSheetNarration(
      [Description("Id of the call sheet item")]
      int callSheetId,
      [Description("Name of the new narration")]
      string name,
      [Description("The narration to add")]
      string narration,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddCallSheetNarration(callSheetId, name, narration, todoId);


    [McpTool(Cx.CmdAddCallSheetRole, Cx.CmdAddCallSheetRoleDesc)]
    public static async Task<string> AddCallSheetRole(
      [Description("Id of the call sheet item")]
      int callSheetId,
      [Description("Name of the character")]
      string character,
      [Description("Directions for the characters role")]
      string directions,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddCallSheetRole(callSheetId, character, directions, todoId);


    [McpTool(Cx.CmdScheduleActorPerformances, Cx.CmdScheduleActorPerformancesDesc)]
    public static async Task<string> ScheduleActors(
      [Description("Id of the performance to schedule")]
      int performanceId,
      [Description("Id of the handler desk")]
      int handlerDeskId,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int fromTodoId
    ) => await GetTools().ScheduleActorPerformances(performanceId, handlerDeskId, fromTodoId);
  

    [McpTool(Cx.CmdAddPerformance, Cx.CmdAddPerformanceDesc)]
    public static async Task<string> AddPerformance(
      [Description("Id of the scene to add to.")]
      int sceneId,
      [Description("Name of the new performance")]
      string name
    ) => await GetTools().AddPerformance(sceneId, name, "");

    
    [McpTool(Cx.CmdAddPerformanceAction, Cx.CmdAddPerformanceActionDesc)]
    public static async Task<string> AddPerformanceAction(
      [Description("Id of the actor performance item")]
      int actorPerformanceId,      
      [Description("Describe the action performed by the character")]
      string action,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddPerformanceAction(actorPerformanceId, action, todoId);


    [McpTool(Cx.CmdAddPerformanceLine, Cx.CmdAddPerformanceLineDesc)]
    public static async Task<string> AddPerformanceLine(
      [Description("Id of the actor performance item")]
      int actorPerformanceId,
      [Description("Line spoken by the character")]
      string line,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddPerformanceLine(actorPerformanceId, line, todoId);

    
    [McpTool(Cx.CmdGetPerformanceRollup, Cx.CmdGetPerformanceRollupDesc)]
    public static async Task<string> GetPerformanceRollup(
    [Description("Id of the performance item")]
    int performanceItemId
    ) => await GetTools().GetPerformanceRollup(performanceItemId);


    [McpTool(Cx.CmdAddObservation, Cx.CmdAddObservationDesc)]
    public static async Task<string> AddObserved(
      [Description("Id of the parent performance item to add to.")]
      int performanceId,
      [Description("Name of the new observation")]
      string name,
      [Description("Contents of the new observation")]
      string contents,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddObservation(performanceId, name, contents, todoId);


    [McpTool(Cx.CmdAddStoryRollup, Cx.CmdAddStoryRollupDesc)]
    public static async Task<string> AddStoryRollup(
      [Description("Id story item id to add rollup for. Note: new item is added to story parent, result is a sibling of target story.")]
      int storyId,
      [Description("Realm property of the new story rollup. target production prose for realm with respect to story.")]
      string realm,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int todoId
    ) => await GetTools().AddStoryRollup(storyId, realm, todoId);

  }
}
