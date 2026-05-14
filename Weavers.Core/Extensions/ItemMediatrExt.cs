using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemMediatrExt {
    public static async Task SyncLibraryPackageDefaults(this IMediator mediator, 
      ItemDto? libraryItem, PkgType which, bool isAdd, CancellationToken cancellationToken) {

      if (libraryItem == null) return;
      var defaultLibPackage = PkgEx.Defaults;
      var DefaultsToAdd = defaultLibPackage[which];
      foreach (var def in DefaultsToAdd) {
        if (def != null) {
          var itemName = def.PackageInclude;
          var foundRelation = libraryItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.LibPackageRefModel && string.Compare(itemName, r.RelatedItemName, true) == 0);
          if (foundRelation == null && isAdd) {   // is add starts
            var newItem = await mediator.Send(new CreateRelatedItemCommand(libraryItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.LibPackageRefModel,
              itemName, "", "{}"), cancellationToken);
            if (newItem != null) {              

              var includeProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPackageInclude);
              var versionProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPackageVersion);
              var privateProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPrivateAssets);
              var includeAssetProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIncludeAssets);

              if (includeProp != null) {
                includeProp.Value = def.PackageInclude;
                await mediator.Send(includeProp.ToSaveCmd(), cancellationToken);
              }
              if (versionProp != null) {
                versionProp.Value = def.PackageVersion;
                await mediator.Send(versionProp.ToSaveCmd(), cancellationToken);
              }
              if (privateProp != null && def.PrivateAssets != "") {
                privateProp.Value = def.PrivateAssets;
                await mediator.Send(privateProp.ToSaveCmd(), cancellationToken);
              }
              if (includeAssetProp != null && def.IncludeAssets != "") {
                includeAssetProp.Value = def.IncludeAssets;
                await mediator.Send(includeAssetProp.ToSaveCmd(), cancellationToken);
              }
            }
          } else if (foundRelation != null && !isAdd) {  // is remove starts
            if (foundRelation != null && foundRelation.RelatedItemId != null) {
              await mediator.Send(new DeleteItemCommand(foundRelation.RelatedItemId.Value), cancellationToken);
            }
          }
        }
      }


    } 
  }
}
