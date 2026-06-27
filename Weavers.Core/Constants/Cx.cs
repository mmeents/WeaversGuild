using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Constants {
  public static class Cx {
    public static string AppName => "WeaversGuild";  // org like name describes project off the app data folder.
    public static string AppOrgExport => "TheOrgExport.md"; 
    public static string AppDescription => "A agentic oriented collection of tools and services to weave code, docs, and data together in a structured agentic manner.";
    // will just go with what they said below lol... 
    public static string OrgCharter => "WeaversGuild is dedicated to creating tools that seamlessly integrate code, documentation, and data to enhance software development. Our mission is to empower developers with innovative solutions that streamline workflows, foster collaboration, and drive productivity. We are committed to building a vibrant community where knowledge sharing and continuous learning are at the core of everything we do.";

    // Org folder names
    public static string AppHarnessAppName => "TheLoom";
    public static string AppSessionsFolder => "Sessions";
    public static string AppGatewayFolder => "Gateways";
    public static string AppTeamFolder => "Team";
    public static string AppDeskRolesFolder => "DeskRoles";
    public static string AppWorkGroupFolder => "WorkGroups";
    public static string AppHarnessMcpName => "TheLoomMcp";
    public static string Provider => "provider";
    public static string AppExeName => "TheLoomApp.exe";
    public static string AppApiName => "Weavers.Api";
    public static string McpAppName => "TheLoomMCP";
    public static string AppVersion => "1.1.37";
    public const string ApiLocalPort = "44344";
    public const string ApiLocalhostUrl = $"https://localhost:{ApiLocalPort}";  // via iis express 
    public const string CredentialProtectorName = "WeaversGuild.YouGotThis";
    public const int KeyLifetimeDays = 90;

    public const double DefaultTemperature = 0.76;
    public const int DefaultLmStudioContextLength = 24000;
    public const int intPropertyLabelLeft = 116;
        
    public const string DaemonsMcpToolName = "mcp/daemonsmcp";
    public const string WeaversMcpToolName = "mcp/theloommcp";

    public static List<string> availableToolsList = new List<string> { 
      //DaemonsMcpToolName, // optional, remove if configured. 
      WeaversMcpToolName 
    };

    public const string ClaudeDefaultModel = "sonnet";

    // legacy Cmd from BaseToolsHandler, consider refactor to be more specific to the handler or tool using them.
    public const string CmdGetById = "get-item-by-id";
    public const string CmdGetSubgraph = "get-subgraph";
    public const string CmdAddRelationItem = "create-related-item";
    public const string CmdAddItem = "create-item";
    public const string CmdUpdateItem = "update-item";
    public const string CmdGetRelationById = "get-relation-by-id";
    public const string CmdAddRelation = "create-relation";
    public const string CmdUpdateRelation = "update-relation";

    // mcp tool commands
    public const string CmdHelp = "help";
    public const string CmdListProjects = "listProjects";
    public const string CmdSearch = "search";
    public const string CmdGetSummaryById = "getSummaryById";
    public const string CmdGetTypeDetails = "getTypeDetails";
    public const string CmdUpdateItemName = "updateItemName";
    public const string CmdUpdateItemContent = "updateItemContent";
    public const string CmdAppendItemContent = "appendItemContent";
    public const string CmdUpdateItemProperty = "updateItemProperty";

    public const string CmdAddOrgDeskRole = "addOrgDeskRole";
    public const string CmdAddOrgDesk = "addOrgDesk";
    public const string CmdAddDeskTodo = "addDeskTodo";
    public const string CmdAddDigitalOperator = "addDigitalOperator";
    public const string CmdAddOrgFolder = "addOrgFolder";
    public const string CmdAddOrgFile = "addOrgFile";
    public const string CmdAddProjectRoot = "addProjectRoot";
    public const string CmdAddSubFolder = "addSubFolder";
    public const string CmdAddSolution = "addSolution";
    public const string CmdAddSolutionImport = "addSolutionImport";    

    public const string CmdCompleteTodo = "completeTodo";
    public const string CmdRejectTodo = "rejectTodo";
    public const string CmdReviewPass = "reviewPass";
    public const string CmdReviewFail = "reviewFail";

    public const string CmdAddMdFile = "addMdFile";
    public const string CmdAddHtmlFile = "addHtmlFile";
    public const string CmdAddConfigFile = "addConfigFile";

    public const string CmdAddLibrary = "addLibrary";
    public const string CmdAddDiModel = "addDiModel";
    public const string CmdAddNamespace = "addNamespace";

    public const string CmdAddClass = "addClass";
    public const string CmdAddClassImport = "addClassImport";
    public const string CmdAddClassProperty = "addClassProperty";
    public const string CmdAddClassMethod = "addClassMethod";
    public const string CmdAddClassMethodParam = "addClassMethodParam";

    public const string CmdAddEntityClass = "addEntityClass";
    public const string CmdAddEntityClassImport = "addEntityClassImport";
    public const string CmdAddEntityProperty = "addEntityProperty";

    public const string ValidRelationTypes = "Relation type ";
    public const string ValidItemTypes = "Item types Id ";

    // code gen defaults
    public const string DefaultSDK = "Microsoft.NET.Sdk";
    public const string DefaultTestSDK = "MSTest.Sdk/3.6.4";

    // AppSettings keys
    public const string ApsDefaultFolder = "AppDefaultFolder";
        
    public const string OrgDocsFolder = "Documents";

    // itemProperty names constants 
    public const string ItAccessModifier = "AccessModifier";   
    public const string ItApiToken = "ApiToken";
    public const string ItBaseType = "BaseType";
    public const string ItCharter = "Charter";
    public const string ItClassType = "ClassType";
    public const string ItClaudeLaunchPath = "ClaudeLaunchPath";
    public const string ItConfirmedReady = "Ready";
    public const string ItContinueTodo = "NextTodo";
    public const string ItContextLength = "ContextLength";
    public const string ItCloseReason = "CloseReason";
    public const string ItCurrentTodo = "CurrentTodo";
    public const string ItDataType = "DataType";
    public const string ItDeleteBehavior = "DeleteBehavior";
    public const string ItDeskPreAsserts = "PreAsserts";
    public const string ItDbContextName = "DbContextName";
    public const string ItDbSchema = "DbSchema";
    public const string ItDbTableName = "DbTableName";
    public const string ItEnabled = "Enabled";
    public const string ItFilePath = "FilePath";
    public const string ItFileExt = "FileExt";
    public const string ItFloorStatus = "FloorStatus";
    public const string ItForeignKey = "ForeignKey";
    public const string ItFromTodo = "FromTodo";
    public const string ItGenerateInterface = "GenInterface";
    public const string ItHarnessId = "HarnessId";
    public const string ItHasDbContext = "HasDbContext";
    public const string ItHasLmStudioPresence = "HasLmStudio";
    public const string ItHasClaudePresence = "HasClaudeCode";
    public const string ItHasMediator = "HasMediator";
    public const string ItHasNavigation = "HasNav";
    public const string ItHasSetter = "HasSetter";
    public const string ItImportObject = "ImportObj";
    public const string ItImportUseInterface = "UseIntf";
    public const string ItInverseNavigation = "InverseNav";
    public const string ItInterface = "Interface";
    public const string ItIPAddress = "IPAddress";
    public const string ItIsAbstract = "IsAbstract";
    public const string ItIsAsync = "IsAsync";
    public const string ItIsCollection = "IsCollection";
    public const string ItReSync = "DoReSync";
    public const string ItIsTestLibrary = "IsTestLib";
    public const string ItIsNullable = "IsNullable";
    public const string ItIsPrimaryKey = "IsPrimaryKey";
    public const string ItIsLibraryReference = "LibReference";
    public const string ItIsPackageReference = "PkgReference";
    public const string ItIsSealed = "IsSealed";
    public const string ItIsStatic = "IsStatic";
    public const string ItIsVirtual = "IsVirtual";
    //public const string ItJobCounter = "JobCounter";
    //public const string ItJobSuccess = "JobSuccess";
    //public const string ItJobFailure = "JobFailure";
    public const string ItLifetimeScope = "LifetimeScope";
    public const string ItLibraryInclude = "LibInclude";
    public const string ItLmStudioConfig = "LmStudioCfg";
    public const string ItMaxSize = "MaxSize";  
    public const string ItMaxAttempts = "MaxAttempts";
    public const string ItMachineName = "MachineName";
    public const string ItModelName = "ModelName";
    public const string ItNamespace = "Namespace";
    public const string ItNamespaceRoot = "NamespaceRoot";
    public const string ItNotes = "Notes";
    public const string ItOnSuccessSendTo = "OnSuccessTo";
    public const string ItOnFailSendTo = "OnFailTo";
    public const string ItOnPushbackSendTo = "OnPushbackTo";
    public const string ItOperator = "Operator";
       
    public const string ItParameterDataType = "ParamType";
    public const string ItParameterClassType = "ParamClass";
    public const string ItPresence = "Presence";
    public const string ItProcessId = "ProcessId";
    public const string ItPortAddress = "Port";
    public const string ItPropertyDataType = "PropType";
    public const string ItPropertyClassType = "PropClass";    
    public const string ItProjectGuid = "ProjectGuid";
    public const string ItProviderType = "ProviderType";
    public const string ItRecordContent = "RecordContent";
    public const string ItStructContent = "StructContent";
    public const string ItRating = "Rating";
    public const string ItReferenceItem = "RefItem";
    public const string ItReturnDataType = "ReturnType";
    public const string ItReturnClassType = "ReturnClass";
    public const string ItReturnNullable = "ReturnNullable";
    public const string ItRegisterDi = "RegisterDI";   
    public const string ItRegisterObject = "RegisterObj";
    public const string ItRegisterInterface = "RegisterIntf";
    public const string ItRelativeFolder = "RelativeFolder";
    public const string ItResultingState = "Results";
    public const string ItResponse = "Response";
    public const string ItRetentionDays = "KeepDays";
    public const string ItDeskRole = "DeskRole";
    public const string ItRole = "Role";
    public const string ItRoleCommands = "RoleCmds";
    public const string ItRootFolder = "RootFolder";
    public const string ItRepoUrl = "RepoUrl";
    public const string ItSkipPermissions = "SkipPerms";
    public const string ItSolutionGuid = "SlnGuid";
    public const string ItStatus = "Status";
    public const string ItSystemPrompt = "SysPrompt";
    public const string ItSystemPromptTemplate = "SysPrompt";
    public const string ItUserPrompt = "UserPrompt";
    public const string ItUserPromptTemplate = "UserPrompt";
    public const string ItTodoItem = "TodoItem";
    public const string ItTodoDepth = "TodoDepth";
    public const string ItTestClassAttribute = "TestClass";
    public const string ItTestMethodAttribute = "TestMethod";
    public const string ItUrlBase = "UrlBase";
    public const string ItUseThis = "UseThis";
    public const string ItUserName = "UserName";
    public const string ItVote = "Votes";
    public const string ItValidate = "Validate";

    public const string TestMethodType = "TestMethod";

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

    // Above is the method signiture and body start tag. Then MethodStartMarker, then body, then MethodEndMarker.
    public const string MethodStartMarker = $"  //Method Marker Start, edit below, leave above and Markers as is.";
    public const string MethodEndMarker =    "  } //Method Marker End";  // this line needs to be stripped when saving.

  }

}
