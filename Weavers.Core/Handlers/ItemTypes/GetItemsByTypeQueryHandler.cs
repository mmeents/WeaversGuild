using System;
using MediatR;
using Weavers.Core.Models;
using Weavers.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;


namespace Weavers.Core.Handlers.ItemTypes {


  public record GetItemsByItemTypeQuery(
    int ItemTypeId
) : IRequest<List<ItemLookup>>;
  

  public class GetItemsByTypeQueryHandler : IRequestHandler<GetItemsByItemTypeQuery, List<ItemLookup>> {
    private readonly FabricDbContext _context;
    public GetItemsByTypeQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<List<ItemLookup>> Handle(GetItemsByItemTypeQuery request, CancellationToken cancellationToken) {
      var rt = (WeItemType)request.ItemTypeId;
      var lookuptypes = WeItemTypeExtensions.GetLookupTypes();
      if (lookuptypes.Contains(rt)) {
        var items = await _context.ItemTypes
          .Where(i => i.ParentTypeId == request.ItemTypeId)
          .Select(i => new ItemLookup(i.Id, i.Description, i.Description))          
          .ToListAsync(cancellationToken);
        return items;
      } else {
        if (rt >= WeItemType.ProjectFolderModel) { 
          var items = await _context.Items
            .Where(i => i.ItemTypeId == request.ItemTypeId)
            .Select(i => new ItemLookup(i.Id, i.Name, i.Description))
            .ToListAsync(cancellationToken);    
          return items;
        }  
        
        else return new List<ItemLookup>();
      }
    }
  }
}
