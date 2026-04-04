using KB.Core.Entities;
using KB.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.Items {
  public record CreateItemRelationCommand(
      int ItemId,
      int RelationTypeId,
      int RelatedItemId
  ) : IRequest<ItemRelationDto?>;

  public class CreateItemRelationCommandHandler : IRequestHandler<CreateItemRelationCommand, ItemRelationDto?> {
    private readonly FabricDbContext _context;
    public CreateItemRelationCommandHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<ItemRelationDto?> Handle(CreateItemRelationCommand request, CancellationToken cancellationToken) {

      var nextRank = await _context.ItemRelations
        .Where(ir => ir.ItemId == request.ItemId)
        .CountAsync(cancellationToken) + 1;

      var itemRelation = new ItemRelation {
        ItemId = request.ItemId,
        RelationTypeId = request.RelationTypeId,
        RelatedItemId = request.RelatedItemId,
        Rank = nextRank
      };

      _context.ItemRelations.Add(itemRelation);
      await _context.SaveChangesAsync(cancellationToken);

      var result = await _context.ItemRelations
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
