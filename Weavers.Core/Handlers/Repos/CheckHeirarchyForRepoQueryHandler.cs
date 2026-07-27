using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Repos {

  public record HierarchyContainsRepoQuery(int FolderId) : IRequest<bool>;

  public class HierarchyContainsRepoQueryHandler
      : IRequestHandler<HierarchyContainsRepoQuery, bool> {

    private static readonly HashSet<int> FolderTypes = new() {
    (int)WeItemType.ProjectFolderModel,
    (int)WeItemType.RelativeFolderModel
  };
    private const int RepoType = (int)WeItemType.GithubRepoModel;

    private readonly FabricDbContext _context;
    public HierarchyContainsRepoQueryHandler(FabricDbContext context)
      => _context = context;

    public async Task<bool> Handle(HierarchyContainsRepoQuery request, CancellationToken ct) {
      var entry = await _context.GetItemDtoById(request.FolderId, ct)
        ?? throw new Exception($"Item {request.FolderId} not found.");

      if (!FolderTypes.Contains(entry.ItemTypeId))
        throw new Exception($"Item {request.FolderId} is not a valid folder type.");

      var visited = new HashSet<int>();
      if (await DescendantHasRepo(entry, visited, ct)) return true;

      var parentId = entry.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(parentId => parentId != entry.Id);
      return parentId != 0 && await AncestorHasRepo(parentId, visited, ct);
    }

    private async Task<bool> DescendantHasRepo(ItemDto item, HashSet<int> visited, CancellationToken ct) {
      if (!visited.Add(item.Id)) return false;

      foreach (var kid in item.Relations) {
        if (kid.RelatedItemTypeId == RepoType) return true;

        if (kid.RelatedItemId.HasValue
            && kid.RelatedItemTypeId.HasValue
            && FolderTypes.Contains(kid.RelatedItemTypeId.Value)) {
          var child = await _context.GetItemDtoById(kid.RelatedItemId.Value, ct);
          if (child != null && await DescendantHasRepo(child, visited, ct)) return true;
        }
      }
      return false;
    }

    private async Task<bool> AncestorHasRepo(int itemId, HashSet<int> visited, CancellationToken ct) {
      while (true) {
        if (!visited.Add(itemId)) return false;

        var item = await _context.GetItemDtoById(itemId, ct);
        if (item == null || !FolderTypes.Contains(item.ItemTypeId)) return false;

        if (item.Relations.Any(r => r.RelatedItemTypeId == RepoType)) return true;

        var parentId = item.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(parentId => parentId != item.Id);
        if (parentId == 0) return false;
        itemId = parentId;
      }
    }
  }
}
