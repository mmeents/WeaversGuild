using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record GetItemByIdQuery(int Id, bool IncludeRelations = false) : IMcpRequest, IRequest<ItemDto?>;

  public class GetItemByIdQueryHandler(
    FabricDbContext context
  ) : IRequestHandler<GetItemByIdQuery, ItemDto?> {
    private readonly FabricDbContext _context= context;    

    public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken) {      
      var result = await _context.GetItemDtoById(request.Id, cancellationToken);
      return result;
    }
  }
}
