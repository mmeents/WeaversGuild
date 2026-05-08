using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;


namespace Weavers.Core.Handlers.Builds {


  public record MarkItemWrittenCommand(int ItemId) : IRequest<bool>;
  public class MarkItemWrittenCommandHandler : IRequestHandler<MarkItemWrittenCommand, bool> {
    private readonly FabricDbContext _context;
    public MarkItemWrittenCommandHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<bool> Handle(MarkItemWrittenCommand request, CancellationToken cancellationToken) {
      var item = await _context.Items.FindAsync(new object[] { request.ItemId }, cancellationToken);
      if (item == null) return false;
      item.WrittenAt = DateTime.UtcNow;
      _context.Items.Update(item);
      await _context.SaveChangesAsync(cancellationToken);
      return true;
    }
  }
}