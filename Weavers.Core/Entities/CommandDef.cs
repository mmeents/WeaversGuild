using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Entities {

  // Represents a mcp command definition in the system. used with DeskRoles to name the commands
  // that should be presented to the agent for the desk.
  // Version 154 is last version they were in the WeItemType enum, so we keep the legacy id for
  // reference and upgrade purposes.  Upgrade on the fly as used. (RoleCmds upgrades as needed.)
  public class CommandDef {
    public int Id { get; set; } = 0;
    public int? LegacyId { get; set; } = 0;
    public string Code { get; set; } = string.Empty;
    public string McpCode { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

  }

  public class CommandDefConfiguration : IEntityTypeConfiguration<CommandDef> {
    public void Configure(EntityTypeBuilder<CommandDef> builder) {
      builder.ToTable("CommandDefs");
      builder.HasKey(c => c.Id);
      builder.Property(c => c.LegacyId);      
      builder.Property(c => c.Code).IsRequired().HasMaxLength(100);
      builder.Property(c => c.McpCode).HasMaxLength(100);
      builder.Property(c => c.Group).IsRequired().HasMaxLength(100);
      builder.Property(c => c.Description).HasMaxLength(500);

      builder.HasIndex(x => x.Code).IsUnique();

      var seedData = Enum.GetValues(typeof(WeCmdType))
        .Cast<WeCmdType>()
        .Select((cmd, index) => new CommandDef {
          Id = (int)cmd,
          Code = cmd.ToString(),
          McpCode = cmd.McpCode(),
          Group = cmd.Group(),
          Description = cmd.Described(),
          LegacyId = (int)cmd.PreviousId()
        })
        .ToList();

      builder.HasData(seedData);
    }
  }

  public enum WeCmdType {
    CmdHelp = 11,   // in Summary Tools.                
    CmdSearch = 13,
    CmdGetSummaryById = 15,
    CmdGetTypeDetails = 17,
    CmdListProjects = 19,

    CmdUpdateItemName = 21,
    CmdUpdateItemContent = 23,
    CmdAppendItemContent = 25,
    CmdUpdateItemProperty = 27,
    CmdDuplicateItem = 29,


    CmdSetTodoReady = 41, // in TodoTools
    CmdCompleteTodo = 42,    
    CmdRejectTodo = 43,
    CmdReviewPass = 44,
    CmdReviewFail = 45,

    CmdAddOrgDeskRole = 51, // in AppGraphOrgTools
    CmdAddOrgDesk = 55,  
    CmdAddDeskTodo = 59,

    CmdAddDigitalOperator = 61,

    CmdAddOrgFolder = 71,
    CmdAddOrgFile = 73,

    CmdAddRssFolder = 81,
    CmdAddRssChannel = 83,
    CmdRssResyncChannel = 84,
    CmdRssResolveLink = 85,
    CmdRssExtractLinks = 86,
    CmdAppendGuildNote = 87,
    CmdUpdateGuildNote = 88,

    CmdArchiveItem = 89,
    CmdUnarchiveItem = 90,

    CmdAddProjectRoot = 91,  // in AppGraphFileTools
    CmdAddSubFolder = 92,

    CmdAddGithubRepo = 93,
    CmdDoGitClone = 94,
    CmdDoGitRefreshStatus = 95,
    CmdDoGitCheckout = 96,

    CmdAddSolution = 97,
    CmdAddSolutionImport = 98,

    CmdAddMdFile = 99,
    CmdAddHtmlFile = 100,
    CmdAddConfigFile = 101,

    // range of legacy command ids: 120-214  // target to miss if queried so to check legacy on import.

    CmdAddRealm = 311,  // in StorytimeTools
    CmdAddStory = 315,
    CmdAddScene = 319,
    CmdScheduleBeatWriters = 321,
    CmdAddBeat = 325,
    CmdAddCharacter = 329,
    CmdScheduleBeatDirectors = 331,
    CmdAddCallSheet = 335,
    CmdAddCallSheetNarration = 339, 
    CmdAddCallSheetRole = 341,
    CmdAddPerformance = 345,
    CmdScheduleActors = 349,
    CmdAddPerformanceAction = 351,  
    CmdAddPerformanceLine = 355,
    CmdGetPerformanceRollup = 359,  
    CmdAddObservation = 361,
    CmdAddStoryRollupModel = 365,



    CmdAddLibrary = 371,  // in AppGraphLibraryTools
    CmdAddNamespace = 375,

    CmdAddClass = 381,  // in AppGraphClassTools
    CmdAddClassImport = 385,
    CmdAddClassProperty = 389,
    CmdAddClassMethod = 393,
    CmdAddClassMethodParam = 397,

    CmdAddEntityClass = 401,  // in AppGraphEntityTools
                              //CmdAddEntityClassImport = 194,
    CmdAddEntityProperty = 405,

    CmdAddGameRoom = 421,
    CmdAddChessGame = 431,
    CmdGetChessGame = 433,
    CmdChessStartGame = 435,
    CmdChessMakeMove = 437,

    CmdAddPattern = 441,
    CmdAddPatDimension = 443,
    CmdAddPatDimOption = 445,
    CmdGetNextDraw = 447,
    CmdRejectDraw = 449,
    CmdAcceptDraw = 451,

    CmdListComfyWorkflows = 462,
    CmdAddComfyTodo = 469,
  }

  // range of legacy command ids: 122-214 from WeItemType.LoomMcpCommands to WeItemType.CmdAcceptDraw
  public enum WeCmdType154 {
    NotSet = 1,
    CmdHelp = 122,   // in Summary Tools.            
    CmdListProjects = 124,
    CmdSearch = 126,
    CmdGetSummaryById = 128,
    CmdGetTypeDetails = 130,
    CmdDuplicateItem = 131,
    CmdUpdateItemName = 132,
    CmdUpdateItemContent = 134,
    CmdAppendItemContent = 135,
    CmdUpdateItemProperty = 136,

    CmdCompleteTodo = 137,
    CmdSetTodoReady = 138,
    CmdRejectTodo = 139,
    CmdReviewPass = 140,
    CmdReviewFail = 141,

    CmdAddOrgDeskRole = 142,
    CmdAddOrgDesk = 143,  // in AppGraphOrgTools
    CmdAddDeskTodo = 144,

    CmdAddDigitalOperator = 145,
    CmdAddOrgFolder = 146,
    CmdAddOrgFile = 148,

    CmdAddRssFolder = 149,
    CmdAddRssChannel = 150,
    CmdRssResyncChannel = 151,
    CmdRssResolveLink = 152,
    CmdRssExtractLinks = 153,
    CmdAppendGuildNote = 154,
    CmdUpdateGuildNote = 155,
    CmdArchiveItem = 156,
    CmdUnarchiveItem = 157,

    CmdAddProjectRoot = 158,  // in AppGraphFileTools
    CmdAddSubFolder = 159,

    CmdAddGithubRepo = 160,
    CmdDoGitClone = 161,
    CmdDoGitRefreshStatus = 162,
    CmdDoGitCheckout = 163,

    CmdAddRealm = 164,
    CmdAddStory = 165,
    CmdAddScene = 166,
    CmdAddCharacter = 167,
    CmdAddBeat = 168,
    CmdScheduleBeatWriters = 169,
    CmdScheduleBeatDirectors = 170,
    CmdAddCallSheet = 171,
    CmdAddCallSheetNarration = 172,  // director
    CmdAddCallSheetRole = 173,
    CmdAddPerformance = 174,
    CmdScheduleActors = 175,
    CmdAddPerformanceAction = 176,    // performance
    CmdAddPerformanceLine = 177,
    CmdGetPerformanceRollup = 178,  // rollup of all performance lines and cross ref with ActorPerformace.
    CmdAddObservation = 179,
    CmdAddStoryRollupModel = 180,

    CmdAddSolution = 181,
    CmdAddSolutionImport = 182,

    CmdAddMdFile = 183,
    CmdAddHtmlFile = 184,
    CmdAddConfigFile = 185,

    CmdAddLibrary = 186,  // in AppGraphLibraryTools
    CmdAddNamespace = 187,

    CmdAddClass = 188,  // in AppGraphClassTools
    CmdAddClassImport = 189,
    CmdAddClassProperty = 190,
    CmdAddClassMethod = 191,
    CmdAddClassMethodParam = 192,

    CmdAddEntityClass = 193,  // in AppGraphEntityTools
                              //CmdAddEntityClassImport = 194,
    CmdAddEntityProperty = 195,

    CmdAddGameRoom = 200,
    CmdAddChessGame = 201,
    CmdGetChessGame = 202,
    CmdChessStartGame = 203,
    CmdChessMakeMove = 204,

    CmdAddPattern = 205,
    CmdAddPatDimension = 206,
    CmdAddPatDimOption = 207,
    CmdGetNextDraw = 208,
    CmdRejectDraw = 209,
    CmdAcceptDraw = 210,

  }

  public static class WeCmdTypeExts {
    public static WeCmdType154 PreviousId(this WeCmdType cmdType) { 
      string cmdName = cmdType.ToString();
      if (Enum.TryParse<WeCmdType154>(cmdName, out var previousCmdType)) {
        return previousCmdType;
      } else {
        return WeCmdType154.NotSet;
      }
    } 

    public static int UpgradeTypeId(this int legacyId) {
      if (Enum.IsDefined(typeof(WeCmdType154), legacyId)) {
        var legacyCmdType = (WeCmdType154)legacyId;
        string cmdName = legacyCmdType.ToString();
        if (Enum.TryParse<WeCmdType>(cmdName, out var newCmdType)) {
          return (int)newCmdType;
        }
      }
      return 0; // Return 0 or throw an exception if the legacy ID is not valid
    }

    public static string McpCode(this WeCmdType cmdType) {
      return cmdType switch {
        WeCmdType.CmdHelp => Cx.CmdHelp,  // in SummaryTools
        WeCmdType.CmdListProjects => Cx.CmdListProjects,
        WeCmdType.CmdSearch => Cx.CmdSearch,
        WeCmdType.CmdGetSummaryById => Cx.CmdGetSummaryById,
        WeCmdType.CmdGetTypeDetails => Cx.CmdGetTypeDetails,
        WeCmdType.CmdDuplicateItem => Cx.CmdDuplicateItem,

        WeCmdType.CmdUpdateItemName => Cx.CmdUpdateItemName,
        WeCmdType.CmdUpdateItemContent => Cx.CmdUpdateItemContent,
        WeCmdType.CmdAppendItemContent => Cx.CmdAppendItemContent,
        WeCmdType.CmdUpdateItemProperty => Cx.CmdUpdateItemProperty,

        WeCmdType.CmdCompleteTodo => Cx.CmdCompleteTodo,
        WeCmdType.CmdSetTodoReady => Cx.CmdSetTodoReady,
        WeCmdType.CmdRejectTodo => Cx.CmdRejectTodo,
        WeCmdType.CmdReviewPass => Cx.CmdReviewPass,
        WeCmdType.CmdReviewFail => Cx.CmdReviewFail,

        WeCmdType.CmdAddOrgDeskRole => Cx.CmdAddOrgDeskRole,
        WeCmdType.CmdAddOrgDesk => Cx.CmdAddOrgDesk,
        WeCmdType.CmdAddDeskTodo => Cx.CmdAddDeskTodo,

        WeCmdType.CmdAddDigitalOperator => Cx.CmdAddDigitalOperator,
        WeCmdType.CmdAddOrgFolder => Cx.CmdAddOrgFolder,
        WeCmdType.CmdAddOrgFile => Cx.CmdAddOrgFile,

        WeCmdType.CmdAddRssFolder => Cx.CmdAddRssFolder,
        WeCmdType.CmdAddRssChannel => Cx.CmdAddRssChannel,
        WeCmdType.CmdRssResyncChannel => Cx.CmdRssResyncChannel,
        WeCmdType.CmdRssResolveLink => Cx.CmdRssResolveLink,
        WeCmdType.CmdRssExtractLinks => Cx.CmdRssExtractLinks,
        WeCmdType.CmdAppendGuildNote => Cx.CmdAppendGuildNote,
        WeCmdType.CmdUpdateGuildNote => Cx.CmdUpdateGuildNote,
        WeCmdType.CmdArchiveItem => Cx.CmdArchiveItem,
        WeCmdType.CmdUnarchiveItem => Cx.CmdUnarchiveItem,

        WeCmdType.CmdAddProjectRoot => Cx.CmdAddProjectRoot,  // in AppGraphFileTools
        WeCmdType.CmdAddSubFolder => Cx.CmdAddSubFolder,

        WeCmdType.CmdAddGithubRepo => Cx.CmdAddGithubRepo,
        WeCmdType.CmdDoGitClone => Cx.CmdDoGitClone,
        WeCmdType.CmdDoGitRefreshStatus => Cx.CmdDoGitRefreshStatus,
        WeCmdType.CmdDoGitCheckout => Cx.CmdDoGitCheckout,

        WeCmdType.CmdAddRealm => Cx.CmdAddRealm,
        WeCmdType.CmdAddStory => Cx.CmdAddStory,
        WeCmdType.CmdAddScene => Cx.CmdAddScene,
        WeCmdType.CmdAddCharacter => Cx.CmdAddCharacter,
        WeCmdType.CmdAddBeat => Cx.CmdAddBeat,
        WeCmdType.CmdScheduleBeatWriters => Cx.CmdScheduleBeatWriters,
        WeCmdType.CmdScheduleBeatDirectors => Cx.CmdScheduleBeatDirectors,
        WeCmdType.CmdAddCallSheet => Cx.CmdAddCallSheet,
        WeCmdType.CmdAddCallSheetNarration => Cx.CmdAddCallSheetNarration,  // director
        WeCmdType.CmdAddCallSheetRole => Cx.CmdAddCallSheetRole,
        WeCmdType.CmdAddPerformance => Cx.CmdAddPerformance,
        WeCmdType.CmdScheduleActors => Cx.CmdScheduleActorPerformances,
        WeCmdType.CmdAddPerformanceAction => Cx.CmdAddPerformanceAction,    // performance
        WeCmdType.CmdAddPerformanceLine => Cx.CmdAddPerformanceLine,
        WeCmdType.CmdGetPerformanceRollup => Cx.CmdGetPerformanceRollup,
        WeCmdType.CmdAddObservation => Cx.CmdAddObservation,
        WeCmdType.CmdAddStoryRollupModel => Cx.CmdAddStoryRollup,

        WeCmdType.CmdAddSolution => Cx.CmdAddSolution,
        WeCmdType.CmdAddSolutionImport => Cx.CmdAddSolutionImport,

        WeCmdType.CmdAddMdFile => Cx.CmdAddMdFile,
        WeCmdType.CmdAddHtmlFile => Cx.CmdAddHtmlFile,
        WeCmdType.CmdAddConfigFile => Cx.CmdAddConfigFile,

        WeCmdType.CmdAddLibrary => Cx.CmdAddLibrary,  // in AppGraphLibraryTools
        WeCmdType.CmdAddNamespace => Cx.CmdAddNamespace,

        WeCmdType.CmdAddClass => Cx.CmdAddClass,  // in AppGraphClassTools
        WeCmdType.CmdAddClassImport => Cx.CmdAddClassImport,
        WeCmdType.CmdAddClassProperty => Cx.CmdAddClassProperty,
        WeCmdType.CmdAddClassMethod => Cx.CmdAddClassMethod,
        WeCmdType.CmdAddClassMethodParam => Cx.CmdAddClassMethodParam,

        WeCmdType.CmdAddEntityClass => Cx.CmdAddEntityClass,  // in AppGraphEntityTools
        //WeCmdType.CmdAddEntityClassImport => Cx.CmdAddEntityClassImport,
        WeCmdType.CmdAddEntityProperty => Cx.CmdAddEntityProperty,

        WeCmdType.CmdAddGameRoom => Cx.CmdAddGameRoomModel,
        WeCmdType.CmdAddChessGame => Cx.CmdAddChessGameModel,
        WeCmdType.CmdGetChessGame => Cx.CmdChessGetGame,
        WeCmdType.CmdChessStartGame => Cx.CmdChessStartGame,
        WeCmdType.CmdChessMakeMove => Cx.CmdChessMakeMove,

        WeCmdType.CmdAddPattern => Cx.CmdAddPattern,
        WeCmdType.CmdAddPatDimension => Cx.CmdAddPatDimension,
        WeCmdType.CmdAddPatDimOption => Cx.CmdAddPatDimOption,
        WeCmdType.CmdGetNextDraw => Cx.CmdGetNextDraw,
        WeCmdType.CmdRejectDraw => Cx.CmdRejectDraw,
        WeCmdType.CmdAcceptDraw => Cx.CmdAcceptDraw,

        WeCmdType.CmdListComfyWorkflows => Cx.CmdListComfyWorkflows,
        WeCmdType.CmdAddComfyTodo => Cx.CmdAddComfyTodo,
        _ => $"No description available for {cmdType}"
      };
    }
    public static string Group(this WeCmdType cmdType) {
      return cmdType switch {
        WeCmdType.CmdHelp => Cx.CmdGroupQuery,  // in SummaryTools
        WeCmdType.CmdListProjects => Cx.CmdGroupQuery,
        WeCmdType.CmdSearch => Cx.CmdGroupQuery,
        WeCmdType.CmdGetSummaryById => Cx.CmdGroupQuery,
        WeCmdType.CmdGetTypeDetails => Cx.CmdGroupQuery,
        WeCmdType.CmdDuplicateItem => Cx.CmdGroupModify,

        WeCmdType.CmdUpdateItemName => Cx.CmdGroupModify,
        WeCmdType.CmdUpdateItemContent => Cx.CmdGroupModify,
        WeCmdType.CmdAppendItemContent => Cx.CmdGroupModify,
        WeCmdType.CmdUpdateItemProperty => Cx.CmdGroupModify,

        WeCmdType.CmdCompleteTodo => Cx.CmdGroupTodoTools,
        WeCmdType.CmdSetTodoReady => Cx.CmdGroupTodoTools,
        WeCmdType.CmdRejectTodo => Cx.CmdGroupTodoTools,
        WeCmdType.CmdReviewPass => Cx.CmdGroupTodoTools,
        WeCmdType.CmdReviewFail => Cx.CmdGroupTodoTools,

        WeCmdType.CmdAddOrgDeskRole => Cx.CmdGroupOrgTools,
        WeCmdType.CmdAddOrgDesk => Cx.CmdGroupOrgTools,
        WeCmdType.CmdAddDeskTodo => Cx.CmdGroupOrgTools,

        WeCmdType.CmdAddDigitalOperator => Cx.CmdGroupOrgTools,
        WeCmdType.CmdAddOrgFolder => Cx.CmdGroupOrgTools,
        WeCmdType.CmdAddOrgFile => Cx.CmdGroupOrgTools,

        WeCmdType.CmdAddRssFolder => Cx.CmdGroupRssTools,
        WeCmdType.CmdAddRssChannel => Cx.CmdGroupRssTools,
        WeCmdType.CmdRssResyncChannel => Cx.CmdGroupRssTools,
        WeCmdType.CmdRssResolveLink => Cx.CmdGroupRssTools,
        WeCmdType.CmdRssExtractLinks => Cx.CmdGroupRssTools,
        WeCmdType.CmdAppendGuildNote => Cx.CmdGroupRssTools,
        WeCmdType.CmdUpdateGuildNote => Cx.CmdGroupRssTools,
        WeCmdType.CmdArchiveItem => Cx.CmdGroupRssTools,
        WeCmdType.CmdUnarchiveItem => Cx.CmdGroupRssTools,

        WeCmdType.CmdAddProjectRoot => Cx.CmdGroupFileTools,  // in AppGraphFileTools
        WeCmdType.CmdAddSubFolder => Cx.CmdGroupFileTools,

        WeCmdType.CmdAddGithubRepo => Cx.CmdGroupFileTools,
        WeCmdType.CmdDoGitClone => Cx.CmdGroupFileTools,
        WeCmdType.CmdDoGitRefreshStatus => Cx.CmdGroupFileTools,
        WeCmdType.CmdDoGitCheckout => Cx.CmdGroupFileTools,

        WeCmdType.CmdAddRealm => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddStory => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddScene => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddCharacter => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddBeat => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdScheduleBeatWriters => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdScheduleBeatDirectors => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddCallSheet => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddCallSheetNarration => Cx.CmdGroupStorytimeTools,  // director
        WeCmdType.CmdAddCallSheetRole => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddPerformance => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdScheduleActors => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddPerformanceAction => Cx.CmdGroupStorytimeTools,    // performance
        WeCmdType.CmdAddPerformanceLine => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdGetPerformanceRollup => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddObservation => Cx.CmdGroupStorytimeTools,
        WeCmdType.CmdAddStoryRollupModel => Cx.CmdGroupStorytimeTools,

        WeCmdType.CmdAddSolution => Cx.CmdGroupFileTools,
        WeCmdType.CmdAddSolutionImport => Cx.CmdGroupFileTools,

        WeCmdType.CmdAddMdFile => Cx.CmdGroupFileTools,
        WeCmdType.CmdAddHtmlFile => Cx.CmdGroupFileTools,
        WeCmdType.CmdAddConfigFile => Cx.CmdGroupFileTools,

        WeCmdType.CmdAddLibrary => Cx.CmdGroupLibraryTools,  // in AppGraphLibraryTools
        WeCmdType.CmdAddNamespace => Cx.CmdGroupLibraryTools,

        WeCmdType.CmdAddClass => Cx.CmdGroupClassTools,  // in AppGraphClassTools
        WeCmdType.CmdAddClassImport => Cx.CmdGroupClassTools,
        WeCmdType.CmdAddClassProperty => Cx.CmdGroupClassTools,
        WeCmdType.CmdAddClassMethod => Cx.CmdGroupClassTools,
        WeCmdType.CmdAddClassMethodParam => Cx.CmdGroupClassTools,

        WeCmdType.CmdAddEntityClass => Cx.CmdGroupEntityTools,  // in AppGraphEntityTools
        //WeCmdType.CmdAddEntityClassImport => Cx.CmdGroupEntityTools,
        WeCmdType.CmdAddEntityProperty => Cx.CmdGroupEntityTools,

        WeCmdType.CmdAddGameRoom => Cx.CmdGroupGameTools,
        WeCmdType.CmdAddChessGame => Cx.CmdGroupGameTools,
        WeCmdType.CmdGetChessGame => Cx.CmdGroupGameTools,
        WeCmdType.CmdChessStartGame => Cx.CmdGroupGameTools,
        WeCmdType.CmdChessMakeMove => Cx.CmdGroupGameTools,

        WeCmdType.CmdAddPattern => Cx.CmdGroupPatternTools,
        WeCmdType.CmdAddPatDimension => Cx.CmdGroupPatternTools,
        WeCmdType.CmdAddPatDimOption => Cx.CmdGroupPatternTools,
        WeCmdType.CmdGetNextDraw => Cx.CmdGroupPatternTools,
        WeCmdType.CmdRejectDraw => Cx.CmdGroupPatternTools,
        WeCmdType.CmdAcceptDraw => Cx.CmdGroupPatternTools,

        WeCmdType.CmdListComfyWorkflows => Cx.CmdGroupComfyTools,        
        WeCmdType.CmdAddComfyTodo => Cx.CmdGroupComfyTools,
        _ => "DefaultGroup"
      };
    }
    public static string Described(this WeCmdType cmdType) {
      return cmdType switch {
        WeCmdType.CmdHelp => Cx.CmdHelpDesc,  // in SummaryTools
        WeCmdType.CmdListProjects => Cx.CmdListProjectsDesc,
        WeCmdType.CmdSearch => Cx.CmdSearchDesc,
        WeCmdType.CmdGetSummaryById => Cx.CmdGetSummaryByIdDesc,
        WeCmdType.CmdGetTypeDetails => Cx.CmdGetTypeDetailsDesc,
        WeCmdType.CmdDuplicateItem => Cx.CmdDuplicateItemDesc,

        WeCmdType.CmdUpdateItemName => Cx.CmdUpdateItemNameDesc,
        WeCmdType.CmdUpdateItemContent => Cx.CmdUpdateItemContentDesc,
        WeCmdType.CmdAppendItemContent => Cx.CmdAppendItemContentDesc,
        WeCmdType.CmdUpdateItemProperty => Cx.CmdUpdateItemPropertyDesc,

        WeCmdType.CmdCompleteTodo => Cx.CmdCompleteTodoDesc,
        WeCmdType.CmdSetTodoReady => Cx.CmdSetTodoReadyDesc,
        WeCmdType.CmdRejectTodo => Cx.CmdRejectTodoDesc,
        WeCmdType.CmdReviewPass => Cx.CmdReviewPassDesc,
        WeCmdType.CmdReviewFail => Cx.CmdReviewFailDesc,

        WeCmdType.CmdAddOrgDeskRole => Cx.CmdAddOrgDeskRoleDesc,
        WeCmdType.CmdAddOrgDesk => Cx.CmdAddOrgDeskDesc,
        WeCmdType.CmdAddDeskTodo => Cx.CmdAddDeskTodoDesc,

        WeCmdType.CmdAddDigitalOperator => Cx.CmdAddDigitalOperatorDesc,
        WeCmdType.CmdAddOrgFolder => Cx.CmdAddOrgFolderDesc,
        WeCmdType.CmdAddOrgFile => Cx.CmdAddOrgFileDesc,

        WeCmdType.CmdAddRssFolder => Cx.CmdAddRssFolderDesc,
        WeCmdType.CmdAddRssChannel => Cx.CmdAddRssChannelDesc,
        WeCmdType.CmdRssResyncChannel => Cx.CmdRssResyncChannelDesc,
        WeCmdType.CmdRssResolveLink => Cx.CmdRssResolveLinkDesc,
        WeCmdType.CmdRssExtractLinks => Cx.CmdRssExtractLinksDesc,
        WeCmdType.CmdAppendGuildNote => Cx.CmdAppendGuildNoteDesc,
        WeCmdType.CmdUpdateGuildNote => Cx.CmdUpdateGuildNoteDesc,
        WeCmdType.CmdArchiveItem => Cx.CmdArchiveItemDesc,
        WeCmdType.CmdUnarchiveItem => Cx.CmdUnarchiveItemDesc,

        WeCmdType.CmdAddProjectRoot => Cx.CmdAddProjectRootDesc,  // in AppGraphFileTools
        WeCmdType.CmdAddSubFolder => Cx.CmdAddSubFolderDesc,

        WeCmdType.CmdAddGithubRepo => Cx.CmdAddGithubRepoDesc,
        WeCmdType.CmdDoGitClone => Cx.CmdDoGitCloneDesc,
        WeCmdType.CmdDoGitRefreshStatus => Cx.CmdDoGitRefreshStatusDesc,
        WeCmdType.CmdDoGitCheckout => Cx.CmdDoGitCheckoutDesc,

        WeCmdType.CmdAddRealm => Cx.CmdAddRealmDesc,
        WeCmdType.CmdAddStory => Cx.CmdAddStoryDesc,
        WeCmdType.CmdAddScene => Cx.CmdAddSceneDesc,
        WeCmdType.CmdAddCharacter => Cx.CmdAddCharacterDesc,
        WeCmdType.CmdAddBeat => Cx.CmdAddBeatDesc,
        WeCmdType.CmdScheduleBeatWriters => Cx.CmdScheduleBeatWritersDesc,
        WeCmdType.CmdScheduleBeatDirectors => Cx.CmdScheduleBeatDirectorsDesc,
        WeCmdType.CmdAddCallSheet => Cx.CmdAddCallSheetDesc,
        WeCmdType.CmdAddCallSheetNarration => Cx.CmdAddCallSheetNarrationDesc,  // director
        WeCmdType.CmdAddCallSheetRole => Cx.CmdAddCallSheetRoleDesc,
        WeCmdType.CmdAddPerformance => Cx.CmdAddPerformanceDesc,
        WeCmdType.CmdScheduleActors => Cx.CmdScheduleActorPerformancesDesc,
        WeCmdType.CmdAddPerformanceAction => Cx.CmdAddPerformanceActionDesc,    // performance
        WeCmdType.CmdAddPerformanceLine => Cx.CmdAddPerformanceLineDesc,
        WeCmdType.CmdGetPerformanceRollup => Cx.CmdGetPerformanceRollupDesc,
        WeCmdType.CmdAddObservation => Cx.CmdAddObservationDesc,
        WeCmdType.CmdAddStoryRollupModel => Cx.CmdAddStoryRollupDesc,

        WeCmdType.CmdAddSolution => Cx.CmdAddSolutionDesc,
        WeCmdType.CmdAddSolutionImport => Cx.CmdAddSolutionImportDesc,

        WeCmdType.CmdAddMdFile => Cx.CmdAddMdFileDesc,
        WeCmdType.CmdAddHtmlFile => Cx.CmdAddHtmlFileDesc,
        WeCmdType.CmdAddConfigFile => Cx.CmdAddConfigFileDesc,

        WeCmdType.CmdAddLibrary => Cx.CmdAddLibraryDesc,  // in AppGraphLibraryTools
        WeCmdType.CmdAddNamespace => Cx.CmdAddNamespaceDesc,

        WeCmdType.CmdAddClass => Cx.CmdAddClassDesc,  // in AppGraphClassTools
        WeCmdType.CmdAddClassImport => Cx.CmdAddClassImportDesc,
        WeCmdType.CmdAddClassProperty => Cx.CmdAddClassPropertyDesc,
        WeCmdType.CmdAddClassMethod => Cx.CmdAddClassMethodDesc,
        WeCmdType.CmdAddClassMethodParam => Cx.CmdAddClassMethodParamDesc,

        WeCmdType.CmdAddEntityClass => Cx.CmdAddEntityClassDesc,  // in AppGraphEntityTools
        //WeCmdType.CmdAddEntityClassImport => Cx.CmdAddEntityClassImportDesc,
        WeCmdType.CmdAddEntityProperty => Cx.CmdAddEntityPropertyDesc,

        WeCmdType.CmdAddGameRoom => Cx.CmdAddGameRoomModelDesc,
        WeCmdType.CmdAddChessGame => Cx.CmdAddChessGameModelDesc,
        WeCmdType.CmdGetChessGame => Cx.CmdChessGetGameDesc,
        WeCmdType.CmdChessStartGame => Cx.CmdChessStartGameDesc,
        WeCmdType.CmdChessMakeMove => Cx.CmdChessMakeMoveDesc,

        WeCmdType.CmdAddPattern => Cx.CmdAddPatternDesc,
        WeCmdType.CmdAddPatDimension => Cx.CmdAddPatDimensionDesc,
        WeCmdType.CmdAddPatDimOption => Cx.CmdAddPatDimOptionDesc,
        WeCmdType.CmdGetNextDraw => Cx.CmdGetNextDrawDesc,
        WeCmdType.CmdRejectDraw => Cx.CmdRejectDrawDesc,
        WeCmdType.CmdAcceptDraw => Cx.CmdAcceptDrawDesc,

        WeCmdType.CmdListComfyWorkflows => Cx.CmdListComfyWorkflowsDesc,
        WeCmdType.CmdAddComfyTodo => Cx.CmdAddComfyTodoDesc,
        _ => $"No description available for {cmdType}"
      };
    }

  }

}
