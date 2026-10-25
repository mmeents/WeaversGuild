using MediatR;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;

namespace Weavers.Core.Handlers.Items {

  public record GetParentItemPathPropertyQuery(int ItemId) : IRequest<string?>;

  public class GetParentItemPathPropertyQueryHandler
      : IRequestHandler<GetParentItemPathPropertyQuery, string?> {

    private const int MaxDepth = 64;
    private readonly FabricDbContext _context;

    public GetParentItemPathPropertyQueryHandler(FabricDbContext context) => _context = context;

    public async Task<string?> Handle(GetParentItemPathPropertyQuery request, CancellationToken ct) {
      if (request == null) return null;

      var current = await _context.GetItemDtoById(request.ItemId, ct);
      if (current == null) return null;

      var visited = new HashSet<int> { current.Id };

      for (int depth = 0; depth < MaxDepth; depth++) {
        var parentId = current.IncomingRelations
          .FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains)?.ItemId;
        if (parentId is null) return null;                 // ran out of parents

        if (!visited.Add(parentId.Value))
          throw new InvalidOperationException(
            $"Containment cycle detected at item {parentId} walking up from {request.ItemId}.");

        var parent = await _context.GetItemDtoById(parentId.Value, ct);
        if (parent == null) return null;                   // dangling relation

        var propKey = parent.ItemTypeId.GetFolderPropertyName();
        if (string.IsNullOrEmpty(propKey)) {               // the gap: keep climbing
          current = parent;
          continue;
        }

        // Found the first ancestor that owns a path; stop here either way
        var path = parent.Properties.FirstOrDefault(p => p.Name == propKey)?.Value;
        if (string.IsNullOrEmpty(path)) return null;

        return parent.ItemTypeId.IsParentFileWithFileLikeKids()
          ? Path.GetDirectoryName(path) ?? ""
          : path;
      }

      throw new InvalidOperationException(
        $"Exceeded max containment depth ({MaxDepth}) walking up from item {request.ItemId}.");
    }
  }
}
