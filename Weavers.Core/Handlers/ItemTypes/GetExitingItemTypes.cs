using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.ItemTypes {
  public record GetExitingItemTypesQuery : IRequest<List<ItemLookup>>;
  public class GetExitingItemTypes : IRequestHandler<GetExitingItemTypesQuery, List<ItemLookup>> {
    private readonly FabricDbContext _context;
    public GetExitingItemTypes(FabricDbContext context) {
      _context = context;
    }
    public async Task<List<ItemLookup>> Handle(GetExitingItemTypesQuery request, CancellationToken cancellationToken) {

      /*
       SELECT distinct [ItemTypeId], it.Name FROM [dbo].[Items] i
         inner join dbo.ItemTypes it on it.id = i.ItemTypeId

       */
      var activeTypes = await _context.Items
        .Join(_context.ItemTypes, i => i.ItemTypeId, it => it.Id,
          (i, it) => new ItemLookup(i.ItemTypeId, it.Name, ""))
        .Distinct()
        .ToListAsync(cancellationToken);

      return activeTypes;
    }
  }
}
