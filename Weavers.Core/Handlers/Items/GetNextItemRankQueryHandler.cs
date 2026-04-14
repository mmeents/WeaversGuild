using MediatR;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.Items {
  public record GetNextItemRankQuery(int? ItemId) : IRequest<int>;
  public class GetNextItemRankQueryHandler : IRequestHandler<GetNextItemRankQuery, int> {
    private readonly FabricDbContext _context;
    public GetNextItemRankQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<int> Handle(GetNextItemRankQuery request, CancellationToken cancellationToken) {
      if (request.ItemId == null) {
        var maxRank = await _context.Items.Where(i => i.ItemTypeId == (int)WeItemType.ProjectFolderModel).CountAsync(cancellationToken);
        return (maxRank + 1);        
      } else {
        var rank = await _context.GetItemsNextRankId(request.ItemId.Value);
        return rank;
      }     
    }
  }
}
