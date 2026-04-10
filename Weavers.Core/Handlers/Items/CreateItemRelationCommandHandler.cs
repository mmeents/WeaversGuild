using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Entities;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record CreateItemRelationCommand(
      int ItemId,
      int RelationTypeId,
      int RelatedItemId
  ) : IRequest<RelationDto?>;

  public class CreateItemRelationCommandHandler : IRequestHandler<CreateItemRelationCommand, RelationDto  ?> {
    private readonly FabricDbContext _context;
    public CreateItemRelationCommandHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<RelationDto?> Handle(CreateItemRelationCommand request, CancellationToken cancellationToken) {

      var nextRank = await _context.Relations
        .Where(ir => ir.ItemId == request.ItemId)
        .CountAsync(cancellationToken) + 1;

      var itemRelation = new Relation {
        ItemId = request.ItemId,
        RelationTypeId = request.RelationTypeId,
        RelatedItemId = request.RelatedItemId,
        Rank = nextRank
      };

      _context.Relations.Add(itemRelation);
      await _context.SaveChangesAsync(cancellationToken);

      var result = await _context.Relations
        .Include(ir => ir.Item)
        .Include(ir => ir.RelatedItem)
        .Include(ir => ir.RelationType)
        .AsNoTracking()
        .Where(ir => ir.Id == itemRelation.Id)
        .FirstOrDefaultAsync(cancellationToken);

      return result?.ToDto();
    }
  }
}
