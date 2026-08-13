using System.ComponentModel;
using Weavers.Core.Service;
using MCPSharp;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public class StorytimeTools {
    private static IStorytimeToolsHandler GetTools() => DiBridgeService.GetService<IStorytimeToolsHandler>();

    [McpTool(Cx.CmdAddRealm, "Adds a new story realm project")]
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


    [McpTool(Cx.CmdAddStory, "Adds a new story to a realm.")]
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
      int targetSceneCount
    ) => await GetTools().AddStory(realmId, name, details, povTypeId, targetSceneCount);

    [McpTool(Cx.CmdAddScene, "Adds a new scene to a story")]
    public static async Task<string> AddScene(
      [Description("Id of the parent story item")]
      int storyId,
      [Description("Name of the new scene")]
      string name,
      [Description("Entry state of the new scene")]
      string entryState,
      [Description("Exit state of the new scene")]
      string exitState
    ) => await GetTools().AddScene(storyId, name, "", entryState, exitState);

    
    [McpTool(Cx.CmdScheduleBeatWriters, "Adds todo for each scene in story to write the beats on the handler desk. Skips scenes that have been requested or if it has beats. Details in results")]
    public static async Task<string> ScheduleBeatWriters(
      [Description("Id of the story item")]
      int storyId,
      [Description("Id of the handler desk")]
      int handlerDeskId,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int fromTodoId 
    ) => await GetTools().ScheduleBeatWriters(storyId, handlerDeskId, fromTodoId);     

    [McpTool(Cx.CmdAddBeat, "Adds a new beat to a scene, requires: sceneId, name, details parameters.")]
    public static async Task<string> AddBeat(
      [Description("Id of the parent scene item")]
      int sceneId,
      [Description("Name of the new beat")]
      string name,
      [Description("Details of the new beat")]
      string details
    ) => await GetTools().AddBeat(sceneId, name, details);

    [McpTool(Cx.CmdAddCharacter, "Adds a new character to a scene.")]
    public static async Task<string> AddCharacter(
      [Description("Id of the parent scene item to add to")]
      int sceneId,
      [Description("Name of the new character")]
      string name,
      [Description("Details of the new character")]
      string details
    ) => await GetTools().AddCharacter(sceneId, name, details);

    [McpTool(Cx.CmdScheduleBeatDirectors, "Adds todo for each beat in scene to direct the beat on the handler desk. Skips beats that have been requested or if it has a call sheet. Details in results")]
    public static async Task<string> ScheduleBeatDirectors(
      [Description("Id of the scene item")]
      int sceneId,
      [Description("Id of the handler desk")]
      int handlerDeskId,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int fromTodoId
    ) => await GetTools().ScheduleBeatDirectors(sceneId, handlerDeskId, fromTodoId);


    [McpTool(Cx.CmdAddCallSheet, "Adds a new call sheet to a beat.")]
    public static async Task<string> AddCallSheet(
      [Description("Id of the parent beat item")]
      int beatId,
      [Description("Name of the new call sheet")]
      string name      
    ) => await GetTools().AddCallSheet(beatId, name, "");
    
    [McpTool(Cx.CmdAddCallSheetNarration, "Adds a new narration to a call sheet.")]
    public static async Task<string> AddCallSheetNarration(
      [Description("Id of the call sheet item")]
      int callSheetId,
      [Description("Name of the new narration")]
      string name,
      [Description("The narration to add")]
      string narration
    ) => await GetTools().AddCallSheetNarration(callSheetId, name, narration);

    [McpTool(Cx.CmdAddCallSheetRole, "Adds a character role to a call sheet. Adds Character to scene if not already present by character.")]
    public static async Task<string> AddCallSheetRole(
      [Description("Id of the call sheet item")]
      int callSheetId,
      [Description("Name of the character")]
      string character,
      [Description("Directions for the characters role")]
      string directions 
    ) => await GetTools().AddCallSheetRole(callSheetId, character, directions);


    [McpTool(Cx.CmdScheduleActorPerformances, "Adds todo for each role in performance to direct the acting performance on the handler desk. Skips Roles that have been requested or if it has a ActorPerformance. Details in results")]
    public static async Task<string> ScheduleActors(
      [Description("Id of the performance to schedule")]
      int performanceId,
      [Description("Id of the handler desk")]
      int handlerDeskId,
      [Description("Id of the todo item the agent is working from. For tracking and chaining, if no todo use zero.")]
      int fromTodoId
    ) => await GetTools().ScheduleActorPerformances(performanceId, handlerDeskId, fromTodoId);
  

    [McpTool(Cx.CmdAddPerformance, "Adds a new performance for a scene. Build the data field by enumerating the script entries for all call sheets in scene.")]
    public static async Task<string> AddPerformance(
      [Description("Id of the scene to add to.")]
      int sceneId,
      [Description("Name of the new performance")]
      string name
    ) => await GetTools().AddPerformance(sceneId, name, "");

    
    [McpTool(Cx.CmdAddPerformanceAction, "Adds a new character action to a performance.")]
    public static async Task<string> AddPerformanceAction(
      [Description("Id of the actor performance item")]
      int actorPerformanceId,      
      [Description("Describe the action performed by the character")]
      string action
    ) => await GetTools().AddPerformanceAction(actorPerformanceId, action);

    [McpTool(Cx.CmdAddPerformanceLine, "Adds a new line of dialogue for a character in a performance.")]
    public static async Task<string> AddPerformanceLine(
      [Description("Id of the actor performance item")]
      int actorPerformanceId,
      [Description("Line spoken by the character")]
      string line
    ) => await GetTools().AddPerformanceLine(actorPerformanceId, line);

    
     [McpTool(Cx.CmdGetPerformanceRollup, "Gets a rollup of the performance actions and lines for a performance.")]
     public static async Task<string> GetPerformanceRollup(
      [Description("Id of the performance item")]
      int performanceItemId
     ) => await GetTools().GetPerformanceRollup(performanceItemId);



    [McpTool(Cx.CmdAddObservation, "Adds a new observation to a performance.")]
    public static async Task<string> AddObserved(
      [Description("Id of the parent performance item to add to.")]
      int performanceId,
      [Description("Name of the new observation")]
      string name,
      [Description("Contents of the new observation")]
      string contents
    ) => await GetTools().AddObservation(performanceId, name, contents);

  }
}
