using KB.Core.Entities;
using KB.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.Items {
  public record CreateItemCommand(
   string Name,
   int ItemTypeId,
   string Description,
   string Data
 ) : IRequest<ItemDto?>;


  public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    public CreateItemCommandHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<ItemDto?> Handle(CreateItemCommand request, CancellationToken cancellationToken) {

      var itemType = await _context.ItemTypes.FindAsync(new object[] { request.ItemTypeId }, cancellationToken);

      if (itemType == null) {
        throw new Exception($"ItemType with id {request.ItemTypeId} not found");
      }

      var item = new Item {
        Name = request.Name,
        ItemTypeId = itemType.Id,
        Description = request.Description,
        Data = request.Data,
        IsActive = true
      };

      _context.Items.Add(item);
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
