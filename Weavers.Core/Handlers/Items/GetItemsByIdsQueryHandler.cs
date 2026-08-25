using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record GetItemsByIdsQuery(List<int> Ids) : IRequest<List<ItemDto>>;
  public class GetItemsByIdsQueryHandler : IRequestHandler<GetItemsByIdsQuery, List<ItemDto>> {
    private readonly FabricDbContext _context;
    public GetItemsByIdsQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<List<ItemDto>> Handle(GetItemsByIdsQuery request, CancellationToken cancellationToken) {

      if (request.Ids == null || request.Ids.Count == 0) return new List<ItemDto>();
      if (request.Ids.Count >= 500) { throw new ArgumentException("GetItemsByIds list cannot contain more than 500 items.", nameof(request.Ids)); }
      HashSet<int> ids = new HashSet<int>(request.Ids);

      var dbResults = await _context.Items
        .AsNoTracking()
        .Where(i => ids.Contains(i.Id) && i.IsActive)        
        .Select(ItemProjections.ToItemDto)
        .AsSplitQuery()
        .ToListAsync(cancellationToken); // Execute database query here

      return dbResults;
    }

  }

}
