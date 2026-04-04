using KB.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Handlers.Items {
  public record UpdateItemRelationCommand(
      int Id,
      int ItemId,
      int RelationTypeId,
      int RelatedItemId,
      int? Rank = null
  ) : IRequest<ItemRelationDto?>;
  public class UpdateItemRelationCommandHandler : IRequestHandler<UpdateItemRelationCommand, ItemRelationDto?> {
    private readonly FabricDbContext _context;
    public UpdateItemRelationCommandHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<ItemRelationDto?> Handle(UpdateItemRelationCommand request, CancellationToken cancellationToken) {
      var itemRelation = await _context.ItemRelations.FindAsync(request.Id);
      if (itemRelation == null) {
        throw new KeyNotFoundException("Item relation not found");
      }
      itemRelation.ItemId = request.ItemId;
      itemRelation.RelationTypeId = request.RelationTypeId;
      itemRelation.RelatedItemId = request.RelatedItemId;
      itemRelation.Rank = request.Rank;
      await _context.SaveChangesAsync(cancellationToken);

      var query = _context.ItemRelations
       .Include(ir => ir.Item)
       .Include(ir => ir.RelatedItem)
       .Include(ir => ir.RelationType)
       .AsNoTracking()
       .AsQueryable();
      query = query.Where(ir => ir.Id == itemRelation.Id);
      var result = await query.FirstOrDefaultAsync(cancellationToken);
      return result?.ToDto();
    }
  }
}
