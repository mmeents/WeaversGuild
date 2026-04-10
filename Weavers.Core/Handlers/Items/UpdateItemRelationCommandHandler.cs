using Weavers.Core.Models;
using Weavers.Core.Entities;
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
  ) : IRequest<RelationDto?>;
  public class UpdateItemRelationCommandHandler : IRequestHandler<UpdateItemRelationCommand, RelationDto?> {
    private readonly FabricDbContext _context;
    public UpdateItemRelationCommandHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<RelationDto?> Handle(UpdateItemRelationCommand request, CancellationToken cancellationToken) {
      var itemRelation = await _context.Relations.FindAsync(request.Id);
      if (itemRelation == null) {
        throw new KeyNotFoundException("Item relation not found");
      }
      itemRelation.ItemId = request.ItemId;
      itemRelation.RelationTypeId = request.RelationTypeId;
      itemRelation.RelatedItemId = request.RelatedItemId;
      itemRelation.Rank = request.Rank;
      await _context.SaveChangesAsync(cancellationToken);

      var query = _context.Relations
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
