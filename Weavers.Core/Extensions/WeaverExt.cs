using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;

namespace Weavers.Core.Extensions {
  public static class WeaverExt {

    public static char[] InvalidFileNameChars() => [.. Path.GetInvalidFileNameChars(), .. MyInvalidList()];
    public static char[] MyInvalidList() => " `~!@#$%^&*()_-+=[]{},.;'".ToCharArray();
    public static string UrlSafe(this string str) {
      return string.Concat(str.Split(InvalidFileNameChars()));
    }

    public static char[] NamesafeChars() => " `~!@#$%^&*()_-+=[]{},;'".ToCharArray();
    public static char[] InvalidNamesafeChars() => [.. Path.GetInvalidFileNameChars(), .. NamesafeChars()];
    public static string NameSafe(this string str) {
      return string.Concat(str.Split(InvalidNamesafeChars()));
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
        if (!Directory.Exists(claudePath)) Directory.CreateDirectory(claudePath);
        if (!File.Exists(Path.Combine(claudePath, ".mcp.json"))) {
          StringBuilder sb = new();
          sb.Append($"{{\r\n\t\"mcpServers\": {{\r\n\t  \"weavers-mcp\": {{\r\n\t\t\"type\": \"stdio\",\r\n\t\t\"command\": \"{claudePath}\\\\WeaversMCP.exe\",\r\n\t\t\"args\": []\r\n\t  }}\r\n\t}}\r\n}}");
          File.WriteAllText(Path.Combine(claudePath, ".mcp.json"), sb.ToString());
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


    public static string ResolvePath(this string path) {
      if (!Path.IsPathRooted(path)) {
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
      }
      return Path.GetFullPath(path);
    }

    public static bool IsValidBrowserUrl(this string url) {
      // 1. Try to create the Uri object
      if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
        return false;

      // 2. Ensure it's a web protocol (not file:// or mailto: unless intended)
      return result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps;
    }
  }
}
