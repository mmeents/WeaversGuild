using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Items {
  public record UpdateItemCommand(
     int Id,
     int ItemTypeId,
     string Name,
     string Description,
     string Data,
     bool IsActive,
     DateTime? WrittenAt
   ) : IMcpRequest, IRequest<ItemDto?>;

  public static class UpdateItemCommandExtensions {
    public static UpdateItemCommand ToUpdateCmd(this ItemDto item) {
      return new UpdateItemCommand(
        item.Id,
        item.ItemTypeId,
        item.Name,
        item.Description,
        item.Data,
        item.IsActive,
        item.WrittenAt
      );
    }
  }


  public class UpdateItemCommandHandler(
    FabricDbContext context, 
    IMediator mediator,
    ILogger<UpdateItemCommandHandler> logger,
    IAppSettingService appSettingService
  ) : IRequestHandler<UpdateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context = context;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<UpdateItemCommandHandler> _logger = logger;
    private readonly IAppSettingService _appSettingService = appSettingService;
    private readonly HashSet<WeItemType> _parentFolderTypes = WeItemTypeExtensions.GetParentFileFolderDependTypes();
    private readonly HashSet<WeItemType> _parentNamespaceTypes = WeItemTypeExtensions.GetParentNamespaceDependTypes();
    public async Task<ItemDto?> Handle(UpdateItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.Items.FindAsync(request.Id);
      var nameWas = item?.Name;

      if (item == null) {
        var error = new KeyNotFoundException($"Item with id {request.Id} not found");
        _logger.LogError(error, "Failed to update item with id {ItemId}", request.Id);
        throw error;
      }

      item.Name = request.Name;
      item.Description = request.Description;
      item.Data = request.Data;
      item.ItemTypeId = request.ItemTypeId;
      item.Established = DateTime.UtcNow;  //updates to current time on update.
      item.WrittenAt = request.WrittenAt;  //carries last time written to disk.
      item.IsActive = request.IsActive;

      _context.Items.Update(item);
      await _context.SaveChangesAsync(cancellationToken);

      var itemDto = await _context.GetItemDtoById(item.Id, cancellationToken);

      if (itemDto == null) { throw new Exception("Reload after save failed."); }

      ItemDto? parentItem = null;
      if (itemDto.ItemTypeId == (int)WeItemType.OrganizationModel) {
        string defaultPath = _appSettingService.DefaultProjectsPath;
        string newPath = Path.Combine(defaultPath, itemDto.Name);
        await UpdateFolderPathIfNeededAsync(itemDto, newPath);
      } else { 

        var parentItemId = itemDto.IncomingRelations.FirstOrDefault(r => r.ItemId != itemDto.Id)?.ItemId;
        if (parentItemId != null) {

          parentItem = await _context.GetItemDtoById(parentItemId.Value, cancellationToken);
          if (parentItem != null) {

            if (_parentFolderTypes.Contains((WeItemType)itemDto.ItemTypeId)) {
              string newPath = parentItem.ResolveParentFolderPath(_appSettingService.DefaultProjectsPath);
              await UpdateFolderPathIfNeededAsync(itemDto, newPath);
            }
          
            if (_parentNamespaceTypes.Contains((WeItemType)itemDto.ItemTypeId)) {
              string newNamespace = parentItem.ResolveItemsNamespace(itemDto.Name);
              await UpdateNamespacePathIfNeededAsync(itemDto, newNamespace);
            }                

          }  // parent null
        }  // parent id null
      }  // should have a parent.


      if (itemDto.ItemTypeId == (int)WeItemType.LibraryModel) {
        
        var isTestLibrary = itemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItIsTestLibrary)?.Value.AsBoolean() ?? false;
        if (isTestLibrary) {
          await _mediator.SyncLibraryPackageDefaults(itemDto, PkgType.LibraryBase, false, cancellationToken);
          await _mediator.SyncLibraryPackageDefaults(itemDto, PkgType.TestLibrary, true, cancellationToken);
        } else {
          await _mediator.SyncLibraryPackageDefaults(itemDto, PkgType.LibraryBase, true, cancellationToken);
          await _mediator.SyncLibraryPackageDefaults(itemDto, PkgType.TestLibrary, false, cancellationToken);
        }        
      }
            
      if (itemDto.ItemTypeId == (int)WeItemType.DependencyInjectionModel) {
        var libItem = await _mediator.Send(new GetLibraryRelativeCommand(itemDto.Id), cancellationToken);
        if (libItem != null) {
          var hasDbContext = itemDto.Properties.Any(p => p.Name == Cx.ItHasDbContext && p.Value.AsBoolean());
          await _mediator.SyncLibraryPackageDefaults(libItem, PkgType.DbContext, hasDbContext, cancellationToken);
          var hasMediatR = itemDto.Properties.Any(p => p.Name == Cx.ItHasMediator && p.Value.AsBoolean());
          await _mediator.SyncLibraryPackageDefaults(libItem, PkgType.Mediatr, hasMediatR, cancellationToken);
        }
      }

      if (itemDto.ItemTypeId == (int)WeItemType.EntityClassModel && nameWas != itemDto.Name) {
        // EntityClassModel has dependent EntityConfigurationModel and DbContextEntityImportModel names that also need to change.
        var libraryItem = await _mediator.Send(new GetLibraryRelativeCommand(itemDto.Id), cancellationToken);
        if (libraryItem != null) { 
          var dbContextItem = await _context.ResolveDbContextFromLib(libraryItem, cancellationToken);
          if (dbContextItem != null) {
            var dbContextEntityItem = await _context.FindRelatedDbContextEntity(dbContextItem, itemDto, cancellationToken);
            if (dbContextEntityItem != null) {
             var entity = await _context.Items.FindAsync(dbContextEntityItem.Id);
              if (entity != null) {
                entity.Name = item.Name;
                entity.Established = DateTime.UtcNow;
                _context.Items.Update(entity);
                await _context.SaveChangesAsync(cancellationToken);
              }
            }
          }
        }

        var entityConfigItemRef = itemDto.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityConfigurationModel);
        if (entityConfigItemRef != null) {
          await DoRename(entityConfigItemRef.RelatedItemId, request.Name.Trim()+"Config", cancellationToken);
        }

      } else if (itemDto.ItemTypeId == (int)WeItemType.EntityPropertyModel) {  /*----------------------  property model ------*/
        var propItem = itemDto;
        var rels = propItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationModel);
        if (rels != null && rels.RelatedItemId != null) { 
          await DoRename(rels.RelatedItemId, request.Name, cancellationToken); // child nav
        }             

      } 
        else if (itemDto.ItemTypeId == (int)WeItemType.EntityNavigationModel && nameWas != itemDto.Name) {
        var entityPropId = itemDto.IncomingRelations.FirstOrDefault(r => r.ItemId != itemDto.Id)?.ItemId;
        if (entityPropId == null) { return null; }
        var entityClass = await _mediator.Send(new GetEntityClassFromChildrenCommand(entityPropId.Value), cancellationToken);
        var remoteEntityId = itemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType)?.Value.AsInt32();
        if (remoteEntityId.HasValue && remoteEntityId.Value > 0 && entityClass != null) {
          var cmd3 = new AddRemoveEntityInboundNavCommand(remoteEntityId.Value, entityPropId.Value, entityClass.Id, false);
          await _mediator.Send(cmd3, cancellationToken);
        }
      }

      if (itemDto.ItemTypeId == (int)WeItemType.EntityPropertyModel || itemDto.ItemTypeId == (int)WeItemType.EntityNavigationModel) {
        ItemDto? preParentNode = itemDto;
        while (parentItem != null && parentItem.ItemTypeId > (int)WeItemType.EntityClassModel) {
          preParentNode = parentItem;
          var newParentId = preParentNode.IncomingRelations.FirstOrDefault(r => r.ItemId != preParentNode.Id)?.ItemId;
          if (!newParentId.HasValue) { parentItem = null; continue; }
          parentItem = await _context.GetItemDtoById(newParentId.Value, cancellationToken);
        }

        // ugly, starts at nav update walks up to prop then class. process property needs to land on a property.            
        if (parentItem != null && parentItem.ItemTypeId == (int)WeItemType.EntityClassModel 
          && preParentNode != null && preParentNode.ItemTypeId == (int)WeItemType.EntityPropertyModel) {
          await _mediator.Send(new ProcessPropertyUpdateCommand(parentItem, preParentNode));
        }
      }

      return itemDto;

    }

    private async Task<bool> DoRename(int? itemId, string newName, CancellationToken cancellationToken) {
      int theId = 0;
      if (itemId != null) { 
        theId = itemId.Value;
      }
      if (theId != 0) { 
        var entityConfig = await _context.Items.FindAsync(theId);
        if (entityConfig != null) {
          entityConfig.Name = newName;
          entityConfig.Established = DateTime.UtcNow;
          _context.Items.Update(entityConfig);
          await _context.SaveChangesAsync(cancellationToken);
        }
      } else return false;
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
        var updated = await _mediator.UpdateItemProp(item, folderProp);
        await _mediator.Send(new UpdateItemPropertyPathRecursiveCommand(item.Id, fullPath));
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
        await _mediator.Send(new UpdateItemPropertyNamespaceRecursiveCommand(item.Id, basePath, newNamespace));      
      }
    }



  }
}
