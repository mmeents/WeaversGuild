using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.DepItems {

  public record AddRemoveDbContextToLibDiCommand( int DiItemId, bool HasDbContext ) : IRequest<bool>;
  public class AddRemoveDbContextToLibDiCommandHandler : IRequestHandler<AddRemoveDbContextToLibDiCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public AddRemoveDbContextToLibDiCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<bool> Handle(AddRemoveDbContextToLibDiCommand request, CancellationToken cancellationToken) {

      var diItem = await _context.GetItemDtoById(request.DiItemId, cancellationToken);
      if ( diItem == null || (diItem.ItemTypeId != (int)WeItemType.DependencyInjectionModel) ) return false;

      var libParentRel = diItem.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains);
      if ( libParentRel == null ) return false;      

      var DbContextRel = diItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.DbContextModel);
      if ( DbContextRel == null && request.HasDbContext) {

        var libItem = await _context.GetItemDtoById(libParentRel.ItemId, cancellationToken);
        var libItemname = libItem?.Name.UrlSafe() ?? "NeedsLib";
        var dbContextName = $"{libItemname}DbContext";

        var command = new CreateRelatedItemCommand(diItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.DbContextModel, dbContextName, "", "");
        var dbContextItem = await _mediator.Send(command);
        if (dbContextItem == null) return false;

        string newPath = diItem.ResolveParentFolderPath("NoPath");
        await UpdateFolderPathIfNeededAsync(dbContextItem, newPath);
        
        string newNamespace = diItem.ResolveParentNamespace(dbContextItem.Name);
        await UpdateNamespacePathIfNeededAsync(dbContextItem, newNamespace);  
        

      } else if ( !request.HasDbContext && DbContextRel != null && DbContextRel.RelatedItemId != null) {

        await _mediator.Send(new DeleteItemCommand(DbContextRel.RelatedItemId.Value), cancellationToken);       

      }

      return true;
    }


    private async Task UpdateFolderPathIfNeededAsync(ItemDto item, string basePath) {
      string propKey = item.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") return;

      var folderProp = item.Properties.FirstOrDefault(p => p.Name == propKey);
      if (folderProp == null) return;

      var fullPath = "";
      var fileName = "";  // namespaces are folders within a file and get add to the path the file is in.
      var fileBasePath = basePath;
      if (propKey == Cx.ItFilePath) {
        if (fileBasePath.Contains(".csproj")) {
          fileBasePath = Path.GetDirectoryName(fileBasePath) ?? basePath;
        }
        fileName = item.GetFileName();
        fullPath = Path.Combine(fileBasePath, fileName);
      } else if (item.ItemTypeId == (int)WeItemType.RelativeFolderModel) {
        fullPath = Path.Combine(basePath, item.Name.UrlSafe());
      } else {
        fullPath = basePath;
      }

      folderProp.Value = fullPath;
      var command = new AddUpdateItemPropertyCommand(
        folderProp.Id,
        folderProp.ItemId,
        folderProp.Name,
        folderProp.Value,
        folderProp.ValueDataTypeId,
        folderProp.EditorTypeId,
        folderProp.ReferenceItemTypeId
       );
      var updated = await _mediator.Send(command);
      
      if (updated != null) item.AddOrUpdateProperty(updated);
      

    }

    private async Task UpdateNamespacePathIfNeededAsync(ItemDto item, string basePath) {
      var namespaceProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace || p.Name == Cx.ItNamespaceRoot);
      if (namespaceProp == null) return;
      var newNamespace = "";
      var folderPropKey = item.ItemTypeId.GetFolderPropertyName();
      if (folderPropKey == Cx.ItFilePath && (item.ItemTypeId != (int)WeItemType.NamespaceModel)) {
        newNamespace = basePath.NameSafe(); // files use the parents namespace.
      } else {  // vs a folder, we include items name.
        newNamespace = basePath.NameSafe() + "." + item.Name.NameSafe();
      }

      if (namespaceProp.Value != newNamespace) {
        namespaceProp.Value = newNamespace;
        var command = new AddUpdateItemPropertyCommand(
        namespaceProp.Id,
        namespaceProp.ItemId,
        namespaceProp.Name,
        namespaceProp.Value,
        namespaceProp.ValueDataTypeId,
        namespaceProp.EditorTypeId,
        namespaceProp.ReferenceItemTypeId
        );
        var updated = await _mediator.Send(command);        
      }
    }





  }
}
