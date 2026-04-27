using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Models;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;

namespace Weavers.Core.Handlers.DepItems {

  public record GetLibDiModelCommand(int ClassItemId) : IRequest<ItemDto?>;
  public class GetLibDiModelCommandHandler(FabricDbContext context) : IRequestHandler<GetLibDiModelCommand, ItemDto?> {
    private readonly FabricDbContext _context = context;    
    public async Task<ItemDto?> Handle(GetLibDiModelCommand request, CancellationToken cancellationToken) {     
      var item = await _context.GetItemDtoById(request.ClassItemId);
      var visited = new HashSet<int>();
      while (item != null) {
        if (!visited.Add(item.Id)) break;
        if (item.ItemTypeId == (int)WeItemType.LibraryModel) {
          return item;
        }
        var parentItemId = item.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains)?.ItemId;
        if (parentItemId == null) {
          break;
        }
        item = await _context.GetItemDtoById(parentItemId.Value);
      }
      return null;
    }
  }
}
