using Weavers.Core.Entities;
using Weavers.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Weavers.Core.Handlers.Items {
  public record GetSubgraphQuery(int itemId, int depth) : IRequest<SubgraphDto>;
  public class GetSubgraphQueryHandler(
    FabricDbContext context,
    ILogger<GetSubgraphQueryHandler> logger
  ) : IRequestHandler<GetSubgraphQuery, SubgraphDto> {
    private readonly FabricDbContext _context = context;
    private readonly ILogger<GetSubgraphQueryHandler> _logger = logger;

    public async Task<SubgraphDto> Handle(GetSubgraphQuery request, CancellationToken cancellationToken) {
      var visited = new HashSet<int>();

      var root = await _context.GetItemDtoById(request.itemId, cancellationToken);
      if (root == null) {
        var error = new KeyNotFoundException("Root item not found");
        _logger.LogError(error, "Root item with ID {ItemId} not found", request.itemId);
        throw error;
      }          

      visited.Add(root.Id);

      return new SubgraphDto {
        Root = root,
        Nodes = await BuildChildren(root, 1, request.depth, visited, cancellationToken)
      };
    }

    private async Task<ICollection<SubgraphNodeDto>> BuildChildren(
    ItemDto parent, int level, int maxDepth,
    HashSet<int> visited, CancellationToken cancellationToken) {
      if (level > maxDepth) return [];
      var nodes = new List<SubgraphNodeDto>();
      foreach (var relation in parent.Relations.Where(r => r.RelatedItemId.HasValue)) {
        var relatedId = relation.RelatedItemId!.Value;
        if (visited.Contains(relatedId)) continue;  // cycle guard
        visited.Add(relatedId);
        var child = await _context.GetItemDtoById(relatedId, cancellationToken);
        if (child == null) continue;
        nodes.Add(new SubgraphNodeDto {
          Item = child,
          Relation = relation,
          Level = level,
          Children = await BuildChildren(child, level + 1, maxDepth, visited, cancellationToken)
        });
      }
      return nodes;
    }
  }

  public class SubgraphDto {
    public ItemDto Root { get; set; } = null!;
    public ICollection<SubgraphNodeDto> Nodes { get; set; } = [];
  }

  public class SubgraphNodeDto {
    public ItemDto Item { get; set; } = null!;
    public RelationDto Relation { get; set; } = null!;
    public int Level { get; set; }
    public ICollection<SubgraphNodeDto> Children { get; set; } = [];
  }

}
