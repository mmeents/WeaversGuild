using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;

namespace Weavers.Core.Constants {

  // This class contains constants and configuration values for the WeaversGuild applications.
  public static class Cx {    

    // The Org details.
    public static string AppName => "WeaversGuild";  // repo, root org like name.
    public static string AppVersion => "1.2.160";    // version of the: 1. app. database.    
    public static string AppDescription => "A agentic oriented collection of tools and services to weave code, docs, "+
      "and data together in a structured agentic manner.";    
    public static string OrgCharter => "WeaversGuild is dedicated to creating tools that seamlessly integrate code, "+
      "documentation, and data to enhance software development. Our mission is to empower developers with innovative "+
      "solutions that streamline workflows, foster collaboration, and drive productivity. We are committed to building "+
      "a vibrant community where knowledge sharing and continuous learning are at the core of everything we do.";
    

    // Org folder names                                                    
    public const string ApsDefaultFolder = "AppDefaultFolder";
    public static string AppHarnessAppName => "TheLoom";  // partial name of the harness app, used to find the harness folder in the org.
    public static string AppSessionsFolder => "Sessions";  // harness app folder for sessions.
    public static string AppGatewayFolder => "Gateways";  // harness app folder for gateways.
    public static string AppLoomPresenceFolder => "LoomApp Gateway";  // Gateway to represent the loom app nodes text. 
    public static string AppCredentialStoreFolder => "Credentials";  // Systems secrets vaults name for the org.
    public static string AppTeamFolder => "Team";   // org team folder name for the org.
    public static string AppDeskRolesFolder => "DeskRoles";  // org desk roles folder name for the org.
    public static string AppWorkGroupFolder => "WorkGroups";  // org work groups folder name for the org.
    public const string OrgDocsFolder = "Documents";


    // Mcp App details.
    public static string McpAppName => "TheLoomMCP";
    public const string WeaversMcpToolName = "mcp/theloommcp";

    // MCP startup in mcp.json, use provider key to name the service connection provider.
    // Used in session creation to identify the which app is requesting from params set in startup.
    public static string McpStartupParamProviderKey => "provider";
    public static List<string> availableToolsList = new List<string> { 
      //DaemonsMcpToolName, // optional, remove if configured. 
      WeaversMcpToolName 
    };

    // API details 
    public const string ApiVersion = "v1";
    public const string ApiLocalPort = "44344";
    public const string ApiLocalhostUrl = $"https://localhost:{ApiLocalPort}";  // via iis express 
    public const string CredentialProtectorName = "WeaversGuild.YouGotThis";  // don't change unless starting over with a new org, otherwise existing secrets will be lost.
    public const int KeyLifetimeDays = 90;  // days before a new key is generated for the data protector, old keys are still valid for decryption.


    // int defaults.
    public const double DefaultTemperature = 0.76;  // todo: need to make this configurable per model, maybe per request.
    public const int DefaultLmStudioContextLength = 60000;  // this is default that sets the model contextLength property.
    public const int DefaultSummaryMaxLength = 20000;  // cut off guard on the SummaryDto content results to tool calls. anything larger than this and content is not returned.
    public const int intPropertyLabelLeft = 116; // left margin for the left edge of the property editors in the PropertiesTabs control. 
    public const int NameFieldMaxLength = 500;  // Item Name column width and guard.
    public const long MaxImportFileSize = 512 * 1024; // 512 KB limit for the git file sync import, to avoid large files being imported into the org graph.



    public static string AppOrgExport => "TheOrgExport.md"; 

    // legacy Cmd from BaseToolsHandler, they are unavailable in the mcp server.
    public const string CmdGetById = "get-item-by-id";
    public const string CmdGetSubgraph = "get-subgraph";
    public const string CmdAddRelationItem = "create-related-item";
    public const string CmdAddItem = "create-item";
    public const string CmdUpdateItem = "update-item";
    public const string CmdGetRelationById = "get-relation-by-id";
    public const string CmdAddRelation = "create-relation";
    public const string CmdUpdateRelation = "update-relation";

