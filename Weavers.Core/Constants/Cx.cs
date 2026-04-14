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


    public const int intPropertyLabelLeft = 116;

    public const string ApsDefaultFolder = "AppDefaultFolder";


    // itemProperty names constants
    public const string ItRepoUrl = "RepositoryUrl";
    public const string ItRootFolder = "RootFolder";    
    public const string ItRelativeFolder = "RelativeFolder";
    public const string ItFilePath = "FilePath";
    public const string ItBaseType = "BaseType";
    public const string ItPropertyType = "PropertyType";
    public const string ItPropertyTypeRefName = "PropertyTypeRefName";
    public const string ItIsNullable = "IsNullable";
    public const string ItParameterType = "ParameterType";    
    public const string ItNamespace = "Namespace";
    public const string ItInterface = "Interface";
    public const string ItRecordContent = "RecordContent";
    public const string ItStructContent = "StructContent";
    public const string ItReturnType = "ReturnType";
    public const string ItReturnTypeRefName = "ReturnTypeRefName";
    public const string ItIsAsync = "IsAsync";
    public const string ItIsVirtual = "IsVirtual";
    public const string ItIsStatic = "IsStatic";
    public const string ItIsAbstract = "IsAbstract";
    public const string ItIsSealed = "IsSealed";
    public const string ItParameterTypeRefName = "ParameterTypeRefName";






  }

}
