using LibGit2Sharp;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Repos {

  public record RefreshGitStatusCommand(int RepoId) : IMcpRequest, IRequest<ItemDto?>;
  public class RefreshGitStatusHandler : IRequestHandler<RefreshGitStatusCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public RefreshGitStatusHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<ItemDto?> Handle(RefreshGitStatusCommand request, CancellationToken ct) {

      var repoItem = await _context.GetItemDtoById(request.RepoId, ct);
      repoItem.ValidateRepoItemExists(request.RepoId);
      var parentItemId = repoItem.GetParentId();
      var parentItem = await _context.GetItemDtoById(parentItemId, ct);
      var parentItemPath = parentItem.ValidateRepoParentFolder(request.RepoId, true);
      bool isGoForSync = false;
        
      using (var repo = new Repository(parentItemPath)) {
        var currentBranch = repo.Head.FriendlyName;
        var remote = repo.Network.Remotes.FirstOrDefault();
        var remoteName = remote?.Name ?? "origin";
              
        var status = repo.RetrieveStatus();  // Get repository status
        var isDirty = status.IsDirty;
        var modifiedCount = status.Modified.Count();
        var untrackedCount = status.Untracked.Count();
        var lastCommit = repo.Head.Tip;  // was TrackedBranch.Tip, but that can be null if the branch is not tracking a remote branch

        var remoteNameProp = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRemoteName);
        if (remoteNameProp != null) {
          var updated0 = await _mediator.Send(new UpdateItemPropertyCommand(remoteNameProp.Id, remoteName), ct);
        }

        var lastCommitShaProp = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLastCommitSha);
        if (lastCommitShaProp != null && lastCommit != null) {
          var updated2 = await _mediator.Send(new UpdateItemPropertyCommand(lastCommitShaProp.Id, lastCommit.Sha), ct);          
        }

        var lastStatusChkProp = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLastStatusChk);
        if (lastStatusChkProp != null) {
          string lastStatusChkValue = DateTime.UtcNow.ToString("o");
          var updated3 = await _mediator.Send(new UpdateItemPropertyCommand(lastStatusChkProp.Id, lastStatusChkValue), ct);
        }

        var isDirtyProp = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsDirty);
        if (isDirtyProp != null) {
          var updated4 = await _mediator.Send(new UpdateItemPropertyCommand(isDirtyProp.Id, isDirty ? "1" : "0"), ct);
        }

        var modifiedCountProp = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItModifiedCount);
        if (modifiedCountProp != null) {
          var updated5 = await _mediator.Send(new UpdateItemPropertyCommand(modifiedCountProp.Id, modifiedCount.ToString()), ct);
        }

        var untrackedCountProp = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUntrackedFiles);
        if (untrackedCountProp != null) {
          var updated6 = await _mediator.Send(new UpdateItemPropertyCommand(untrackedCountProp.Id, untrackedCount.ToString()), ct);
        }

        int currentBranchId = 0;
        HashSet<int> repoBranches = new HashSet<int>();
        foreach (var branch in repo.Branches) {
          if (branch.FriendlyName.EndsWith("/HEAD")) continue;
          var branchItem = await AddBranchAsChildItem(repoItem, branch, ct);
          if (branchItem == null) continue;
          repoBranches.Add(branchItem.Id);          
          if (branch.FriendlyName == currentBranch) currentBranchId = branchItem.Id;
        }

        var staleRelations = repoItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.GithubRepoBranchModel 
          && r.RelatedItemId.HasValue && !repoBranches.Contains(r.RelatedItemId.Value)).ToList();
        foreach(var relation in staleRelations) {
          if (relation.RelatedItemId.HasValue) {
            var deleted = await _mediator.Send(new ArchiveItemCommand(relation.RelatedItemId.Value, true), ct);
          }
        }

        var currentBranchProp = repoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItCurrentBranch);
        if (currentBranchProp != null && currentBranchId != 0) {
          repoItem = await _mediator.Send(new UpdateItemPropertyCommand(currentBranchProp.Id, currentBranchId.ToString()), ct);
        }

        isGoForSync = true;
      }

      if (isGoForSync) {
        var syncCommand = new SyncRepoCommand(repoItem.Id);
        var syncResult = await _mediator.Send(syncCommand, ct);        
      }
      return repoItem;
    }

    private async Task<ItemDto?> AddBranchAsChildItem(ItemDto repoItem, Branch branch, CancellationToken cancellationToken) {
      var isRemote = branch.IsRemote;
      var branchName = isRemote ? branch.FriendlyName.Replace("origin/", "") : branch.FriendlyName;
      var trackingBranch = branch.TrackedBranch?.FriendlyName;
      var friendlyName = branch.FriendlyName;
      var lastCommit = branch.Tip;
      var lastCommitSha = lastCommit?.Sha;
      var lastCommitMessage = lastCommit?.MessageShort;
      var lastCommitAuthor = lastCommit?.Author.Name;
      var lastCommitDate = lastCommit?.Author.When.DateTime;

      // Check if the branch already exists as a child item
      ItemDto? existingBranchItem = null;
      foreach (var r in repoItem.Relations.Where(r => r.RelatedItemId.HasValue)) {
        var item = await _context.GetItemDtoById(r.RelatedItemId!.Value, cancellationToken);
        if (item?.ItemTypeId != (int)WeItemType.GithubRepoBranchModel) continue;
        if (item.Properties.Any(p => p.Name == Cx.ItFriendlyName && p.Value == friendlyName)) {
          existingBranchItem = item; break;
        }
      }
      ItemDto? updateBranch = null;
      if (existingBranchItem != null) {
        var shaProp = existingBranchItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLastCommitSha);
        if (shaProp != null && shaProp.Value != lastCommitSha) {
            updateBranch = existingBranchItem;          
        }
      } else { // Create new branch item
        updateBranch = await _mediator.Send(
          new CreateRelatedItemCommand(repoItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.GithubRepoBranchModel, branchName, "", "{}"), cancellationToken);
      }
      if (updateBranch != null) {

        var branchNameProp = updateBranch.Properties.FirstOrDefault(p => p.Name == Cx.ItBranchName);
        if (branchNameProp != null && branchNameProp.Value != branchName) {
          updateBranch = await _mediator.Send(new UpdateItemPropertyCommand(branchNameProp.Id, branchName), cancellationToken);
        }

        var friendlyNameProp = updateBranch!.Properties.FirstOrDefault(p => p.Name == Cx.ItFriendlyName);
        if (friendlyNameProp != null && friendlyNameProp.Value != friendlyName) {
          updateBranch = await _mediator.Send(new UpdateItemPropertyCommand(friendlyNameProp.Id, friendlyName), cancellationToken);
        }

        var isRemoteProp = updateBranch!.Properties.FirstOrDefault(p => p.Name == Cx.ItIsRemote);
        if (isRemoteProp != null && isRemoteProp.Value != (isRemote ? "1" : "0")) {
          updateBranch = await _mediator.Send(new UpdateItemPropertyCommand(isRemoteProp.Id, isRemote ? "1" : "0"), cancellationToken);
        }

        var trackingBranchProp = updateBranch!.Properties.FirstOrDefault(p => p.Name == Cx.ItTrackedBranchName);
        if (trackingBranchProp != null && trackingBranchProp.Value != trackingBranch) {
          updateBranch = await _mediator.Send(new UpdateItemPropertyCommand(trackingBranchProp.Id, trackingBranch ?? ""), cancellationToken);
        }

        var lastCommitShaProp = updateBranch!.Properties.FirstOrDefault(p => p.Name == Cx.ItLastCommitSha);
        if (lastCommitShaProp != null && lastCommitShaProp.Value != lastCommitSha) {
          updateBranch = await _mediator.Send(new UpdateItemPropertyCommand(lastCommitShaProp.Id, lastCommitSha ?? ""), cancellationToken);
        }

        var lastCommitMessageProp = updateBranch!.Properties.FirstOrDefault(p => p.Name == Cx.ItLastCommitMessage);
        if (lastCommitMessageProp != null && lastCommitMessageProp.Value != lastCommitMessage) {
          updateBranch = await _mediator.Send(new UpdateItemPropertyCommand(lastCommitMessageProp.Id, lastCommitMessage ?? ""), cancellationToken);
        }

        var lastCommitAuthorProp = updateBranch!.Properties.FirstOrDefault(p => p.Name == Cx.ItLastCommitAuthor);
        if (lastCommitAuthorProp != null && lastCommitAuthorProp.Value != lastCommitAuthor) {
          updateBranch = await _mediator.Send(new UpdateItemPropertyCommand(lastCommitAuthorProp.Id, lastCommitAuthor ?? ""), cancellationToken);
        }

        var lastCommitDateProp = updateBranch!.Properties.FirstOrDefault(p => p.Name == Cx.ItLastCommitDate);
        if (lastCommitDateProp != null && lastCommitDateProp.Value != lastCommitDate?.ToString("o")) {
          updateBranch = await _mediator.Send(new UpdateItemPropertyCommand(lastCommitDateProp.Id, lastCommitDate?.ToString("o") ?? ""), cancellationToken);
        }

      }

      if (existingBranchItem != null && updateBranch == null) { // Return null on existing will archive.
        updateBranch = existingBranchItem;  // we branch around if sha didn't change, but we still want to return the existing branch item for reference.
      }

      return updateBranch;
    }
  }
}