    // Groups
    public const string CmdGroupQuery = "query";
    public const string CmdGroupModify = "modify";
    public const string CmdGroupOrgTools = "org";
    public const string CmdGroupTodoTools = "todo";
    public const string CmdGroupRssTools = "rss";
    public const string CmdGroupFileTools = "file";
    public const string CmdGroupStorytimeTools = "storytime";
    public const string CmdGroupLibraryTools = "library";
    public const string CmdGroupClassTools = "class";
    public const string CmdGroupEntityTools = "entity";
    public const string CmdGroupGameTools = "game";
    public const string CmdGroupPatternTools = "pattern";
    public const string CmdGroupComfyTools = "comfy";


    // System Summary tool commands
    public const string CmdHelp = "help";
    public const string CmdHelpDesc = "Displays helpful documentation describing how to use the available commands.";
    public const string CmdListProjects = "listProjects";
    public const string CmdListProjectsDesc = "Lists all root level projects.";

    public const string CmdSearch = "search";
    public const string CmdSearchDesc = "Searches for items based on the provided search criteria.";
    public const string CmdGetSummaryById = "getSummaryById";
    public const string CmdGetSummaryByIdDesc = "Gets the summary of an item by its ID.";
    public const string CmdGetTypeDetails = "getTypeDetails";
    public const string CmdGetTypeDetailsDesc = "Lookup details of an item type id.";
    public const string CmdUpdateItemName = "updateItemName";
    public const string CmdUpdateItemNameDesc = "Update the name of an item by its ID.";
    public const string CmdUpdateItemContent = "updateItemContent";
    public const string CmdUpdateItemContentDesc = "Update the content of an item of one of the File types or Method types.";
    public const string CmdAppendItemContent = "appendItemContent";
    public const string CmdAppendItemContentDesc = "Append content to end of existing item. Valid types are Md document types: OrgDocModel and FileMdModel. Infra will handle seperators on append.";
    public const string CmdUpdateItemProperty = "updateItemProperty";
    public const string CmdUpdateItemPropertyDesc = "Update a property of an item by its property ID.";
    public const string CmdDuplicateItem = "duplicateItem";
    public const string CmdDuplicateItemDesc = "Duplicate an item by its ID.";

    // AppGraphOrgTools commands
    public const string CmdAddOrgDeskRole = "addOrgDeskRole";
    public const string CmdAddOrgDeskRoleDesc = "Add a role to an organizational desk.";
    public const string CmdAddOrgDesk = "addOrgDesk";
    public const string CmdAddOrgDeskDesc = "Adds a new desk to the specified workgroup. " +
      "Note: the desk's properties need to be configured after the Add, use " + Cx.CmdUpdateItemProperty + ". " +
      "The SystemPrompt is a Scriban template rendered into the operator's instructions. " +
      "ex: {{ model.desk }} renders the desk name. Template model:\r\n" +
      "  desk - string, the desk name\r\n" +
      "  operator - string, the operator name\r\n" +
      "  role - string, the desk role name\r\n" +
      "  role_commands - list of:\r\n" +
      "    command_type - string\r\n" +
      "    command - string";
    public const string CmdAddDeskTodo = "addDeskTodo";
    public const string CmdAddDeskTodoDesc = "Adds a new Todo to the desk. Note: promptTemplate follows Scriban syntax."+
      "model being passed in has both Todo and Target ItemSummaryDto objects. ex: {{ model.todo.id }} {{ model.target.name }} would "+
      "render todo id and target name.";
    public const string CmdAddDigitalOperator = "addDigitalOperator";
    public const string CmdAddDigitalOperatorDesc = "Adds a digital operator to the specified DigitalOperatorPoolModel typed parentItem."+
      " Note: Properties need to be configured manually after the Add.";
    public const string CmdAddOrgFolder = "addOrgFolder";
    public const string CmdAddOrgFolderDesc = "Adds a new organizational folder.";
    public const string CmdAddOrgFile = "addOrgFile";
    public const string CmdAddOrgFileDesc = "Adds a new .md file item in the specified Org folder item, infra adds ext to name.";

