using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record GetRelationsQuery(
    int? Id, 
    int? ItemId, 
    int? ToItemId, 
    int? RelationTypeId
  ) : IRequest<IEnumerable<RelationDto>>;

  public class GetItemRelationsQueryHandler : IRequestHandler<GetRelationsQuery, IEnumerable<RelationDto>> {
    private readonly FabricDbContext _context;
    public GetItemRelationsQueryHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<IEnumerable<RelationDto>> Handle(GetRelationsQuery request, CancellationToken cancellationToken) {
      var query = _context.Relations
        .Include(ir => ir.Item)
        .Include(ir => ir.RelatedItem)
        .Include(ir => ir.RelationType)
        .AsNoTracking()
        .AsQueryable();

      if (request.Id.HasValue) {
        query = query.Where(ir => ir.Id == request.Id.Value);
      }
      if (request.ItemId.HasValue) {
        query = query.Where(ir => ir.ItemId == request.ItemId.Value);
      }
      if (request.ToItemId.HasValue) {
        query = query.Where(ir => ir.RelatedItemId == request.ToItemId.Value);
      }
      if (request.RelationTypeId.HasValue) {
        query = query.Where(ir => ir.RelationTypeId == request.RelationTypeId.Value);
      }

      query = query.OrderByDescending(ir => ir.Id);

      var result = await query.Select(r => new RelationDto {
        Id = r.Id,
        ItemId = r.ItemId,
        ItemName = r.Item.Name,
        RelatedItemId = r.RelatedItemId,
        RelatedItemName = r.RelatedItem != null ? r.RelatedItem.Name : "",
        RelationTypeId = r.RelationTypeId,
        RelationTypeName = r.RelationType.Name ?? string.Empty,
        Rank = r.Rank,
        Established = r.Established,
        RelatedItemHasChildren = r.RelatedItem != null && r.RelatedItem.Relations.Any(cr => cr.RelationTypeId == (int)WeRelationTypes.Contains)
      }).ToListAsync(cancellationToken);

      return result;
    }
  }
}
