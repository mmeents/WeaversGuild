using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Service {

  public interface ISessionItemCacheService {
    public Task<ItemDto?> GetItemAsync(int itemId, CancellationToken cancellationToken, bool? skipCache = null);
    public bool RemoveCacheItem(int itemId);
    public bool ClearCache();
  }


  public class SessionItemCacheService : ISessionItemCacheService {
    private readonly IServiceScopeFactory _serviceScopeFactory;    
    private readonly ConcurrentDictionary<int, ItemDto> _itemCache;

    public SessionItemCacheService(IServiceScopeFactory serviceScopeFactory) {
      _serviceScopeFactory = serviceScopeFactory;     
      _itemCache = new ConcurrentDictionary<int, ItemDto>();
    }   

    public async Task<ItemDto?> GetItemAsync(int itemId, CancellationToken cancellationToken, bool? skipCache = null) {
      if (skipCache == true) {
        _itemCache.TryRemove(itemId, out _);
      }
      if (skipCache != true && _itemCache.TryGetValue(itemId, out var cachedItem)) {
        return cachedItem;
      }
      using (var scope = _serviceScopeFactory.CreateScope()) {
        var _fabricDbContext = scope.ServiceProvider.GetRequiredService<FabricDbContext>();        
        var item = await _fabricDbContext.GetItemDtoById(itemId, cancellationToken);
        if (item != null) {        
          _itemCache.TryAdd(itemId, item);
          return item;
        }
        return null;
      }
    }

    public bool RemoveCacheItem(int itemId) {
      return _itemCache.TryRemove(itemId, out _);
    }

    public bool ClearCache() {
      _itemCache.Clear();
      return true;
    }

  }
}