    public const string CmdAddRssFolder = "addRssFolder";
    public const string CmdAddRssFolderDesc = "Adds a new RSS folder.";
    public const string CmdAddRssChannel = "addRssChannel";
    public const string CmdAddRssChannelDesc = "Adds a new RSS channel.";
    public const string CmdRssResyncChannel = "rssResyncChannel";
    public const string CmdRssResyncChannelDesc = "Resyncs the RSS channel, fetches new items and updates the channel.";
    public const string CmdRssResolveLink = "rssResolveLink";
    public const string CmdRssResolveLinkDesc = "Resolves the specified Rss link to an Org file.";
    public const string CmdRssExtractLinks = "rssExtractLinks";
    public const string CmdRssExtractLinksDesc = "Extracts links from the specified Rss link.";
    public const string CmdAppendGuildNote = "appendGuildNote";
    public const string CmdAppendGuildNoteDesc = "Appends a note to the specified GuildNote property. works with item types RssLinkedHtmlModel, RssItemModel, RssChannelModel, RssFolderModel";
    public const string CmdUpdateGuildNote = "updateGuildNote";
    public const string CmdUpdateGuildNoteDesc = "Updates a note in the specified GuildNote property. works with item types RssLinkedHtmlModel, RssItemModel, RssChannelModel, RssFolderModel";
    public const string CmdArchiveItem = "archiveItem";
    public const string CmdArchiveItemDesc = "Archives the specified item, only items with type: TodoModel, TodoAttemptModel, RssLinkedHtmlModel, RssItemModel";
    public const string CmdUnarchiveItem = "unarchiveItem";
    public const string CmdUnarchiveItemDesc = "Unarchives the specified item, only items with type: TodoModel, TodoAttemptModel, RssLinkedHtmlModel, RssItemModel";

    // AppGraphFileTools commands
    public const string CmdAddProjectRoot = "addProjectRoot";
    public const string CmdAddProjectRootDesc = "Adds a new root level project folder.";
    public const string CmdAddSubFolder = "addSubFolder";
    public const string CmdAddSubFolderDesc = "Adds a new sub folder to the specified parent folder or project root.";

    public const string CmdAddGithubRepo = "addGithubRepo";
    public const string CmdAddGithubRepoDesc = "Adds a new GitHub repository item to the specified folder.";
    public const string CmdDoGitClone = "doGitClone";
    public const string CmdDoGitCloneDesc = "Clones the GitHub repository item to the local file system, calls RefreshStatus";
    public const string CmdDoGitRefreshStatus = "doGitRefreshStatus";
    public const string CmdDoGitRefreshStatusDesc = "Refreshes the Git status of the specified repository item. Syncs the child branches to graph.";
    public const string CmdDoGitCheckout = "doGitCheckout";
    public const string CmdDoGitCheckoutDesc = "Checks out the specified branch item. Returns the repository item.";

    public const string CmdAddSolution = "addSolution";
    public const string CmdAddSolutionDesc = "Adds a new solution item under the specified folder.";
    public const string CmdAddSolutionImport = "addSolutionImport";
    public const string CmdAddSolutionImportDesc = "Adds a new solution import relation to the specified solution item.";

    public const string CmdAddMdFile = "addMdFile";
    public const string CmdAddMdFileDesc = "Adds a new .md file item in the specified folder item, infra adds ext to name.";
    public const string CmdAddHtmlFile = "addHtmlFile";
    public const string CmdAddHtmlFileDesc = "Adds a new .html file item in the specified folder item, infra adds ext to name.";
    public const string CmdAddConfigFile = "addConfigFile";
    public const string CmdAddConfigFileDesc = "Adds a new .json file item in the specified folder item, infra adds ext to name.";


