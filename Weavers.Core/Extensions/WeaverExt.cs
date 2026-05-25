using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;

namespace Weavers.Core.Extensions {
  public static class WeaverExt {

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

    public static string AppProjectsPath {
      get {
        string projectsPath = Path.Combine(CommonAppPath, "projects").ResolvePath();
        if (!Directory.Exists(projectsPath)) {
          Directory.CreateDirectory(projectsPath);
        }
        return projectsPath;
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
        string filePath = Path.Combine(claudePath, ".mcp.json");
        if (!Directory.Exists(claudePath)) Directory.CreateDirectory(claudePath);
        if (!File.Exists(filePath)) {
          StringBuilder sb = new();
          sb.Append($"{{\r\n\t\"mcpServers\": {{\r\n\t  \"weavers-mcp\": {{\r\n\t\t\"type\": \"stdio\",\r\n\t\t\"command\": \"{claudePath}\\\\WeaversMCP.exe\",\r\n\t\t\"args\": []\r\n\t  }}\r\n\t}}\r\n}}");
          if (File.Exists(filePath)) {
            File.Delete(filePath);
          }
          File.WriteAllText(filePath, sb.ToString());
        }
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

    public static string KeysAppPath {
      get {
        string keysPath = Path.Combine(CommonAppPath, "keys").ResolvePath();
        if (!Directory.Exists(keysPath)) {
          Directory.CreateDirectory(keysPath);
        }
        return keysPath;
      }
    }


  }
}
