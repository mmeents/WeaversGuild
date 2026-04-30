using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Enums;

namespace Weavers.Core.Handlers.Items {
  public record DeleteItemCommand(int Id) : IRequest<bool>;

  public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public DeleteItemCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.Items
        .Include(i => i.Relations)
        .Include(i => i.IncomingRelations)
        .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken)
        ?? throw new KeyNotFoundException("Item not found");

      if (item.ItemTypeId == (int)WeItemType.EntityClassModel) {
        var libraryItem = await _mediator.Send(new GetLibDiModelCommand(item.Id), cancellationToken);
        if (libraryItem != null) {
          await _mediator.Send(new AddRemoveEntityToDbContextCommand(item.Id, false), cancellationToken);
        }
      }      

      // Remove inbound relations (other items pointing here)
      _context.Relations.RemoveRange(item.IncomingRelations);

      // Walk outbound relations, cascade delete orphaned targets
      foreach (var relation in item.Relations.ToList()) {
        _context.Relations.Remove(relation);
        if (relation.RelatedItemId.HasValue)
          await CascadeIfOrphan(relation.RelatedItemId.Value, cancellationToken);
      }

      _context.Items.Remove(item);
      await _context.SaveChangesAsync(cancellationToken);
      return true;
    }

    private async Task CascadeIfOrphan(int itemId, CancellationToken cancellationToken) {
      var related = await _context.Items
          .Include(i => i.Relations)
          .Include(i => i.IncomingRelations)
          .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);
      if (related == null) return;

      // Only cascade if no remaining connections after this delete
      var remainingIncoming = related.IncomingRelations.Count(r => r.ItemId != itemId);

      if (remainingIncoming == 0 && !related.Relations.Any()) {
        _context.Items.Remove(related);
      }
    }

  }

}
