using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  public class GitEntryItem {
    public string GitPath { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
    public long Size { get; set; } = 0;
    public bool IsDir { get; set; } = false;
    public bool IsBinary { get; set; } = false;
  }

  public class DbGitEntryItem {
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public int ParentId { get; set; } = 0;
    public int ItemTypeId { get; set; } = 0;    
    public string IsDirStr { get; set; } = string.Empty;
    public bool IsDir => IsDirStr == "1";
    public string GitPath { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
    public long Size { get; set; } = 0;    
    public string IsBinaryStr { get; set; } = string.Empty;
    public bool IsBinary => IsBinaryStr == "1";

  }


  public static class GitEntryDtoExtensions {
    public static GitEntryItem ToEntry(this Blob blob, bool isDir, string path) { 
      var entry = new GitEntryItem() { 
        GitPath = path,
        Sha = blob.Sha,
        Size = blob.Size,
        IsDir = isDir,
        IsBinary = blob.IsBinary
      };
      return entry;
    }

    public static GitEntryItem ToEntryFolder(this Tree tree, string path) {
      var entry = new GitEntryItem() {
        GitPath = path,
        Sha = tree.Sha,
        Size = 0,
        IsDir = true,
        IsBinary = false
      };
      return entry;
    }

    public static void UpdateMetadata(this DbGitEntryItem existing, GitEntryItem newItem) {
      if (existing.IsDir != newItem.IsDir) {
        existing.IsDirStr = newItem.IsDir ? "1" : "0";
      }
      if (!existing.IsDir) {
        existing.Size = newItem.Size;
        existing.IsBinaryStr = newItem.IsBinary ? "1" : "0";
      }
      existing.Sha = newItem.Sha;
      
    }

  }


}
