using LibGit2Sharp;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Repos {

  public record SyncRepoCommand(int RepoItemId) : IMcpRequest, IRequest<SyncRepoCmdResult>;

  public class SyncRepoCmdResult {
    public ItemDto? RepoItem { get; set; }
    public ItemDto? ParentItem { get; set; } 
    public List<DbGitEntryItem> AddedNodes { get; set; } = new List<DbGitEntryItem>();
    public List<DbGitEntryItem> UpdatedNodes { get; set; } = new List<DbGitEntryItem>();
    public List<DbGitEntryItem> DeletedNodes { get; set; } = new List<DbGitEntryItem>();
    public List<string> Errors { get; set; } = new List<string>();
  }

  public class SyncRepoCommandHandler : IRequestHandler<SyncRepoCommand, SyncRepoCmdResult> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    private readonly IFileSystemValidationService _fileSystemValidationService;
    public SyncRepoCommandHandler(IMediator mediator, FabricDbContext context, IFileSystemValidationService fileSystemValidationService) {
      _mediator = mediator;
      _context = context;
      _fileSystemValidationService = fileSystemValidationService;
    }

    public async Task<SyncRepoCmdResult> Handle(SyncRepoCommand request, CancellationToken ct) {
      var callResult = new SyncRepoCmdResult();
      try {

        var repoItem = await _context.GetItemDtoById(request.RepoItemId, ct);
        repoItem.ValidateRepoItemExists(request.RepoItemId);
        callResult.RepoItem = repoItem;
        var parentId = repoItem.GetParentId();        
        var parentItem = await _context.GetItemDtoById(parentId, ct);
        callResult.ParentItem = parentItem;
        var parentItemPath = parentItem.ValidateRepoParentFolder(request.RepoItemId, true);


        // load from db
        List<DbGitEntryItem> existingNodes = await _mediator.Send(new GetRepoFolderItemsQuery(request.RepoItemId, parentId), ct) ?? new List<DbGitEntryItem>();
        var existingByPath = existingNodes.ToDictionary(e => e.GitPath, StringComparer.OrdinalIgnoreCase);

        // load from git
        List<GitEntryItem> fileSystemState = new List<GitEntryItem>();
        using (var repo = new Repository(parentItemPath)) {
          var tree = repo.Head.Tip.Tree;
          foreach ((string Path, bool IsDir, Blob? Blob) in WalkTree(tree)) {
            if (Blob != null) {
              fileSystemState.Add(Blob.ToEntry(IsDir, Path));
            } else if (IsDir) {
              fileSystemState.Add(tree.ToEntryFolder(Path));
            }
          }
        }
        var fileSystemByPath = fileSystemState.ToDictionary(e => e.GitPath, StringComparer.OrdinalIgnoreCase);


        // Determine what changed.
        var toAdd = new List<GitEntryItem>();
        var toDelete = new List<DbGitEntryItem>();
        var toUpdate = new List<DbGitEntryItem>();

        // Find additions and updates
        foreach (var fsNode in fileSystemState) {
          if (existingByPath.TryGetValue(fsNode.GitPath, out var existingNode)) {
            // Node exists - check if it needs updating
            if (NeedsUpdate(existingNode, fsNode)) {
              existingNode.UpdateMetadata(fsNode);
              toUpdate.Add(existingNode);
            }
          } else {
            // New node
            toAdd.Add(fsNode);
          }
        }

        // Find deletions
        foreach (var existingNode in existingNodes) {
          if (!fileSystemByPath.ContainsKey(existingNode.GitPath)) {
            toDelete.Add(existingNode);
          }
        }

        // Step 4: Apply changes in correct order
        // Delete files first (bottom-up), then directories
        var filesToDelete = toDelete.Where(n => !n.IsDir).ToList();
        var dirsToDelete = toDelete.Where(n => n.IsDir).OrderByDescending(n => n.GitPath.Length).ToList();

        foreach (var node in filesToDelete) {
          await _mediator.Send(new DeleteItemCommand(node.Id), ct);
          callResult.DeletedNodes.Add(node);
        }

        foreach (var node in dirsToDelete) {
          await _mediator.Send(new DeleteItemCommand(node.Id), ct);
          callResult.DeletedNodes.Add(node);
        }

        // Add directories first (top-down), then files
        var dirsToAdd = toAdd.Where(n => n.IsDir).OrderBy(n => n.GitPath.Length).ToList();
        var filesToAdd = toAdd.Where(n => !n.IsDir).ToList();

        await AddNodesWithParentResolution(repoItem!.Id, dirsToAdd, existingByPath, callResult, ct);
        await AddNodesWithParentResolution(repoItem.Id, filesToAdd, existingByPath, callResult, ct);

        foreach (var node in toUpdate) {
          try {
            await UpdateGitNode(node, repoItem.Id, ct);
            callResult.UpdatedNodes.Add(node);
          } catch (Exception ex) {
            callResult.Errors.Add($"Error updating GitNode: {node.GitPath}, Id: {node.Id}. Exception: {ex.Message}");
          }
        }

      } catch (Exception ex) {
        callResult.Errors.Add($"Error syncing repo: {ex.Message}");
      }

      return callResult;
    }

    private async Task AddNodesWithParentResolution(
      int RepoItemId,
      List<GitEntryItem> nodes,
      Dictionary<string, DbGitEntryItem> existingByPath,
      SyncRepoCmdResult callResult,
      CancellationToken ct) 
    {
      // Group by depth level (number of path separators)
      var nodesByDepth = nodes
          .GroupBy(n => n.GitPath.Count(c => c == Path.DirectorySeparatorChar))
          .OrderBy(g => g.Key)
          .ToList();


      // Process level by level so parents always exist before children
      foreach (var level in nodesByDepth) {        
        foreach (var node in level) {
          try {
            // Resolve parent ID by finding parent directory path
            var parentPath = GetParentPath(node.GitPath);

            if (!string.IsNullOrEmpty(parentPath)) {
              // Check if parent exists in DB (including nodes we just added)
              if (existingByPath.TryGetValue(parentPath, out var parentNode)) {
                var addedNode = await AddGitNode(node, RepoItemId, parentNode.Id, callResult, ct);
                if (addedNode != null) {                  
                  existingByPath[node.GitPath] = addedNode;
                  callResult.AddedNodes.Add(addedNode);
                }
              }
            } else { // Root level item, use the repo item as parent
              var addedNode = await AddGitNode(node, RepoItemId, callResult.ParentItem!.Id, callResult, ct);
              if (addedNode != null) {                
                existingByPath[node.GitPath] = addedNode;
                callResult.AddedNodes.Add(addedNode);
              }
            }

          } catch (Exception ex) {
            callResult.Errors.Add($"Error adding FileSystemNode: {node.GitPath}. Exception: {ex.Message}");            
          }
        }
        
      }
    }

    private async Task<DbGitEntryItem> AddGitNode(GitEntryItem node,
      int RepoItemId, int ParentId, SyncRepoCmdResult callResult, CancellationToken ct) {

      var newType = node.IsDir ? WeItemType.GitFolderModel : WeItemType.GitFileModel;
      var gitPath = node.GitPath.TrimEnd('/');
      string name;
      string ext = "";

      if (node.IsDir) {        
        name = gitPath.ParseLast("/");
      } else {
        // File → get filename without extension, but handle dotfiles correctly
        var fileName = Path.GetFileName(gitPath.Replace('/', Path.DirectorySeparatorChar));

        if (string.IsNullOrEmpty(fileName)) {
          name = "";
          ext = "";
        } else if (fileName.StartsWith('.') && fileName.IndexOf('.', 1) == -1) {
          // Pure dotfile like ".gitignore", ".env", ".dockerignore"
          name = fileName;
          ext = "";
        } else {
          name = fileName;
          ext = Path.GetExtension(fileName);
        }
      }
      
      if (string.IsNullOrEmpty(name)) name = gitPath.ParseLast("/");  // Final safety net

      var repoRoot = callResult.ParentItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder)?.Value;
      if (repoRoot == null) throw new Exception($"Parent item with ID {ParentId} does not have a valid relative folder property.");
            

      var addedNode = await _mediator.Send( new CreateRelatedItemCommand(ParentId,    // this is create under the parent folder
        (int)WeRelationTypes.Contains, (int)newType, name, "", "{}"), ct);

      if (addedNode == null) throw new Exception($"Failed to create item for {node.GitPath}");

      if (newType == WeItemType.GitFolderModel) {
        var itRelativeFolderProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
        if (itRelativeFolderProp != null) {
          itRelativeFolderProp.Value = Path.Combine(repoRoot, node.GitPath.Replace('/', Path.DirectorySeparatorChar));
          await _mediator.SetProperty(addedNode, itRelativeFolderProp.Name, itRelativeFolderProp.Value);
        }
      } else {
        var itFilePathProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (itFilePathProp != null) {
          itFilePathProp.Value = Path.Combine(repoRoot, node.GitPath.Replace('/', Path.DirectorySeparatorChar));          
          await _mediator.SetProperty(addedNode, itFilePathProp.Name, itFilePathProp.Value);
        }

        var itSizeProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItFileSize);
        if (itSizeProp != null) {
          itSizeProp.Value = node.Size.ToString();
          await _mediator.SetProperty(addedNode, itSizeProp.Name, itSizeProp.Value);
        }

        var itBinaryProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItIsBinary);
        if (itBinaryProp != null) {
          itBinaryProp.Value = node.IsBinary ? "1" : "0";
          await _mediator.SetProperty(addedNode, itBinaryProp.Name, itBinaryProp.Value);
        }

        var itExtProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItFileExt);
        if (itExtProp != null) {
          itExtProp.Value = ext;
          await _mediator.SetProperty(addedNode, itExtProp.Name, itExtProp.Value);
        }
      }

      var itGitPathProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItGitPath);
      if (itGitPathProp != null) {
        itGitPathProp.Value = node.GitPath;
        await _mediator.SetProperty(addedNode, itGitPathProp.Name, itGitPathProp.Value);
      }

      var itGitShaProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItEntrySha);
      if (itGitShaProp != null) {
        itGitShaProp.Value = node.Sha;
        await _mediator.SetProperty(addedNode, itGitShaProp.Name, itGitShaProp.Value);
      }

      var itRepoItemProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItRepoItemId);
      if (itRepoItemProp != null) {
        itRepoItemProp.Value = RepoItemId.ToString();
        await _mediator.SetProperty(addedNode, itRepoItemProp.Name, itRepoItemProp.Value);
      }

      if (newType == WeItemType.GitFileModel) {
        var itFilePathProp = addedNode.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (itFilePathProp != null) {
          var filename = itFilePathProp.Value;
          if (filename != null && File.Exists(filename) && node.Size > 0 && !node.IsBinary && node.Size < Cx.MaxImportFileSize) {
            string fileContent = await File.ReadAllTextAsync(filename, ct);
            addedNode.Description = fileContent;
            await _mediator.Send(addedNode.ToUpdateCmd(), ct);
          }
        }
      }

      var result = new DbGitEntryItem {
        Id = addedNode.Id,
        Name = name,
        ParentId = ParentId,
        ItemTypeId = (int)newType,
        IsDirStr = node.IsDir ? "1" : "0",
        GitPath = node.GitPath,
        Sha = node.Sha,
        Size = node.Size,
        IsBinaryStr = node.IsBinary ? "1" : "0"
      };

      return result;
    }

    private async Task UpdateGitNode(DbGitEntryItem node, int RepoItemId, CancellationToken ct) {

      var existingItem = await _context.GetItemDtoById(node.Id, ct);
      if (existingItem == null) { throw new Exception($"Item with ID {node.Id} failed to load."); }
      var newType = node.IsDir ? WeItemType.GitFolderModel : WeItemType.GitFileModel;
      var doesItemTypeChange = (existingItem.ItemTypeId == (int)newType);        

      var existingParentId = node.ParentId;
      var parentItem = await _context.GetItemDtoById(existingParentId, ct);
      if (parentItem == null) { throw new Exception($"Parent item with ID {existingParentId} failed to load."); }
      if (!parentItem.ItemTypeId.IsValidGitFolderParent()) throw new Exception($"Parent item with ID {existingParentId} is not a folder.");
      var name = Path.GetFileName(Path.GetFileNameWithoutExtension(node.GitPath)).NameSafe().AsUpperCaseFirstLetter();
      var ext = Path.GetExtension(node.GitPath);
      var parentPath = parentItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder)?.Value;
      if (parentPath == null) throw new Exception($"Parent item with ID {existingParentId} does not have a valid relative folder property.");


      if (newType == WeItemType.GitFolderModel) {
        var itRelativeFolderProp = existingItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
        if (itRelativeFolderProp != null) {
          itRelativeFolderProp.Value = Path.Combine(parentPath, existingItem.Name.UrlSafe());
          await _mediator.SetProperty(existingItem, itRelativeFolderProp.Name, itRelativeFolderProp.Value);
        }
      } else {
        var itFilePathProp = existingItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (itFilePathProp != null) {
          itFilePathProp.Value = Path.Combine(parentPath, existingItem.Name.UrlSafe() + ext);
          await _mediator.SetProperty(existingItem, itFilePathProp.Name, itFilePathProp.Value);
        }

        var itSizeProp = existingItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFileSize);
        if (itSizeProp != null) {
          itSizeProp.Value = node.Size.ToString();
          await _mediator.SetProperty(existingItem, itSizeProp.Name, itSizeProp.Value);
        }

        var itBinaryProp = existingItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsBinary);
        if (itBinaryProp != null) {
          itBinaryProp.Value = node.IsBinary ? "1" : "0";
          await _mediator.SetProperty(existingItem, itBinaryProp.Name, itBinaryProp.Value);
        }

        var itExtProp = existingItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFileExt);
        if (itExtProp != null) {
          itExtProp.Value = ext;
          await _mediator.SetProperty(existingItem, itExtProp.Name, itExtProp.Value);
        }
      }

      var itGitPathProp = existingItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGitPath);
      if (itGitPathProp != null) {
        itGitPathProp.Value = node.GitPath;
        await _mediator.SetProperty(existingItem, itGitPathProp.Name, itGitPathProp.Value);
      }

      var itGitShaProp = existingItem.Properties.FirstOrDefault(p => p.Name == Cx.ItEntrySha);
      if (itGitShaProp != null) {
        itGitShaProp.Value = node.Sha;
        await _mediator.SetProperty(existingItem, itGitShaProp.Name, itGitShaProp.Value);
      }

      var itRepoItemProp = existingItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRepoItemId);
      if (itRepoItemProp != null) {
        itRepoItemProp.Value = RepoItemId.ToString();
        await _mediator.SetProperty(existingItem, itRepoItemProp.Name, itRepoItemProp.Value);
      }
    }

    private string? GetParentPath(string relativePath) {  // relativePath is a GitPath like "src/Utils/File.cs" or "src/Utils"
      var lastSeparator = relativePath.Replace('/', Path.DirectorySeparatorChar).LastIndexOf(Path.DirectorySeparatorChar);
      if (lastSeparator <= 0) {
        return null;  // Root level item
      }
      return relativePath.Substring(0, lastSeparator);
    }

    private bool NeedsUpdate(DbGitEntryItem existing, GitEntryItem filesystem) {
      // For files, check size and modified date
      if (!existing.IsDir) {
        return existing.Size != filesystem.Size 
          || existing.Sha != filesystem.Sha;
      }

      // For directories, check modified date
      return existing.Sha != filesystem.Sha;
    }

    // Walk the tree recursively and yield each entry with its path, whether it's a directory, and the blob if it's a file.
    static IEnumerable<(string Path, bool IsDir, Blob? Blob)> WalkTree(Tree tree) {
      foreach (var e in tree) {
        switch (e.TargetType) {
          case TreeEntryTargetType.Blob:
            yield return (e.Path, false, (Blob)e.Target);
            break;
          case TreeEntryTargetType.Tree:
            yield return (e.Path, true, null);
            foreach (var c in WalkTree((Tree)e.Target)) yield return c;
            break;
          case TreeEntryTargetType.GitLink:
            break;
        }
      }
    }

  }
}
