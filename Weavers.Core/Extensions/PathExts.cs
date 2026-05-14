using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Extensions {
  public static class PathExts {
    public static PathValidity GetPathValidity(this string? path) {
      if (string.IsNullOrWhiteSpace(path))
        return PathValidity.Invalid;

      try {
        var fullPath = Path.GetFullPath(path);
        
        if (!Path.IsPathRooted(fullPath))
          return PathValidity.Invalid;

        return (Directory.Exists(fullPath) || File.Exists(fullPath))
            ? PathValidity.ValidAndExists : PathValidity.ValidNotFound;

      } catch (ArgumentException) { 
        return PathValidity.Invalid; 
      } catch (NotSupportedException) { 
        return PathValidity.Invalid; 
      } catch (PathTooLongException) {
        return PathValidity.Invalid; 
      }
    }

    // Convenience wrapper — "can I use this path?" (exists OR could be created)
    public static bool IsValidPath(this string? path) => path.GetPathValidity() != PathValidity.Invalid;

    // "Does this path exist and is the string well-formed?"
    public static bool IsExistingPath(this string? path) =>  path.GetPathValidity() == PathValidity.ValidAndExists;

    public static bool ValidatePath(this string path) {
      var validity = path.GetPathValidity();
      if (validity == PathValidity.Invalid) {
        throw new ArgumentException($"The provided path '{path}' is not valid.");
      }
      if (validity == PathValidity.ValidNotFound) {
        try {
         var info = Directory.CreateDirectory(path);
         validity = path.GetPathValidity(); // re-check validity after creation attempt
        } catch (Exception ex) {
          throw new IOException($"Failed to create directory '{path}'.", ex);
        }
      }
      return validity == PathValidity.ValidAndExists;
    }

  }

  public enum PathValidity {
    Invalid,        // string is not a valid path format
    ValidNotFound,  // valid format, does not exist on disk
    ValidAndExists  // valid format, exists on disk
  }


}