    // StorytimeTools commands
    public const string CmdAddRealm = "addRealm";
    public const string CmdAddRealmDesc = "Adds a new story realm project.";
    public const string CmdAddStory = "addStory";
    public const string CmdAddStoryDesc = "Adds a new story item to the realm.";
    public const string CmdAddScene = "addScene";
    public const string CmdAddSceneDesc = "Adds a new scene item to the story.";
    public const string CmdScheduleBeatWriters = "scheduleBeatWriters";
    public const string CmdScheduleBeatWritersDesc = "Adds todo for each scene in story to write the beats on the handler desk. Skips scenes that have been requested or if it has beats. Details in results";
    public const string CmdAddBeat = "addBeat";
    public const string CmdAddBeatDesc = "Adds a new beat item to the scene, requires: sceneId, name, details parameters.";

    public const string CmdAddCharacter = "addCharacter";
    public const string CmdAddCharacterDesc = "Adds a new character item to the scene.";
    public const string CmdScheduleBeatDirectors = "scheduleBeatDirectors";
    public const string CmdScheduleBeatDirectorsDesc = "Adds todo for each beat in scene to direct the beat on the handler desk. Skips beats that have been requested or if it has a call sheet. Details in results"; 

    public const string CmdAddCallSheet = "addCallSheet";
    public const string CmdAddCallSheetDesc = "Adds a new call sheet item to the beat.";
    public const string CmdAddCallSheetNarration = "addCallSheetNarration";  // director
    public const string CmdAddCallSheetNarrationDesc = "Adds a new narration to the call sheet.";
    public const string CmdAddCallSheetRole = "addCallSheetRole";
    public const string CmdAddCallSheetRoleDesc = "Adds a character role to a call sheet. Adds Character to scene if not already present by character.";
    public const string CmdScheduleActorPerformances = "scheduleActorPerformances";
    public const string CmdScheduleActorPerformancesDesc = "Adds todo for each role in performance to direct the acting performance on the handler desk. Skips Roles that have been requested or if it has a ActorPerformance. Details in results";

    
    public const string CmdAddPerformance = "addPerformance";
    public const string CmdAddPerformanceDesc = "Adds a new performance for a scene. Builds the data field by enumerating the script entries for all call sheets in scene.";
    public const string CmdAddPerformanceAction = "addPerformanceAction";    // performance
    public const string CmdAddPerformanceActionDesc = "Adds a new character action item to the performance.";
    public const string CmdAddPerformanceLine = "addPerformanceLine";    
    public const string CmdAddPerformanceLineDesc = "Adds a new line of dialogue for a character in a performance.";
    public const string CmdGetPerformanceRollup = "getPerformanceRollup";
    public const string CmdGetPerformanceRollupDesc = "Gets a rollup of the performance actions and lines for a performance.";
    public const string CmdAddObservation = "addObservation";
    public const string CmdAddObservationDesc = "Adds a new observation item to the scene.";
    public const string CmdAddStoryRollup = "addStoryRollup";
    public const string CmdAddStoryRollupDesc = "Adds a new story rollup item to the story.";

  
    // TodoTools commands 
    public const string CmdSetTodoReady = "setTodoReady";
    public const string CmdSetTodoReadyDesc = "Marks a todo item as ready for execution. Adds it to the execution queue if desk is enabled.";
    public const string CmdCompleteTodo = "completeTodo";
    public const string CmdCompleteTodoDesc = "Marks a todo item as completed with a note and produced item. Use zero for no produced item.";
    public const string CmdRejectTodo = "rejectTodo";
    public const string CmdRejectTodoDesc = "Rejects a todo item with a reason.";

    public const string CmdReviewPass = "reviewPass";
    public const string CmdReviewPassDesc = "Marks a todo item as passed review with optional review notes.";

