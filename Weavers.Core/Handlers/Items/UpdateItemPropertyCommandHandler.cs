using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Items {
  public record UpdateItemPropertyCommand(int itemPropertyId, string PropertyValue) : IMcpRequest, IRequest<ItemDto?>;
  public class UpdateItemPropertyCommandHandler : IRequestHandler<UpdateItemPropertyCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public UpdateItemPropertyCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<ItemDto?> Handle(UpdateItemPropertyCommand request, CancellationToken cancellationToken) {

      var itemProperty = _context.ItemProperties.FirstOrDefault(x => x.Id == request.itemPropertyId);
      if (itemProperty == null) { return null; }
      var itemPropertyName = itemProperty.Name;   // property that is changing.
      var oldValue = itemProperty.Value ?? "";           // old value before change.
      var newValue = request.PropertyValue;

      var item = await _context.GetItemDtoById( itemProperty.ItemId, cancellationToken);
      if (item == null) { return null; }
      var itemType = (WeItemType)item.ItemTypeId;
      var folderPropName = item.ItemTypeId.GetFolderPropertyName();
      var namespacePropName = item.ItemTypeId.GetNamespacePropertyName();

      var parentItemId = item.IncomingRelations.FirstOrDefault(r => r.ItemId != item.Id)?.ItemId;
      bool hasParent = parentItemId != null;
      ItemDto? parentItem = null;
      if (itemType != WeItemType.ProjectFolderModel && parentItemId != null) { 
        parentItem = await _context.GetItemDtoById(parentItemId.Value, cancellationToken);
        hasParent = parentItem != null;
      }

      bool propUpdated = false;
      switch (itemType) {
        case WeItemType.ProjectFolderModel:
          if (itemPropertyName == Cx.ItRootFolder) {
            string newPath = newValue.UrlSafe();
            if (newPath.ValidatePath()) {
              await _mediator.Send(new UpdateItemPropertyPathRecursiveCommand(item.Id, newPath), cancellationToken);
              propUpdated = true;
            }
          }
          break;
        case WeItemType.RelativeFolderModel:
          if (itemPropertyName == Cx.ItRelativeFolder && hasParent) {
            var newPath = parentItem == null ? "MissingPath" : parentItem.ResolveParentFolderPath("MissingPath");
            newPath = Path.Combine(newPath, newValue.UrlSafe());
            if (newPath.ValidatePath()) {
              await _mediator.Send(new UpdateItemPropertyPathRecursiveCommand(item.Id, newPath), cancellationToken);
              propUpdated = true;
            }
          }
          break;
        case WeItemType.LibraryModel:
          if (itemPropertyName == Cx.ItFilePath) {
            var newPath = parentItem == null ? "MissingPath" : parentItem.ResolveParentFolderPath("MissingPath");
            await UpdateFolderPathIfNeededAsync(item, newPath, cancellationToken);
            propUpdated = true;
          } else if (itemPropertyName == Cx.ItNamespaceRoot) {            
            await UpdateNamespacePathIfNeededAsync(item, newValue, newValue, cancellationToken);
            propUpdated = true;
          } else if (itemPropertyName == Cx.ItIsTestLibrary) {
            var newBoolValue = newValue.AsBoolean();            
            var oldBoolValue = oldValue.AsBoolean();
            if (newBoolValue == oldBoolValue) { break; }
            if (newBoolValue) {
              await _mediator.SyncLibraryPackageDefaults(item, PkgType.LibraryBase, false, cancellationToken);
              await _mediator.SyncLibraryPackageDefaults(item, PkgType.TestLibrary, true, cancellationToken);
            } else {
              await _mediator.SyncLibraryPackageDefaults(item, PkgType.LibraryBase, true, cancellationToken);
              await _mediator.SyncLibraryPackageDefaults(item, PkgType.TestLibrary, false, cancellationToken);
            }
          }
          break;
        case WeItemType.DependencyInjectionModel:
          if (itemPropertyName == Cx.ItFilePath) {
            var newPath = parentItem == null ? "MissingPath" : parentItem.ResolveParentFolderPath("MissingPath");
            await UpdateFolderPathIfNeededAsync(item, newPath, cancellationToken);
            propUpdated = true;
          } else if (itemPropertyName == Cx.ItNamespace) {
            string parentNs = parentItem.ResolveItemsNamespace(item.Name);
            await UpdateNamespacePathIfNeededAsync(item, parentNs, newValue, cancellationToken);
            propUpdated = true;
          } else if (itemPropertyName == Cx.ItHasDbContext) {
            var newBoolValue = newValue.AsBoolean();
            await _mediator.Send(new AddRemoveDbContextToLibDiCommand(item.Id, newBoolValue), cancellationToken);            
          }
          break;
        case WeItemType.NamespaceModel:
          if (itemPropertyName == Cx.ItFilePath) {
            var newPath = parentItem == null ? "MissingPath" : parentItem.ResolveParentFolderPath("MissingPath");
            await UpdateFolderPathIfNeededAsync(item, newPath, cancellationToken);
            propUpdated = true;
          } else if (itemPropertyName == Cx.ItNamespace) {
            string parentNS = parentItem.ResolveItemsNamespace(item.Name);
            await UpdateNamespacePathIfNeededAsync(item, parentNS, newValue, cancellationToken);
            propUpdated = true;
          }
          break;
        case WeItemType.ClassModel:
          if (itemPropertyName == Cx.ItGenerateInterface || itemPropertyName == Cx.ItRegisterDi) {
            var genIntf = item.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface)?.Value.AsBoolean();
            var regDi = item.Properties.FirstOrDefault(p => p.Name == Cx.ItRegisterDi)?.Value.AsBoolean();
            if (genIntf.HasValue && regDi.HasValue) {
              await _mediator.Send(new AddRemoveClassToLibDiCommand(item.Id, regDi.Value, genIntf.Value), cancellationToken);
            }
          }
          break;
        case WeItemType.EntityPropertyModel:
        case WeItemType.EntityNavigationModel:
          ItemDto? preParentNode = item;
          while (parentItem != null && parentItem.ItemTypeId > (int)WeItemType.EntityClassModel) {
            preParentNode = parentItem;
            var newParentId = preParentNode.IncomingRelations.FirstOrDefault(r => r.ItemId != preParentNode.Id)?.ItemId;
            if (!newParentId.HasValue) { parentItem = null; continue; }
            parentItem = await _context.GetItemDtoById(newParentId.Value, cancellationToken);
          }

          // ugly, starts at nav update walks up to prop then class. process property needs to land on a property.            
          if (parentItem != null && parentItem.ItemTypeId == (int)WeItemType.EntityClassModel
            && preParentNode != null && preParentNode.ItemTypeId == (int)WeItemType.EntityPropertyModel) {
            await _mediator.Send(new ProcessPropertyUpdateCommand(parentItem, preParentNode), cancellationToken);
          }
          break;

      }   

      if (!propUpdated) {
        // save new property value
        itemProperty.Value = request.PropertyValue;  //  new value.
        _context.ItemProperties.Update(itemProperty);
        await _context.SaveChangesAsync(cancellationToken);
      }

      item = await _context.GetItemDtoById(item.Id, cancellationToken);  // get updated item to return.
      return item;
    }


    private async Task UpdateFolderPathIfNeededAsync(ItemDto item, string basePath, CancellationToken cancellationToken) {
      string propKey = item.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") return;

      var folderProp = item.Properties.FirstOrDefault(p => p.Name == propKey);
      if (folderProp == null) return;

      var fullPath = "";
      var fileName = "";  // namespaces are folders within a file and get add to the path the file is in.
      var fileBasePath = basePath;
      if (item.ItemTypeId == (int)WeItemType.NamespaceModel || item.ItemTypeId == (int)WeItemType.RelativeFolderModel) {
        fullPath = Path.Combine(basePath, item.Name.UrlSafe());
      } else if (propKey == Cx.ItFilePath) {
        if (item.ItemTypeId.IsFileNameType()) {
          fileBasePath = Path.GetDirectoryName(fileBasePath) ?? basePath;
        }
        fileName = item.GetFileName();
        fullPath = Path.Combine(fileBasePath, fileName);
      } else {
        fullPath = basePath;
      }

      if (folderProp.Value != fullPath) {
        folderProp.Value = fullPath;
        await _mediator.Send(new UpdateItemPropertyPathRecursiveCommand(item.Id, fullPath), cancellationToken);
      }
    }

    private async Task UpdateNamespacePathIfNeededAsync(ItemDto item, string parentNS, string newValue, CancellationToken cancellationToken) {
      var namespaceProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace || p.Name == Cx.ItNamespaceRoot);
      if (namespaceProp == null) return;
      var newNamespace = "";
      var folderPropKey = item.ItemTypeId.GetFolderPropertyName();
      if (folderPropKey == Cx.ItFilePath && (item.ItemTypeId != (int)WeItemType.NamespaceModel)) {
        newNamespace = parentNS.NameSafe(); // files use the parents namespace.
      } else {  // vs a folder, we include items name.
        newNamespace = parentNS.NameSafe() + "." + newValue.NameSafe();
      }
      if (namespaceProp.Value != newNamespace) {
        var oldNamespace = namespaceProp?.Value ?? "";
        namespaceProp!.Value = newNamespace;
        var updated = await _mediator.Send(namespaceProp!.ToSaveCmd(), cancellationToken);
        await _mediator.Send(new UpdateItemPropertyNamespaceRecursiveCommand(item.Id, oldNamespace, newNamespace), cancellationToken);
      }
    }


  }
}
