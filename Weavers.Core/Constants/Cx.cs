using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Constants {
  public static class Cx {
    public static string AppName => "WeaversGuild";  // org like name describes project off the app data folder.
    public static string AppExeName => "TheLoomApp.exe";
    public static string AppApiName => "Weavers.Api";
    public static string McpAppName => "WeaversMCP";
    public static string AppVersion => "0.1.2";
    public const string ApiLocalPort = "44344";
    public const string ApiLocalhostUrl = $"https://localhost:{ApiLocalPort}";  // via iis express 
        
    public const int DefaultLmStudioContextLength = 8000;
    public const int intPropertyLabelLeft = 116;

    public const string LMStudioUrl = "http://10.0.0.118:8669";
    public const string LMStudioApiKey = "sk-lm-njtLGuVe:Vcbn9IXvEghho3wt9TCx";
    public const string LMStudioMcpToolName = "mcp/weavers-mcp";
    public const string LMStudioDefaultModel = "nvidia/nemotron-3-nano-4b";

    public const string ClaudeDefaultModel = "sonnet";

    public const string CmdGetById = "get-item-by-id";
    public const string CmdGetSubgraph = "get-subgraph";
    public const string CmdAddRelationItem = "create-related-item";
    public const string CmdAddItem = "create-item";
    public const string CmdUpdateItem = "update-item";
    public const string CmdGetRelationById = "get-relation-by-id";
    public const string CmdAddRelation = "create-relation";
    public const string CmdUpdateRelation = "update-relation";

    public const string ValidRelationTypes = "Relation type ";
    public const string ValidItemTypes = "Item types Id ";


    public const string ApsDefaultFolder = "AppDefaultFolder";

    // itemProperty names constants 
    public const string ItAccessModifier = "AccessModifier";
    public const string ItBaseType = "BaseType";
    public const string ItClassType = "ClassType";
    public const string ItDataType = "DataType";
    public const string ItDbContextName = "DbContextName";
    public const string ItDbSchema = "DbSchema";
    public const string ItDbTableName = "DbTableName";
    public const string ItFilePath = "FilePath";
    public const string ItFileExt = "FileExtension";
    public const string ItGenerateInterface = "GenerateInterface";
    public const string ItHasDbContext = "HasDbContext";
    public const string ItHasMediator = "HasMediator";
    public const string ItHasNavigation = "HasNavigation";
    public const string ItHasSetter = "HasSetter";
    public const string ItImportObject = "ImportObject";
    public const string ItImportUseInterface = "UseInterface";
    public const string ItInverseNavigation = "InverseNav";
    public const string ItInterface = "Interface";    
    public const string ItIsAbstract = "IsAbstract";
    public const string ItIsCollection = "IsCollection";
    public const string ItIsAsync = "IsAsync";
    public const string ItIsTestLibrary = "IsTestLibrary";
    public const string ItIsNullable = "IsNullable";
    public const string ItIsPrimaryKey = "IsPrimaryKey";
    public const string ItIsLibraryReference = "IsLibraryReference";
    public const string ItIsPackageReference = "IsPackageReference";
    public const string ItIsSealed = "IsSealed";
    public const string ItIsStatic = "IsStatic";
    public const string ItIsVirtual = "IsVirtual";
    public const string ItLifetimeScope = "LifetimeScope";
    public const string ItLibraryInclude = "LibraryInclude";
    public const string ItMaxSize = "MaxSize";
    public const string ItNamespace = "Namespace";
    public const string ItNamespaceRoot = "NamespaceRoot";    
    public const string ItPackageInclude = "PackageInclude";
    public const string ItPackageVersion = "PackageVersion";
    public const string ItParameterDataType = "ParamType";
    public const string ItParameterClassType = "ParamClass";
    public const string ItPropertyDataType = "PropertyType";
    public const string ItPropertyClassType = "PropertyClass";
    public const string ItForeignKey = "ForeignKey";
    public const string ItProjectGuid = "ProjectGuid";
    public const string ItRecordContent = "RecordContent";
    public const string ItStructContent = "StructContent";
    public const string ItReturnDataType = "ReturnType";
    public const string ItReturnClassType = "ReturnClass"; 
    public const string ItRegisterDi = "RegisterDI";   
    public const string ItRegisterObject = "RegisterObject";
    public const string ItRegisterInterface = "RegisterInterface";
    public const string ItRelativeFolder = "RelativeFolder";    
    public const string ItRootFolder = "RootFolder";
    public const string ItRepoUrl = "RepositoryUrl";
    public const string ItSolutionGuid = "SolutionGuid";
    public const string ItUseThis = "UseThis";




  }

}
