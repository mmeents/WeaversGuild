using Weavers.Core.Entities;
using Weavers.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Handlers.Items {
  public record GetSubgraphQuery(int itemId, int depth) : IRequest<SubgraphDto>;
  public class GetSubgraphQueryHandler(FabricDbContext context) : IRequestHandler<GetSubgraphQuery, SubgraphDto> {
    private readonly FabricDbContext _context = context;

    public async Task<SubgraphDto> Handle(GetSubgraphQuery request, CancellationToken cancellationToken) {
      var visited = new HashSet<int>();
      var root = await _context.Items
          .Include(i => i.ItemType)
          .Include(i => i.Relations)
              .ThenInclude(r => r.RelatedItem)
          .Include(i => i.Relations)
              .ThenInclude(r => r.RelationType)
          .FirstOrDefaultAsync(i => i.Id == request.itemId && i.IsActive, cancellationToken)
          ?? throw new KeyNotFoundException("Root item not found");

      visited.Add(root.Id);

      return new SubgraphDto {
        Root = root.ToDto(),
        Nodes = await BuildChildren(root, 1, request.depth, visited, cancellationToken)
      };
    }

    private async Task<ICollection<SubgraphNodeDto>> BuildChildren(
    Item parent, int level, int maxDepth,
    HashSet<int> visited, CancellationToken cancellationToken) {
      if (level > maxDepth) return [];

      var nodes = new List<SubgraphNodeDto>();

      foreach (var relation in parent.Relations.Where(r => r.RelatedItemId.HasValue)) {
        var relatedId = relation.RelatedItemId!.Value;
        if (visited.Contains(relatedId)) continue;  // cycle guard
        visited.Add(relatedId);

        var child = await _context.Items
            .Include(i => i.ItemType)
            .Include(i => i.Relations)
                .ThenInclude(r => r.RelatedItem)
            .Include(i => i.Relations)
                .ThenInclude(r => r.RelationType)
            .FirstOrDefaultAsync(i => i.Id == relatedId && i.IsActive, cancellationToken);

        if (child == null) continue;

        nodes.Add(new SubgraphNodeDto {
          Item = child.ToDto(),
          Relation = relation.ToDto(),
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
