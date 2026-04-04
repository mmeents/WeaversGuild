using KB.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.Items {
  public record GetItemByIdQuery(int Id, bool IncludeRelations = false) : IRequest<ItemDto?>;

  public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto?> {
    private readonly FabricDbContext _context;
    public GetItemByIdQueryHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken) {
      // Then in the handler:
      var query = _context.Items
          .AsNoTracking()
          .Where(i => i.Id == request.Id && i.IsActive);

      if (request.IncludeRelations) {
        query = query
            .Include(i => i.ItemType)
            .Include(i => i.Relations)
                .ThenInclude(r => r.RelatedItem)
            .Include(i => i.Relations)
                .ThenInclude(r => r.RelationType)
            .Include(i => i.IncomingRelations)
                .ThenInclude(r => r.Item)
            .Include(i => i.IncomingRelations)
                .ThenInclude(r => r.RelationType);
      } else {
        query = query.Include(i => i.ItemType);
      }

      var item = await query.FirstOrDefaultAsync(cancellationToken);

      return item != null ? item.ToDto(request.IncludeRelations) : null;
    }
  }
}