    public const string CmdReviewFail = "reviewFail";
    public const string CmdReviewFailDesc = "Marks a todo item as failed review with review notes and a change request.";


    // AppGraphLibraryTools commands
    public const string CmdAddLibrary = "addLibrary";
    public const string CmdAddLibraryDesc = "Adds a new csharp library model.";
    public const string CmdAddDiModel = "addDiModel";  // not used, di is included by default.  
    public const string CmdAddNamespace = "addNamespace";
    public const string CmdAddNamespaceDesc = "Adds a new namespace.";

    // AppGraphClassTools.cs commands
    public const string CmdAddClass = "addClass";    
    public const string CmdAddClassDesc = "Adds a new class model, with options to generate interface and register DI.";
    public const string CmdAddClassImport = "addClassImport";
    public const string CmdAddClassImportDesc = "Adds a new class import model to an existing class. Makes a private _var and sets it via constructor and DI.";
    public const string CmdAddClassProperty = "addClassProperty";
    public const string CmdAddClassPropertyDesc = "Adds a new class property model to an existing class.";
    public const string CmdAddClassMethod = "addClassMethod";
    public const string CmdAddClassMethodDesc = "Adds a new class method to an existing class.";
    public const string CmdAddClassMethodParam = "addClassMethodParam";
    public const string CmdAddClassMethodParamDesc = "Adds a new class method parameter to an existing class method.";

    // AppGraphEntityTools.cs commands
    public const string CmdAddEntityClass = "addEntityClass";
    public const string CmdAddEntityClassDesc = "Adds two classes, a new entity class with primary Id property, a entity config class," +
      " and imports ref to DbContext.";
    //public const string CmdAddEntityClassImport = "addEntityClassImport";  // not used just yet...
    public const string CmdAddEntityProperty = "addEntityProperty";
    public const string CmdAddEntityPropertyDesc = "Adds a new entity property model to an existing entity class. "+
      "If it is a navigation property, additional navigation properties will be added; they will need to be configured.";


    // ChessTools.cs commands
    public const string CmdAddGameRoomModel = "addGameRoomModel";
    public const string CmdAddGameRoomModelDesc = "Adds a new game room model. Game rooms can be added to the Org root or other game rooms.";
    public const string CmdAddChessGameModel = "addChessGameModel";
    public const string CmdAddChessGameModelDesc = "Adds a new chess game model.";
    public const string CmdChessGetGame = "getChessGame";
    public const string CmdChessGetGameDesc = "Gets an existing chess game model.";
    public const string CmdChessStartGame = "chessStartGame";
    public const string CmdChessStartGameDesc = "Starts a chess game. (Issues todo on whites desk.)";
    public const string CmdChessMakeMove = "chessMakeMove";
    public const string CmdChessMakeMoveDesc = "Makes a move in a chess game. (Issues todo on opponents desk, marks todo as done.)";

    // PatternTools.cs commands
    public const string CmdAddPattern = "addPattern";
    public const string CmdAddPatternDesc = "Adds a new pattern.";
    public const string CmdAddPatDimension = "addPatDimension";
    public const string CmdAddPatDimensionDesc = "Adds a new dimension to a pattern.";
    public const string CmdAddPatDimOption = "addPatDimOption";
    public const string CmdAddPatDimOptionDesc = "Adds an additional option to a pattern dimension.";
    public const string CmdGetNextDraw = "getNextDraw";
    public const string CmdGetNextDrawDesc = "Get the next draw for a pattern. Sets draw status to DrawIssued id: 311.";
    public const string CmdRejectDraw = "rejectDraw";
    public const string CmdRejectDrawDesc = "Reject a draw for a pattern. Sets draw status to DrawRejected id: 315. Issues and returns another draw.";
    public const string CmdAcceptDraw = "acceptDraw";
    public const string CmdAcceptDrawDesc = "Accept a draw for a pattern. Sets draw status to DrawAccepted id 314. Assigns the reference item.";

