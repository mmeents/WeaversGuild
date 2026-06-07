using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Items {
  public record GetItemByIdQuery(int Id) : IMcpRequest, IRequest<ItemDto?>;

  public class GetItemByIdQueryHandler(    
    ISessionItemCacheService sessionCache
  ) : IRequestHandler<GetItemByIdQuery, ItemDto?> {    
    private readonly ISessionItemCacheService _sessionCache = sessionCache;

    public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken) {      
      var result = await _sessionCache.GetItemAsync(request.Id, cancellationToken);      
      return result;
    }
  }
}
