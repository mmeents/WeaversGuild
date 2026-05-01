using MediatR;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.DepItems {
  public record GetEntityClassFromChildrenCommand(int ItemId) : IRequest<ItemDto?>;
  public class GetEntityClassFromChildrenCommandHandler : IRequestHandler<GetEntityClassFromChildrenCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    public GetEntityClassFromChildrenCommandHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<ItemDto?> Handle(GetEntityClassFromChildrenCommand request, CancellationToken cancellationToken) {

      var item = await _context.GetItemDtoById(request.ItemId);
      var visited = new HashSet<int>();
      while (item != null) {
        if (!visited.Add(item.Id)) break;
        if (item.ItemTypeId == (int)WeItemType.EntityClassModel) {
          return item;
        }
        var parentItemId = item.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains)?.ItemId;
        if (parentItemId == null) {
          break;
        }
        item = await _context.GetItemDtoById(parentItemId.Value);
        if (item == null) {
          break;
        } else if (item.ItemTypeId < (int)WeItemType.EntityClassModel) { 
          break;
        }
      }
      return null;
    }
  }
}
