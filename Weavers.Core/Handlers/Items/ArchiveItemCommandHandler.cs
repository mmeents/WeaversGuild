using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Items {

  public record ArchiveItemCommand(int Id, bool IsArchive) : IMcpRequest, IRequest<bool>;

  public class ArchiveItemCommandHandler : IRequestHandler<ArchiveItemCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly ISessionItemCacheService _sessionCache;
    private static readonly HashSet<int> ArchivableTypes = new() { 
      (int)WeItemType.TodoModel,
      (int)WeItemType.TodoAttemptModel,
      (int)WeItemType.RssItemModel, 
      (int)WeItemType.RssLinkedHtmlModel 
    };

    public ArchiveItemCommandHandler(FabricDbContext context, ISessionItemCacheService sessionCache) { 
      _context = context; 
      _sessionCache = sessionCache;
    }

    public async Task<bool> Handle(ArchiveItemCommand request, CancellationToken ct) {
      var item2 = _context.Items.FirstOrDefault(x => x.Id == request.Id);
      if (item2 == null) {
        throw new Exception($"Item with Id {request.Id} not found.");
      }
      if (item2.IsActive != !request.IsArchive) {        
        if (!ArchivableTypes.Contains(item2.ItemTypeId)) {
          throw new Exception($"Type {item2.ItemType.Name} is not archivable.");
        }        
        item2.IsActive = !request.IsArchive;
        await _context.SaveChangesAsync(ct);        
      }     
      _sessionCache.RemoveCacheItem(item2.Id);  // removes for cache.
      return true;
    }

  }

}
