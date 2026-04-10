using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Constants {
  public static class Cx {
    public static string AppName => "Weavers";
    public static string McpAppName => "WeaversMCP";
    public static string AppVersion => "0.0.2";
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



  }

}
