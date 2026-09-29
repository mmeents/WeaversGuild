using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Cache {

  public record RemoveItemsFromCacheCommand(int[] itemIds) : IRequest<bool>;

  public class RemoveItemsFromCacheCommandHandler : IRequestHandler<RemoveItemsFromCacheCommand, bool> {
    private readonly ISessionItemCacheService _sessionCache;
    public RemoveItemsFromCacheCommandHandler(ISessionItemCacheService sessionCache) {
      _sessionCache = sessionCache;
    }
    public async Task<bool> Handle(RemoveItemsFromCacheCommand request, CancellationToken cancellationToken) {
      foreach(var itemId in request.itemIds) {
        if (itemId > 0) {
          _sessionCache.RemoveCacheItem(itemId);
        }        
      }
      return true;
    }
  }
 

  public class RemoveTool {  
    public static async Task<bool> RemoveItemsFromCache(int[] itemIds) 
      => await DiBridgeService2.Send(new RemoveItemsFromCacheCommand(itemIds));   

  }

}
