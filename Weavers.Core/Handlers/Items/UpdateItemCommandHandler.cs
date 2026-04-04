using KB.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Handlers.Items {
  public record UpdateItemCommand(
     int Id,
     int ItemTypeId,
     string Name,
     string Description,
     string Data,
     bool IsActive
   ) : IRequest<ItemDto?>;


  public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context;

    public UpdateItemCommandHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<ItemDto?> Handle(UpdateItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.Items.FindAsync(request.Id);

      if (item == null) {
        throw new KeyNotFoundException("Item not found");
      }

      item.Name = request.Name;
      item.Description = request.Description;
      item.Data = request.Data;
      item.ItemTypeId = request.ItemTypeId;
      item.IsActive = request.IsActive;

      await _context.SaveChangesAsync(cancellationToken);

      var query = _context.Items
        .AsNoTracking()
        .Where(i => i.Id == item.Id && i.IsActive);

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

      item = await query.FirstOrDefaultAsync(cancellationToken);

      return item != null ? item.ToDto(true) : null;

    }
  }
}
