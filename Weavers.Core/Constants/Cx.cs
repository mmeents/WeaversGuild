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
    
    public static char[] InvalidFileNameChars() => Path.GetInvalidFileNameChars()
      .Concat(MyInvalidList()).ToArray();
    public static char[] MyInvalidList() => " `~!@#$%^&*()_-+=[]{},.;'".ToCharArray();
    public static string UrlSafe(this string str) {
      return string.Concat(str.Split(Cx.InvalidFileNameChars()));
    }

    public static string CommonAppPath {
      get {
        string commonPath = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
          Cx.AppName).ResolvePath();
        if (!Directory.Exists(commonPath)) {
          Directory.CreateDirectory(commonPath);
        }
        return commonPath;
      }
    }

    public static string LogsAppPath {
      get {
        string logsPath = Path.Combine(CommonAppPath, "logs").ResolvePath();
        if (!Directory.Exists(logsPath)) {
          Directory.CreateDirectory(logsPath);
        }
        return logsPath;
      }
    }

    public static string ClaudeExecutablePath {
      get {
        string claudePath = Path.Combine(CommonAppPath, "claude").ResolvePath();
        if (!Directory.Exists(claudePath)) Directory.CreateDirectory(claudePath);
//        if (!File.Exists(Path.Combine(claudePath, ".mcp.json"))) {
//          StringBuilder sb = new StringBuilder();
//          sb.Append($"{{\r\n\t\"mcpServers\": {{\r\n\t  \"weavers-mcp\": {{\r\n\t\t\"type\": \"stdio\",\r\n\t\t\"command\": \"{claudePath}\\\\WeaversMCP.exe\",\r\n\t\t\"args\": []\r\n\t  }}\r\n\t}}\r\n}}");
//          File.WriteAllText(Path.Combine(claudePath, ".mcp.json"), sb.ToString());
//        }
        return claudePath;
      }
    }

    public static string ExportPath {
      get {
        string exportPath = Path.Combine(CommonAppPath, "exports").ResolvePath();
        if (!Directory.Exists(exportPath)) {
          Directory.CreateDirectory(exportPath);
        }
        return exportPath;
      }
    }


    public static string ResolvePath(this string path) {
      if (!Path.IsPathRooted(path)) {
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
      }
      return Path.GetFullPath(path);
    }

  }
}