    // ends list of commands when they were members of the WeItemType enum.
    public const int LastCommandItemTypeId = 210;

    // ComfyTools.cs commands
    public const string CmdListComfyWorkflows = "listComfyWorkflows";
    public const string CmdListComfyWorkflowsDesc = "Lists all Comfy workflow templates installed.";
    public const string CmdAddComfyTodo = "addComfyTodo";
    public const string CmdAddComfyTodoDesc = "Adds a new Comfy Todo. Clones the workflow template.";

    // Tool property descriptions
    public const string ValidRelationTypes = "Relation type ";
    public const string ValidItemTypes = "Item types Id ";

    // code gen defaults
    public const string DefaultSDK = "Microsoft.NET.Sdk";
    public const string DefaultTestSDK = "MSTest.Sdk/3.6.4";



    // json CharacterPrompt list types
    public const string RoleType = "Role";
    public const string NarrationType = "Narration";
    public const string ActionType = "Action";
    public const string LineType = "Line";


    // itemProperty names constants 
    public const string ItAccessModifier = "AccessModifier";    
    public const string ItAcceptedCount = "AcceptedCount";
    public const string ItApiToken = "ApiToken";
    public const string ItAddedBy = "AddedBy";
    public const string ItBaseType = "BaseType";
    public const string ItBranchName = "BranchName";    
    public const string ItBeatsRequested = "BeatsRequested";
    public const string ItCallSheetRequested = "CallSheetRequested";
    public const string ItFromCallSheet = "FromCallSheet";
    public const string ItCharter = "Charter";
    public const string ItChannelUrl = "ChannelUrl";
    public const string ItClassType = "ClassType";
    public const string ItClaudeLaunchPath = "ClaudeLaunchPath";    
    public const string ItCharacter = "Character";

    public const string ItServiceInput = "ServiceInput";
    public const string ItServiceOutput = "ServiceOutput";

    public const string ItCoverImgPrompt = "CoverImgPrompt";
    public const string ItCoverImgUrl = "CoverImgUrl";
    public const string ItConfirmedReady = "Ready";
    public const string ItContinueTodo = "NextTodo";
    public const string ItContextLength = "ContextLength";  
    public const string ItContentLength = "ContentLength";
    public const string ItCloseReason = "CloseReason";
    public const string ItCredits = "Credits";
    public const string ItCurrentTodo = "CurrentTodo";
    public const string ItCurrentBranch = "CurrentBranch";    
    public const string ItDataType = "DataType";
    public const string ItDeleteBehavior = "DeleteBehavior";
    public const string ItDeskPreAsserts = "PreAsserts";    
    public const string ItDbContextName = "DbContextName";
    public const string ItDbSchema = "DbSchema";
    public const string ItDbTableName = "DbTableName";    
    public const string ItDrawStatus = "DrawStatus";
    public const string ItEnabled = "Enabled";
    public const string ItEntrySha = "EntrySha";
    public const string ItEntryState = "EntryState";
    public const string ItExitState = "ExitState";
    public const string ItExpires = "Expires";
    public const string ItFilePath = "FilePath";
    public const string ItFileExt = "FileExt";
    public const string ItFileSize = "FileSize";
    public const string ItFloorStatus = "FloorStatus";
    public const string ItForeignKey = "ForeignKey";
    public const string ItFromAttempt = "FromAttempt";
    public const string ItFromTodo = "FromTodo";
    public const string ItFriendlyName = "FriendlyName";
    public const string ItGenerateInterface = "GenInterface";
    public const string ItGithubUser = "GithubUser";
    public const string ItGithubPAT = "GithubPAT";
    public const string ItGitPath = "GitPath";
    public const string ItGithubCreds = "GithubCreds";
    public const string ItGuildNotes = "GuildNotes";
    public const string ItHarnessId = "HarnessId";
    public const string ItHasDbContext = "HasDbContext";
    public const string ItHasLmStudioPresence = "HasLmStudio";
    public const string ItHasClaudePresence = "HasClaudeCode";
    public const string ItHasComfyPresence = "HasComfy";
    public const string ItHasMediator = "HasMediator";
    public const string ItHasNavigation = "HasNav";
    public const string ItHasUrl = "HasUrl";
    public const string ItHasSetter = "HasSetter";
    public const string ItImportObject = "ImportObj";
    public const string ItImportUseInterface = "UseIntf";
    public const string ItInverseNavigation = "InverseNav";
    public const string ItInterface = "Interface";
    public const string ItInstructions = "Instructions";
    public const string ItIPAddress = "IPAddress";
    public const string ItIsAbstract = "IsAbstract";
    public const string ItIsAsync = "IsAsync";
    public const string ItIsBinary = "IsBinary";
    public const string ItIsCollection = "IsCollection";    
    public const string ItIsDirty = "IsDirty";
    public const string ItIsTestLibrary = "IsTestLib";
    public const string ItIsNullable = "IsNullable";
    public const string ItIsPrimaryKey = "IsPrimaryKey";
    public const string ItIsLibraryReference = "LibReference";
    public const string ItIsPackageReference = "PkgReference";
    public const string ItIsSealed = "IsSealed";
    public const string ItIsStatic = "IsStatic";
    public const string ItIssuedCount = "IssuedCount";
    public const string ItIsVirtual = "IsVirtual";
    public const string ItIsRemote = "IsRemote";
    public const string ItLastStatusChk = "LastStatusChk";

