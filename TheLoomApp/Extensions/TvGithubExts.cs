using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace TheLoomApp.Extensions {
  public static class TvGithubExts {

    public static async Task AddGithubToken(this TreeView _tv, IAppGraphOrgService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.CredentialStoreModel) {
        throw new InvalidOperationException("Invalid node selected for adding GitHub token.");
      }
      var newSubItem = await graphSrvs.AddGithubToken(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddGithubRepo(this TreeView _tv, IAppGraphOrgService graphSrvs, string name, int? credItemId = null, string? remoteUrl = null) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null
        || (item.ItemTypeId != (int)WeItemType.ProjectFolderModel) && (item.ItemTypeId != (int)WeItemType.RelativeFolderModel)) {
        throw new InvalidOperationException("Invalid node selected for adding GitHub repository.");
      }
      var newSubItem = await graphSrvs.AddGithubRepo(item, name, credItemId, remoteUrl);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task UpdateRepoItem(this TreeView _tv, ItemDto? repoItem) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      ItemNode selectedNode = _selectedNode ?? throw new InvalidOperationException("No node selected.");
      selectedNode.Item = repoItem;
      _tv.ReExpandSelectNode(repoItem!.Id);
    }



  }
}
