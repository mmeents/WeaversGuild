using LibGit2Sharp;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Repos {
  public record CloneGithubRepoCommand(int RepoId) : IMcpRequest, IRequest<ItemDto?>;
  public class CloneGithubRepoCommandHandler : IRequestHandler<CloneGithubRepoCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    private readonly ICryptoService _cryptoService;
    public CloneGithubRepoCommandHandler(IMediator mediator, FabricDbContext context, ICryptoService cryptoService) {
      _mediator = mediator;
      _context = context;
      _cryptoService = cryptoService;
    }

    public async Task<ItemDto?> Handle(CloneGithubRepoCommand request, CancellationToken cancellationToken) {

      var repoItem = await _context.GetItemDtoById(request.RepoId, cancellationToken);
      repoItem.ValidateRepoItemExists(request.RepoId);
      var parentItemId = repoItem.GetParentId();      
      var parentItem = await _context.GetItemDtoById(parentItemId, cancellationToken);
      var parentItemPath = parentItem.ValidateRepoParentFolder(request.RepoId, false);

      var remoteUrl = repoItem!.Properties.FirstOrDefault(p => p.Name == Cx.ItRepoUrl)?.Value;
      if (string.IsNullOrEmpty(remoteUrl)) {
        throw new Exception($"Repo with ID {request.RepoId} does not have a valid remote URL.");
      }

      var gitCredsItemId = repoItem.GithubCredsId(request.RepoId);
      var gitCredsItem = await _context.GetItemDtoById(gitCredsItemId, cancellationToken);
      var (gitUsername, gitPat) = gitCredsItem.GetGitCredentials(gitCredsItemId, request.RepoId, _cryptoService);

    
      if (!Directory.Exists(parentItemPath)) {
        Directory.CreateDirectory(parentItemPath);
      }
      if (!Directory.Exists(parentItemPath)) {
        throw new Exception($"Parent item path {parentItemPath} for repo with ID {request.RepoId} could not be created.");
      }

      parentItemPath.ValidateNoRepoFolderExists();      

      // ancestors — catches cloning inside a repo the graph doesn't know about
      for (var d = new DirectoryInfo(parentItemPath).Parent; d != null; d = d.Parent)
        if (Directory.Exists(Path.Combine(d.FullName, ".git")))
          throw new Exception($"Target is inside an existing repository at {d.FullName}.");

      var cloneOptions = new CloneOptions {
        FetchOptions = {
          CredentialsProvider = (r, u, t) => {
            return new UsernamePasswordCredentials() {
              Username = gitUsername,
              Password = gitPat
            };
          }
        }
      };

      Repository.Clone(remoteUrl, parentItemPath, cloneOptions);

      var refreshedRepoItem = await _mediator.Send(new RefreshGitStatusCommand(repoItem.Id), cancellationToken);

      return refreshedRepoItem;
    }

  
  }
}