    public const string ItLastCommitSha = "LastCommitSha";
    public const string ItLastCommitDate = "LastCommitDate";
    public const string ItLastCommitMessage = "LastCommitMsg";
    public const string ItLastCommitAuthor = "LastCommitAuthor";

    public const string ItLifetimeScope = "LifetimeScope";
    public const string ItLibraryInclude = "LibInclude";
    public const string ItLmStudioConfig = "LmStudioCfg";
    public const string ItMaxSize = "MaxSize";  
    public const string ItMaxAttempts = "MaxAttempts";
    public const string ItMaxLinks = "MaxLinks";
    public const string ItMachineName = "MachineName";
    public const string ItMediaType = "MediaType";
    public const string ItModelName = "ModelName";
    public const string ItModelKey = "ModelKey";
    public const string ItModelDetails = "ModelDetails";
    public const string ItModifiedCount = "ModifiedCount";
    public const string ItNamespace = "Namespace";
    public const string ItNamespaceRoot = "NamespaceRoot";
    public const string ItNotes = "Notes";
    public const string ItOnSuccessSendTo = "OnSuccessTo";
    public const string ItOnFailSendTo = "OnFailTo";
    public const string ItOnPushbackSendTo = "OnPushbackTo";
    public const string ItOperator = "Operator";

    public const string ItObjectKey = "ObjectKey";
    

    public const string ItParameterDataType = "ParamType";
    public const string ItParameterClassType = "ParamClass";
    public const string ItParsedOn = "ParsedOn";
    public const string ItPresence = "Presence";
    public const string ItProcessId = "ProcessId";
    public const string ItPropKey = "PropKey";
    public const string ItPropValue = "PropValue";    
    public const string ItPortAddress = "Port";
    public const string ItPovDefault = "PovDefault";
    public const string ItPov = "POV";
    public const string ItPropertyDataType = "PropType";
    public const string ItPropertyClassType = "PropClass";    
    public const string ItProjectGuid = "ProjectGuid";
    public const string ItProviderType = "ProviderType";
    public const string ItRecordContent = "RecordContent";
    public const string ItResolveLink = "ResolveLink";
    public const string ItExtractLink = "ExtractLink";
    public const string ItStructContent = "StructContent";
    public const string ItRank = "Rank";

