using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Entities;
using Weavers.Core.Models;  

namespace Weavers.Core.Handlers.ItemTypes {
  public record GetAllItemTypesQuery() : IRequest<List<ItemTypeDto>>;
  internal class GetAllItemTypesQueryHandler : IRequestHandler<GetAllItemTypesQuery, List<ItemTypeDto>> {
    private readonly FabricDbContext _context;
    public GetAllItemTypesQueryHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<List<ItemTypeDto>> Handle(GetAllItemTypesQuery request, CancellationToken cancellationToken) {
      var itemTypes = await _context.ItemTypes.ToListAsync(cancellationToken);
      return itemTypes.Select(it => new ItemTypeDto {
        Id = it.Id,
        Name = it.Name,
        Description = it.Description
      }).ToList();
    }
  }
}