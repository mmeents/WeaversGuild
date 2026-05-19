using System;
using MediatR;
using Weavers.Core.Models;
using Weavers.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;
using System.Security.Permissions;


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
      } else if (request.ItemTypeId.IsCSharpLookupType()) {
        switch (rt) {
          case WeItemType.CSharpClassType:
            var items = await _context.Items
              .Where(i => i.ItemTypeId == (int)WeItemType.ClassModel || i.ItemTypeId == (int)WeItemType.EntityClassModel)
              .Select(i => new ItemLookup(i.Id, i.Name, i.Name))
              .ToListAsync(cancellationToken);
            return items;            
          case WeItemType.CSharpRecordType:
            var items2 = await _context.Items
              .Where(i => i.ItemTypeId == (int)WeItemType.RecordModel)
              .Select(i => new ItemLookup(i.Id, i.Name, i.Name))
              .ToListAsync(cancellationToken);
            return items2;            
          case WeItemType.CSharpStructType:
            var items3 = await _context.Items
              .Where(i => i.ItemTypeId == (int)WeItemType.StructModel)
              .Select(i => new ItemLookup(i.Id, i.Name, i.Name))
              .ToListAsync(cancellationToken);
            return items3;            
        }
        return new List<ItemLookup>();
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
