using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;

namespace Weavers.Core.Extensions {
  public static class StringExt {
    public static char[] InvalidFileNameChars() => [.. Path.GetInvalidFileNameChars(), .. MyInvalidList()];
    public static char[] MyInvalidList() => " `~!@#$%^&*()_-+=[]{},.;'".ToCharArray();
    public static string UrlSafe(this string str) {
      return string.Concat(str.Split(InvalidFileNameChars()));
    }

    // same as UrlSafe but allows for period.
    public static char[] NamesafeChars() => " `~!@#$%^&*()_-+=[]{},;'".ToCharArray();
    public static char[] InvalidNamesafeChars() => [.. Path.GetInvalidFileNameChars(), .. NamesafeChars()];
    public static string NameSafe(this string str) {
      return string.Concat(str.Split(InvalidNamesafeChars()));
    }

    public static bool AsBoolean(this string? value) {
      if (value == null) return false;
      return value switch {
        "1" => true,
        "0" => false,
        "true" => true,
        "false" => false,
        "True" => true,
        "False" => false,
        _ => false
      };
    }

    public static string[] Parse(this string content, string delims) {
      return content.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public static int AsInt32(this string? value) {
      if (value == null) return 0;
      if (int.TryParse(value, out int result)) return result;
      return 0;
    }

    public static int AsInt(this object? value) { 
      if (value == null) return 0; 
      if (int.TryParse(value.ToString(), out int result)) return result; 
      return 0; 
    }

    public static string AsLowerCaseFirstLetter(this string content) {
      if (string.IsNullOrEmpty(content)) return "";
      var newName = content.Substring(0, 1).ToLower() + content.Substring(1);
      return newName.UrlSafe();
    }

    public static string AsUpperCaseFirstLetter(this string content) {
      if (string.IsNullOrEmpty(content)) return "";
      var newName = content.Substring(0, 1).ToUpper() + content.Substring(1);
      return newName.UrlSafe();
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


    public static List<string> SplitCamelCase(this string input) {
      if (string.IsNullOrEmpty(input))
        return new List<string>();

      var parts = new List<string>();
      var currentWord = new StringBuilder();

      for (int i = 0; i < input.Length; i++) {
        char c = input[i];
        char? next = i + 1 < input.Length ? input[i + 1] : null;
        char? prev = i > 0 ? input[i - 1] : null;

        if (char.IsUpper(c)) {
          if (currentWord.Length == 0) {
            // Start of string
            currentWord.Append(char.ToLower(c));
          } else if (prev.HasValue && char.IsUpper(prev.Value)) {
            // We're in an acronym
            if (next.HasValue && char.IsLower(next.Value)) {
              // This is the last letter of an acronym before a new word
              // e.g., "XMLHttpRequest" -> "XML" is done at 'L'
              if (currentWord.Length > 1) {
                // Remove last char and save the acronym
                var lastChar = currentWord[currentWord.Length - 1];
                currentWord.Length--; // Remove last char
                parts.Add(currentWord.ToString());
                currentWord.Clear();
                currentWord.Append(lastChar); // Start new word with last char
              }
              currentWord.Append(char.ToLower(c));
            } else {
              // Continue the acronym
              currentWord.Append(char.ToLower(c));
            }
          } else {
            // Start of a new word (prev was lowercase or digit)
            if (currentWord.Length > 0) {
              parts.Add(currentWord.ToString());
              currentWord.Clear();
            }
            currentWord.Append(char.ToLower(c));
          }
        } else if (char.IsDigit(c)) {
          if (prev.HasValue && char.IsDigit(prev.Value)) {
            // Continue number sequence
            currentWord.Append(c);
          } else {
            // Start new numeric word
            if (currentWord.Length > 0) {
              parts.Add(currentWord.ToString());
              currentWord.Clear();
            }
            currentWord.Append(c);
          }
        } else if (char.IsLetter(c)) {
          currentWord.Append(char.ToLower(c));
        }
          // Non-alphanumeric characters are treated as word boundaries
          else if (currentWord.Length > 0) {
          parts.Add(currentWord.ToString());
          currentWord.Clear();
        }
      }

      // Add the last word if any
      if (currentWord.Length > 0) {
        parts.Add(currentWord.ToString());
      }

      // Filter out empty strings and single characters (unless it's a meaningful single char)
      return parts.Where(p => p.Length > 0).ToList();
    }

    public static bool IsValidPath(this string path) { 
      if (path == null) { return false; }
      try {
        var fullPath = Path.GetFullPath(path);
        return true;
      } catch {
        return false;
      }
    }

    public static string CutMethodMarkers(this string content) {
      if (string.IsNullOrEmpty(content)) return content;
      // start marker is Cx.MethodStartMarker plan to cut everything between start of file to marker, including the marker.
      // end marker is Cx.MethodEndMarker plan to cut everything between marker and end of file, including the marker.
      var startIndex = content.IndexOf(Cx.MethodStartMarker);
      if (startIndex >= 0) {
        content = content.Substring(startIndex + Cx.MethodStartMarker.Length).TrimStart(Environment.NewLine.ToCharArray());
      } else {
        var firstBraceIndex = content.IndexOf("{");
        var lastBraceIndex = content.LastIndexOf("}");
        if (firstBraceIndex >= 0 && lastBraceIndex > firstBraceIndex) {
          content = content.Substring(firstBraceIndex + 1, lastBraceIndex - firstBraceIndex - 1);
          return content;
        }
      }

      var endIndex = content.IndexOf(Cx.MethodEndMarker);
      if (endIndex >= 0) {
        content = content.Substring(0, endIndex).TrimEnd(Environment.NewLine.ToCharArray());
      } else {
        var lastBraceIndex = content.LastIndexOf("}");
        if ( lastBraceIndex > 0) {
          content = content.Substring(0, content.Length - lastBraceIndex-1);
        }
      }
      return content;
    }

  }
}
