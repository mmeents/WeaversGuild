using System;
using System.Collections.Generic;
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
    public static string AppHarnessAppName => "TheLoomApp";
    public static string AppHarnessMcpName => "TheLoomMcp";
    public static string Provider => "provider";
    public static string AppExeName => "TheLoomApp.exe";
    public static string AppApiName => "Weavers.Api";
    public static string McpAppName => "TheLoomMCP";
    public static string AppVersion => "0.1.2";
    public const string ApiLocalPort = "44344";
    public const string ApiLocalhostUrl = $"https://localhost:{ApiLocalPort}";  // via iis express 
    public const string CredentialProtectorName = "WeaversGuild.YouGotThis";
    public const int KeyLifetimeDays = 90;

    public const int DefaultLmStudioContextLength = 8000;
    public const int intPropertyLabelLeft = 116;

    public const string LMStudioUrl = "http://10.0.0.118:8669";
    public const string LMStudioApiKey = "sk-lm-njtLGuVe:Vcbn9IXvEghho3wt9TCx";
    public const string LMStudioMcpToolName = "mcp/weavers-mcp";
    public const string LMStudioDefaultModel = "nvidia/nemotron-3-nano-4b";

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
    public const string CmdUpdateItemProperty = "updateItemProperty";

    public const string CmdAddDigitalOperator = "addDigitalOperator";
    public const string CmdAddOrgFolder = "addOrgFolder";
    public const string CmdAddOrgFile = "addOrgFile";
    public const string CmdAddProjectRoot = "addProjectRoot";
    public const string CmdAddSubFolder = "addSubFolder";
    public const string CmdAddSolution = "addSolution";
    public const string CmdAddSolutionImport = "addSolutionImport";    

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

    // itemProperty names constants 
    public const string ItAccessModifier = "AccessModifier";
    public const string ItAgentName = "AgentName";
    public const string ItAgentRole = "AgentRole";
    public const string ItApiToken = "ApiToken";
    public const string ItBaseType = "BaseType";    
    public const string ItClassType = "ClassType";
    public const string ItClaudeLaunchPath = "ClaudeLaunchPath";
    public const string ItDataType = "DataType";
    public const string ItDeleteBehavior = "DeleteBehavior";
    public const string ItDbContextName = "DbContextName";
    public const string ItDbSchema = "DbSchema";
    public const string ItDbTableName = "DbTableName";
    public const string ItFilePath = "FilePath";
    public const string ItFileExt = "FileExtension";
    public const string ItForeignKey = "ForeignKey";
    public const string ItGenerateInterface = "GenerateInterface";
    public const string ItHarnessId = "HarnessId";
    public const string ItHasDbContext = "HasDbContext";
    public const string ItHasLmStudioPresence = "HasLmStudio";
    //public const string ItHasClaudePresence = "HasClaude";
    public const string ItHasMediator = "HasMediator";
    public const string ItHasNavigation = "HasNavigation";
    public const string ItHasSetter = "HasSetter";
    public const string ItImportObject = "ImportObject";
    public const string ItImportUseInterface = "UseInterface";
    public const string ItInverseNavigation = "InverseNav";
    public const string ItInterface = "Interface";
    public const string ItIPAddress = "IPAddress";
    public const string ItIsAbstract = "IsAbstract";
    public const string ItIsAsync = "IsAsync";
    public const string ItIsCollection = "IsCollection";
    public const string ItReSync = "DoReSync";
    public const string ItIsTestLibrary = "IsTestLibrary";
    public const string ItIsNullable = "IsNullable";
    public const string ItIsPrimaryKey = "IsPrimaryKey";
    public const string ItIsLibraryReference = "LibReference";
    public const string ItIsPackageReference = "PkgReference";
    public const string ItIsSealed = "IsSealed";
    public const string ItIsStatic = "IsStatic";
    public const string ItIsVirtual = "IsVirtual";
    public const string ItJobCounter = "JobCounter";
    public const string ItJobSuccess = "JobSuccess";
    public const string ItJobFailure = "JobFailure";
    public const string ItLifetimeScope = "LifetimeScope";
    public const string ItLibraryInclude = "LibraryInclude";
    public const string ItLmStudioConfig = "LmStudioConfig";
    public const string ItMaxSize = "MaxSize";    
    public const string ItMachineName = "MachineName";
    public const string ItModelName = "ModelName";
    public const string ItNamespace = "Namespace";
    public const string ItNamespaceRoot = "NamespaceRoot";      
    public const string ItOrgCharter = "OrgCharter";
    public const string ItParameterDataType = "ParamType";
    public const string ItParameterClassType = "ParamClass";
    public const string ItPresence = "Presence";
    public const string ItProcessId = "ProcessId";
    public const string ItPortAddress = "Port";
    public const string ItPropertyDataType = "PropertyType";
    public const string ItPropertyClassType = "PropertyClass";    
    public const string ItProjectGuid = "ProjectGuid";
    public const string ItProviderType = "ProviderType";
    public const string ItRecordContent = "RecordContent";
    public const string ItStructContent = "StructContent";
    public const string ItRating = "Rating";
    public const string ItReturnDataType = "ReturnType";
    public const string ItReturnClassType = "ReturnClass";
    public const string ItReturnNullable = "ReturnNullable";
    public const string ItRegisterDi = "RegisterDI";   
    public const string ItRegisterObject = "RegisterObject";
    public const string ItRegisterInterface = "RegisterInterface";
    public const string ItRelativeFolder = "RelativeFolder";
    public const string ItResultingState = "Results";
    public const string ItRetentionDays = "RetentionDays";
    public const string ItRootFolder = "RootFolder";
    public const string ItRepoUrl = "RepoUrl";
    public const string ItSolutionGuid = "SlnGuid";
    public const string ItSystemPrompt = "SystemPrompt";
    public const string ItTestClassAttribute = "TestClass";
    public const string ItTestMethodAttribute = "TestMethod";
    public const string ItUrlBase = "UrlBase";
    public const string ItUseThis = "UseThis";
    public const string ItUserName = "UserName";
    public const string ItVote = "Votes";

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
