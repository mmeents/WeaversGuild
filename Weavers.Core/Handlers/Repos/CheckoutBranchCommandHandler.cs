using MediatR;
using LibGit2Sharp;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Repos {
  public record CheckoutBranchCommand(int BranchId) : IMcpRequest, IRequest<ItemDto?>;
  public class CheckoutBranchCommandHandler : IRequestHandler<CheckoutBranchCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public CheckoutBranchCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<ItemDto?> Handle(CheckoutBranchCommand request, CancellationToken ct) {

      var branchItem = await _context.GetItemDtoById(request.BranchId, ct);
      branchItem.ValidateBranchItemExists(request.BranchId);

      var parentItemId = branchItem.GetParentId();
      var repoItem = await _context.GetItemDtoById(parentItemId, ct);
      
      var parentItemPath = repoItem.ValidateRepoParentFolder(request.BranchId, true);
      parentItemPath.ValidateRepoFolderExists(); // false will throw errors.
      repoItem = await _mediator.Send(new RefreshGitStatusCommand(parentItemId), ct);

      using (var repo = new Repository(parentItemPath)) {
        var branch = repo.Branches.FirstOrDefault(b => b.FriendlyName == branchItem.FriendlyName());
        if (branch == null) {
          throw new Exception($"Branch {branchItem!.Name} not found in repository {parentItemPath}.");
        }
        
        var isDirty = repoItem.IsRepoDirty(repoItem!.Id);
        if (isDirty) { 
          throw new Exception($"Cannot checkout branch {branchItem!.Name} because the repository has uncommitted changes.");
        }

        Branch target;
        if (branch.IsRemote) {
          // create (or get) a local tracking branch, then check that out
          var localName = branch.FriendlyName.Replace($"{repo.Head.RemoteName ?? "origin"}/", "");
          target = repo.Branches[localName]
                ?? repo.CreateBranch(localName, branch.Tip);
          repo.Branches.Update(target, b => b.TrackedBranch = branch.CanonicalName);
        } else {
          target = branch;
        }
        Commands.Checkout(repo, target);

        repoItem = await _mediator.Send(new RefreshGitStatusCommand(parentItemId), ct);
      }

      return repoItem;
    }
  }
}