    public const string ItRejectedCount = "RejectedCount";
    public const string ItRating = "Rating";
    public const string ItRealm = "Realm";
    public const string ItReplacedBy = "ReplacedBy";
    public const string ItProduced = "Produced";
    public const string ItRepoItemId = "RepoItemId";
    public const string ItReferenceItem = "RefItem";
    public const string ItReSync = "DoReSync";
    public const string ItReturnDataType = "ReturnType";
    public const string ItReturnClassType = "ReturnClass";
    public const string ItReturnNullable = "ReturnNullable";
    public const string ItRegisterDi = "RegisterDI";   
    public const string ItRegisterObject = "RegisterObj";
    public const string ItRegisterInterface = "RegisterIntf";
    public const string ItRelativeFolder = "RelativeFolder";
    public const string ItResultingState = "Results";
    public const string ItResponse = "Response";
    public const string ItResolveState = "ResolveState";
    public const string ItResyncChannel = "DoResync";
    public const string ItRetentionDays = "KeepDays";
    public const string ItDeskRole = "DeskRole";
    public const string ItRole = "Role";
    public const string ItRoleCommands = "RoleCmds";
    public const string ItRootFolder = "RootFolder";
    public const string ItRemoteName = "RemoteName";
    public const string ItRepoUrl = "RepoUrl";
    public const string ItSceneStatus = "SceneStatus";
    public const string ItSectionKey = "SectionKey";    
    public const string ItSkipPermissions = "SkipPerms";
    public const string ItSolutionGuid = "SlnGuid";
    public const string ItStatus = "Status";
    public const string ItStoryStatus = "StoryStatus";
    public const string ItSystemPrompt = "SysPrompt";
    public const string ItSystemPromptTemplate = "SysPrompt";    
    public const string ItUserPrompt = "UserPrompt";
    public const string ItUserPromptTemplate = "UserPrompt";
    public const string ItTargetSceneCount = "TargetSceneCount";
    public const string ItTimeoutSec = "TimeoutSec";
    public const string ItOverrideType = "OverrideType";
    public const string ItOutFilePath = "OutFilePath";
    public const string ItTestClassAttribute = "TestClass";
    public const string ItTestMethodAttribute = "TestMethod";
    public const string ItTodoItem = "TodoItem";
    public const string ItTodoDepth = "TodoDepth";
    public const string ItTone = "Tone";    
    public const string ItTrackedBranchName = "TrackedBranch";
    public const string ItUrlBase = "UrlBase";
    public const string ItUseThis = "UseThis";
    public const string ItUserName = "UserName";
    public const string ItUntrackedFiles = "UntrackedFiles";
    public const string ItVote = "Votes";
    public const string ItValidate = "Validate";
    public const string ItWfTemplate = "WfTemplate";


    // library specific properties
    public const string ItVersion = "Version";
    public const string ItFileVersion = "FileVersion";
    public const string ItAssemblyVersion = "AssemblyVersion";
    public const string ItTargetFramework = "TargetFramework";
    public const string ItImplicitUsing = "ImplicitUsing";

    // package specific properties
    public const string ItPackageInclude = "PackageInclude";
    public const string ItPackageVersion = "PackageVersion";
    public const string ItPrivateAssets = "PrivateAssets";
    public const string ItIncludeAssets = "IncludeAssets";

    // Game specific properties
    public const string ItWhiteDesk = "WhiteDesk";
    public const string ItBlackDesk = "BlackDesk";
    public const string ItSideToMove = "SideToMove";
    public const string ItGameStatus = "GameStatus";
    public const string ItGameResult = "GameResult";


    // Above is the method signiture and body start tag. Then MethodStartMarker, then body, then MethodEndMarker.
    public const string MethodStartMarker = $"  //Method Marker Start, edit below, leave above and Markers as is.";
    public const string MethodEndMarker =    "  } //Method Marker End";  // this line needs to be stripped when saving.


  }

}
