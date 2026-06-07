using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.DepItems {

  public record AddRemoveDbContextToLibDiCommand( int DiItemId, bool HasDbContext ) : IRequest<bool>;
  public class AddRemoveDbContextToLibDiCommandHandler : IRequestHandler<AddRemoveDbContextToLibDiCommand, bool> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    private readonly ISessionItemCacheService _sessionCache;
    public AddRemoveDbContextToLibDiCommandHandler(FabricDbContext context, IMediator mediator, ISessionItemCacheService sessionCache) {
      _context = context;
      _mediator = mediator;
      _sessionCache = sessionCache;
    }

    public async Task<bool> Handle(AddRemoveDbContextToLibDiCommand request, CancellationToken cancellationToken) {

      var diItem = await _sessionCache.GetItemAsync(request.DiItemId, cancellationToken);
      if ( diItem == null || (diItem.ItemTypeId != (int)WeItemType.DependencyInjectionModel) ) return false;

      var libParentRel = diItem.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains);
      if ( libParentRel == null ) return false;      

      var DbContextRel = diItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.DbContextModel);
      if ( DbContextRel == null && request.HasDbContext) {

        var libItem = await _sessionCache.GetItemAsync(libParentRel.ItemId, cancellationToken);
        if ( libItem == null ) return false;
        var libItemname = libItem?.Name.UrlSafe() ?? "NeedsLib";
        var dbContextName = $"{libItemname}DbContext";

        var command = new CreateRelatedItemCommand(diItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.DbContextModel, dbContextName, "", "");
        var dbContextItem = await _mediator.Send(command);
        if (dbContextItem == null) return false;

        
        string newPath = libItem?.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? "missingPath";
        await UpdateFolderPathIfNeededAsync(dbContextItem, newPath, cancellationToken);
        
        string newNamespace = libItem?.ResolveItemsNamespace(dbContextItem.Name) ?? "NoNamespace";
        await UpdateNamespacePathIfNeededAsync(dbContextItem, newNamespace);  

        await _mediator.SyncLibraryPackageDefaults(libItem, PkgType.DbContext, request.HasDbContext, cancellationToken);
        await _context.MarkItemUpdated(libItem!.Id,cancellationToken);

      } else if ( !request.HasDbContext && DbContextRel != null && DbContextRel.RelatedItemId != null) {

        await _mediator.Send(new DeleteItemCommand(DbContextRel.RelatedItemId.Value), cancellationToken);
        await _context.MarkItemUpdated(diItem!.Id, cancellationToken);
      }

      return true;
    }


    private async Task UpdateFolderPathIfNeededAsync(ItemDto item, string basePath, CancellationToken cancellationToken) {
      string propKey = item.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") return;

      var folderProp = item.Properties.FirstOrDefault(p => p.Name == propKey);
      if (folderProp == null) return;

      var fullPath = "";
      var fileName = "";  // namespaces are folders within a file and get add to the path the file is in.
      var fileBasePath = basePath;
      if (propKey == Cx.ItFilePath) {
        if (item.ItemTypeId.IsFileNameType()){
          fileBasePath = Path.GetDirectoryName(fileBasePath) ?? basePath;
        }
        fileName = item.GetFileName();
        fullPath = Path.Combine(fileBasePath, fileName);
      } else if (item.ItemTypeId == (int)WeItemType.RelativeFolderModel) {
        fullPath = Path.Combine(basePath, item.Name.UrlSafe());
      } else {
        fullPath = basePath;
      }

      if (folderProp != null && string.Compare(folderProp.Value, fullPath, true) != 0) {

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
        var updated = await _mediator.Send(command, cancellationToken);
        await _context.MarkItemUpdated(item.Id, cancellationToken);

        if (updated != null) item.AddOrUpdateProperty(updated);
      }

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
        await _context.MarkItemUpdated(item.Id);
      }
    }







    
  }
}
