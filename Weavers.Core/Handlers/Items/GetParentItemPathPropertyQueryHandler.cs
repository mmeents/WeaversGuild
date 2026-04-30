using MediatR;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;

namespace Weavers.Core.Handlers.Items {

  public record GetParentItemPathPropertyQuery(int ItemId) : IRequest<string?>;
  public class GetParentItemPathPropertyQueryHandler : IRequestHandler<GetParentItemPathPropertyQuery, string?> {
    private readonly FabricDbContext _context;
    public GetParentItemPathPropertyQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<string?> Handle(GetParentItemPathPropertyQuery request, CancellationToken cancellationToken) {     
      if (request == null) { return null;}
      var item = await _context.GetItemDtoById(request.ItemId);
      if (item == null) { return null; }
      var parentRelation = item.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (parentRelation == null) { return null; }
      var parentId = parentRelation.ItemId;
      var parentItem = await _context.GetItemDtoById(parentId);
      if (parentItem == null) { return null; }
      var propKey = parentItem.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") { return null; }

      var pathProp = parentItem.Properties.FirstOrDefault(p => p.Name == propKey);
      if (pathProp == null) { return null; }
      var path = pathProp.Value;
      if (parentItem.ItemTypeId == (int)WeItemType.LibraryModel || parentItem.ItemTypeId == (int)WeItemType.EntityConfigurationModel) {     
        path = Path.GetDirectoryName(path) ?? "";
      }
      return path;
    }
  }
}
