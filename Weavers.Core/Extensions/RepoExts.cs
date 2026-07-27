using Microsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Extensions {
  public static class RepoExts {
    public static bool ValidateRepoItemExists(this ItemDto? repoItem, int repoId) {
      if (repoItem == null) {
        throw new Exception($"Repo with ID {repoId} not found.");
      }

      if (repoItem.ItemTypeId != (int)WeItemType.GithubRepoModel) {
        throw new Exception($"Item with ID {repoId} is not a GitHub repository model.");
      }
      return true;      
    }

    public static bool ValidateBranchItemExists(this ItemDto? branchItem, int branchId) {
      if (branchItem == null) {
        throw new Exception($"Branch with ID {branchId} not found.");
      }
      if (branchItem.ItemTypeId != (int)WeItemType.GithubRepoBranchModel) {
        throw new Exception($"Item with ID {branchId} is not a GitHub repository branch model.");
      }

      return true;
    }

    public static bool GitRepoFolderExists(this string folderPath) {
      if (string.IsNullOrEmpty(folderPath)) {
        return false;
      }
      if (Directory.Exists(folderPath)) { 
        return true;
      } else {
        return false;
      }
    }

    public static bool ValidateRepoFolderExists(this string folderPath) {
      if (string.IsNullOrEmpty(folderPath)) {
        throw new Exception("ValidateRepoFolderExists param folderPath is null or empty.");
      }
      if (!Directory.Exists(Path.Combine(folderPath, ".git"))) {
        throw new Exception($"A Git repository does not exist at {folderPath}.");
      }
      return true;
    }

    public static bool ValidateNoRepoFolderExists(this string folderPath) { 
      if (string.IsNullOrEmpty(folderPath)) {
        throw new Exception("ValidateNoRepoFolderExists param folderPath is null or empty.");
      }
      if (Directory.Exists(Path.Combine(folderPath, ".git"))) { 
        throw new Exception($"A Git repository already exists at {folderPath}.");
      }
      return true;
    }

    public static int GetRepoCurrentBranchId(this ItemDto? repoItem, int repoId) {
      if (repoItem == null) {
        throw new Exception($"Repo with ID {repoId} not found.");
      }
      var currentBranchId = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCurrentBranch)?.Value.AsInt32();
      if (currentBranchId == null || currentBranchId == 0) {
        throw new Exception($"Repo with ID {repoId} does not have a valid current branch ID.");
      }
      return currentBranchId.Value;
    }
    public static string FriendlyName(this ItemDto? item) {
      if (item == null) throw new Exception("FriendlyName param Item is null.");
      return item.Properties.FirstOrDefault(p => p.Name == Cx.ItFriendlyName)?.Value ?? item.Name;
    }
    public static int GetParentId(this ItemDto? item) {
      if (item == null) throw new Exception("GetParentId param Item is null.");
      var parentId = item.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(parentId => parentId != item.Id);
      if (parentId == 0) {
        throw new Exception($"Parent item for item with ID {item.Id} not found.");
      }
      return parentId;
    }

    public static bool IsRepoDirty(this ItemDto? repoItem, int repoId) {
      if (repoItem == null) {
        throw new Exception($"Repo with ID {repoId} not found.");
      }
      var isDirty = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsDirty)?.Value;
      if (isDirty == null) {
        throw new Exception($"Repo with ID {repoId} does not have a valid IsDirty property.");
      }
      return isDirty.AsBoolean();
    }

    public static string ValidateRepoParentFolder(this ItemDto? parentItem, int repoId, bool CheckGitFolderExists = true) {
      if (parentItem == null) {
        throw new Exception($"Parent item for repo with ID {repoId} not found.");
      }

      var parentItemPath = parentItem.ResolveParentFolderPath("");

      if (parentItemPath == null) {
        throw new Exception($"Parent item path for repo with ID {repoId} could not be resolved.");
      }

      if (string.IsNullOrEmpty(parentItemPath)) {
        throw new Exception($"Parent item path for repo with ID {repoId} is not valid.");
      }

      if (CheckGitFolderExists) {
        if (!Directory.Exists(parentItemPath)) {
          throw new Exception($"Parent item path {parentItemPath} for repo with ID {repoId} does not exist.");
        }
        var repoGitPath = Path.Combine(parentItemPath, ".git");
        if (!Directory.Exists(repoGitPath))
          throw new Exception($"missing Git repository internals .git folder {repoGitPath}.");
      }
      return parentItemPath;
    }

    public static (string gitUser, string gitPAT) GetGitCredentials(this ItemDto? gitCredsItem, int gitCredsItemId, int repoId, ICryptoService _cryptoService) {

      if (gitCredsItem == null) {
        throw new Exception($"GitHub credentials item with ID {gitCredsItemId} not found for repo with ID {repoId}.");
      }

      var gitUsername = gitCredsItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGithubUser)?.Value;
      if (gitUsername == null || string.IsNullOrEmpty(gitUsername)) {
        throw new Exception($"GitHub username not found or empty in credentials item with ID {gitCredsItemId} for repo with ID {repoId}.");
      }
      var gitPatCipher = gitCredsItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGithubPAT)?.Value;
      if (gitPatCipher == null || string.IsNullOrEmpty(gitPatCipher)) {
        throw new Exception($"GitHub personal access token not found or empty in credentials item with ID {gitCredsItemId} for repo with ID {repoId}.");
      }
      var gitPat = _cryptoService.Decrypt(gitPatCipher);
      return (gitUsername, gitPat);
    }

    public static int GithubCredsId(this ItemDto? repoItem, int repoId) {
      if (repoItem == null) {
        throw new Exception($"Repo with ID {repoId} not found.");
      }
      var gitCredsItemId = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGithubCreds)?.Value.AsInt32();
      if (gitCredsItemId == null || gitCredsItemId == 0) {
        throw new Exception($"Repo with ID {repoId} does not have a valid GitHub credentials item ID.");
      }
      return gitCredsItemId.Value;
    }

  }
}
