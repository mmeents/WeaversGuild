using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Service {
  public interface IAppGraphClassService {
    Task<ItemDto?> AddLibrary(ItemDto folderItem, string? libraryName);
    Task<ItemDto?> AddDiModel(ItemDto libraryItem, string? diModelName);
    Task<ItemDto?> AddNamespaceModel(ItemDto parentItem, string? namespaceName);
    Task<ItemDto?> AddClassModel(ItemDto parentItem, string? className);
    Task<ItemDto?> AddClassImportModel(ItemDto classItem, string? importNamespace);
    Task<ItemDto?> AddClassPropModel(ItemDto classItem, string? propertyName, int? propertyTypeId, int? propertyClassId);
    Task<ItemDto?> AddClassMethodModel(ItemDto classItem, string? methodName, bool? isAsync, int? returnTypeId, int? returnClassId);
    Task<ItemDto?> AddClassMethodParam(ItemDto methodItem, string? paramName, int? paramTypeId, int? paramClassId);
    Task<ItemDto?> AddEntityClassModel(ItemDto parentItem, string? className, string? entityDbTableName);
    Task<ItemDto?> AddEntityClassImportModel(ItemDto classItem, string? importNamespace);
    Task<ItemDto?> AddEntityPropertyModel(ItemDto classItem, string? propertyName, int? propertyTypeId, bool isNav, int? navEntityClassId);

  }
  public class AppGraphClassService : IAppGraphClassService {
    private readonly IServiceScopeFactory _scopeFactory;
    public AppGraphClassService(IServiceScopeFactory serviceScopeFactory) {
      _scopeFactory = serviceScopeFactory;
    }
    private IMediator GetMediator() {
      var scope = _scopeFactory.CreateScope();
      return scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public async Task<ItemDto?> AddLibrary(ItemDto folderItem, string? libraryName) {
      var mediator = GetMediator();

      if (!folderItem.IsValidFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(libraryName)) nextRank = await mediator.Send(new GetNextItemRankQuery(folderItem.Id)) + 1;
      var name = libraryName == null ? $"Library{nextRank}" : libraryName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(folderItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.LibraryModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }     

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = folderItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fileName = newSubItem.Name.Contains('.') ? newSubItem.Name : newSubItem.Name.UrlSafe() + ".csproj";
        var fullPath = Path.Combine(parentFolderPath, fileName);
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);
      }

      var rootNamespaceProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespaceRoot);
      if (rootNamespaceProperty != null && string.IsNullOrEmpty(rootNamespaceProperty.Value)) {
        rootNamespaceProperty.Value = newSubItem.Name.NameSafe();
        await rootNamespaceProperty.SaveProp(newSubItem, mediator);
      }
      return newSubItem;
    }

    public async Task<ItemDto?> AddDiModel(ItemDto libraryItem, string? diModelName) {
      var mediator = GetMediator();
      if (libraryItem.ItemTypeId != (int)WeItemType.LibraryModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(diModelName)) nextRank = await mediator.Send(new GetNextItemRankQuery(libraryItem.Id)) + 1;
      var name = diModelName == null ? $"DiModel{nextRank}" : diModelName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(libraryItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.DependencyInjectionModel, name, "", "{}"));
      if (newSubItem == null) {        
        return null;
      }           

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null) {
        string parentFolderPath = libraryItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fileName = "DependencyInjection.cs";
        var fullPath = Path.Combine(parentFolderPath, fileName);
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);
      }

      var rootNamespaceProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace);
      if (rootNamespaceProperty != null ) {
        string newNamespace = libraryItem.ResolveItemsNamespace("NoParentNamespace");
        rootNamespaceProperty.Value = newNamespace;
        await rootNamespaceProperty.SaveProp(newSubItem, mediator);
      }
      return newSubItem;
    }

    public async Task<ItemDto?> AddNamespaceModel(ItemDto parentItem, string? namespaceName) {
      var mediator = GetMediator();
      if (parentItem.ItemTypeId != (int)WeItemType.LibraryModel && parentItem.ItemTypeId != (int)WeItemType.NamespaceModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(namespaceName)) nextRank = await mediator.Send(new GetNextItemRankQuery(parentItem.Id)) + 1;
      var name = namespaceName == null ? $"Namespace{nextRank}" : namespaceName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.NamespaceModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null) {
        string parentFolderPath = parentItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var newFolderName = newSubItem.Name.UrlSafe();
        var fullPath = Path.Combine(parentFolderPath, newFolderName);
        if (rootFolderProperty.Value != fullPath) {
          rootFolderProperty.Value = fullPath;
          await rootFolderProperty.SaveProp(newSubItem,mediator);
        }
      }

      var rootNamespaceProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace);
      if (rootNamespaceProperty != null ) {        
        string newNamespace = parentItem.ResolveItemsNamespace("NoParentNamespace");        
        rootNamespaceProperty.Value = newNamespace + "." + newSubItem.Name.NameSafe();
        await rootNamespaceProperty.SaveProp(newSubItem, mediator);
      }
      return newSubItem;
    }

    public async Task<ItemDto?> AddClassModel(ItemDto parentItem, string? className) {
      var mediator = GetMediator();
      if (parentItem.ItemTypeId != (int)WeItemType.LibraryModel && parentItem.ItemTypeId != (int)WeItemType.NamespaceModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(className)) nextRank = await mediator.Send(new GetNextItemRankQuery(parentItem.Id)) + 1;
      var name = className == null ? $"Class{nextRank}" : className;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.ClassModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var classFilePathProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      var fileExtProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFileExt);
      if (classFilePathProperty != null && fileExtProperty != null) {
        string parentFolderPath = parentItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var newFileExt = fileExtProperty.Value ?? ".cs";
        var newFolderName = newSubItem.Name.UrlSafe();
        var fullPath = Path.Combine(parentFolderPath, newFolderName + newFileExt);
        if (classFilePathProperty.Value != fullPath) {
          classFilePathProperty.Value = fullPath;
          await classFilePathProperty.SaveProp(newSubItem, mediator);
        }
      }

      var rootNamespaceProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace || p.Name == Cx.ItNamespaceRoot);
      if (rootNamespaceProperty != null) {
        string newNamespace = parentItem.ResolveItemsNamespace("NoParentNamespace");
        rootNamespaceProperty.Value = newNamespace + "." + newSubItem.Name.NameSafe();
        await rootNamespaceProperty.SaveProp(newSubItem, mediator);
      }
      return newSubItem;
    }

    public async Task<ItemDto?> AddClassImportModel(ItemDto classItem, string? importNamespace) {
      var mediator = GetMediator();
      if (classItem.ItemTypeId != (int)WeItemType.ClassModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(importNamespace)) nextRank = await mediator.Send(new GetNextItemRankQuery(classItem.Id)) + 1;
      var name = importNamespace == null ? $"Import{nextRank}" : importNamespace;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(classItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.ClassImportModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }     
      return newSubItem;
    }

    public async Task<ItemDto?> AddClassPropModel(ItemDto classItem, string? propertyName, int? propertyTypeId, int? propertyClassId) {
      var mediator = GetMediator();
      if (classItem.ItemTypeId != (int)WeItemType.ClassModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(propertyName)) nextRank = await mediator.Send(new GetNextItemRankQuery(classItem.Id)) + 1;
      var name = propertyName == null ? $"Property{nextRank}" : propertyName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(classItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.ClassPropertyModel, name, "", "{}"));
      if (newSubItem == null) { return null; }

      if (propertyTypeId.HasValue) {
        var propTypeProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyDataType);
        if (propTypeProperty != null) {
          propTypeProperty.Value = propertyTypeId.Value.ToString();
          await propTypeProperty.SaveProp(newSubItem, mediator);

          if (propertyTypeId == (int)WeItemType.CSharpClassType || propertyTypeId == (int)WeItemType.CSharpRecordType || propertyTypeId == (int)WeItemType.CSharpStructType) {
            if (propertyClassId.HasValue) {
              var propClassProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType);
              if (propClassProperty != null) {
                propClassProperty.Value = propertyClassId.Value.ToString();
                await propClassProperty.SaveProp(newSubItem, mediator);
              }
            }
          }
        }
      }

      return newSubItem;
    }

    public async Task<ItemDto?> AddClassMethodModel(ItemDto classItem, string? methodName, bool? isAsync, int? returnTypeId, int? returnClassId) {
      var mediator = GetMediator();
      if (classItem.ItemTypeId != (int)WeItemType.ClassModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(methodName)) nextRank = await mediator.Send(new GetNextItemRankQuery(classItem.Id)) + 1;
      var name = methodName == null ? $"Method{nextRank}" : methodName;
      var newMethodItem = await mediator.Send(
        new CreateRelatedItemCommand(classItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.ClassMethodModel, name, "", "{}"));
      if (newMethodItem == null) { return null; }
      var classIsStatic = classItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsStatic)?.Value.AsBoolean();
      if (classIsStatic != null && classIsStatic.Value) { // default is false.
        var methodStaticProp = newMethodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsStatic);
        if (methodStaticProp != null) {
          methodStaticProp.Value = "true";
          await methodStaticProp.SaveProp(newMethodItem, mediator);
        }
      }
      if (isAsync != null && isAsync.Value) {  // default is false.
        var methodAsyncProp = newMethodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsAsync);
        if (methodAsyncProp != null) {
          methodAsyncProp.Value = "1";
          await methodAsyncProp.SaveProp(newMethodItem, mediator);
        }
      }

      if (returnTypeId.HasValue) {
        var returnTypeProperty = newMethodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReturnDataType);
        if (returnTypeProperty != null) {
          returnTypeProperty.Value = returnTypeId.Value.ToString();
          await returnTypeProperty.SaveProp(newMethodItem, mediator);

          if (returnTypeId == (int)WeItemType.CSharpClassType || returnTypeId == (int)WeItemType.CSharpRecordType || returnTypeId == (int)WeItemType.CSharpStructType) {
            if (returnClassId.HasValue) {
              var propClassProperty = newMethodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReturnClassType);
              if (propClassProperty != null) {
                propClassProperty.Value = returnClassId.Value.ToString();
                await propClassProperty.SaveProp(newMethodItem, mediator);
              }
            }
          }
        }
      }

      return newMethodItem;
    }

    public async Task<ItemDto?> AddClassMethodParam(ItemDto methodItem, string? paramName, int? paramTypeId, int? paramClassId) {
      var mediator = GetMediator();
      if (methodItem.ItemTypeId != (int)WeItemType.ClassMethodModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(paramName)) nextRank = await mediator.Send(new GetNextItemRankQuery(methodItem.Id)) + 1;
      var name = paramName == null ? $"Param{nextRank}" : paramName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(methodItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.ClassMethodParameterModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      if (paramTypeId.HasValue) {
        var propTypeProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterDataType);
        if (propTypeProperty != null) {
          propTypeProperty.Value = paramTypeId.Value.ToString();
          await propTypeProperty.SaveProp(newSubItem, mediator);

          if (paramTypeId == (int)WeItemType.CSharpClassType || paramTypeId == (int)WeItemType.CSharpRecordType || paramTypeId == (int)WeItemType.CSharpStructType) {
            if (paramClassId.HasValue) {
              var propClassProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType);
              if (propClassProperty != null) {
                propClassProperty.Value = paramClassId.Value.ToString();
                await propClassProperty.SaveProp(newSubItem, mediator);
              }
            }
          }
        }
      }

      return newSubItem;
    }

    public async Task<ItemDto?> AddEntityClassModel(ItemDto parentItem, string? className, string? entityDbTableName = null) {
      var mediator = GetMediator();
      if (parentItem.ItemTypeId != (int)WeItemType.LibraryModel && parentItem.ItemTypeId != (int)WeItemType.NamespaceModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(className)) nextRank = await mediator.Send(new GetNextItemRankQuery(parentItem.Id)) + 1;
      var name = className == null ? $"Entity{nextRank}" : className;
      var newEntityItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, 
          (int)WeRelationTypes.Contains, 
          (int)WeItemType.EntityClassModel, 
          name, "", "{}"));
      if (newEntityItem == null) { return null; }

      await mediator.Send(new AddRemoveEntityToDbContextCommand(newEntityItem.Id, true));

      var classFilePathProperty = newEntityItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      string parentFolderPath = parentItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
      var fileExtProperty = newEntityItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFileExt);
      if (classFilePathProperty != null && fileExtProperty != null) {
        var newFileExt = fileExtProperty.Value ?? ".cs";
        var newFolderName = newEntityItem.Name.UrlSafe();
        var fullPath = Path.Combine(parentFolderPath, newFolderName + newFileExt);
        if (classFilePathProperty.Value != fullPath) {
          classFilePathProperty.Value = fullPath;
          await classFilePathProperty.SaveProp(newEntityItem, mediator);
        }
      }

      string newNamespace = parentItem.ResolveItemsNamespace("NoParentNamespace");
      var rootNamespaceProperty = newEntityItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace || p.Name == Cx.ItNamespaceRoot);
      if (rootNamespaceProperty != null) {      
        rootNamespaceProperty.Value = newNamespace;  // classes sit in parents namespace.
        await rootNamespaceProperty.SaveProp(newEntityItem, mediator);
      }

      var newPkProperty = await mediator.Send(
        new CreateRelatedItemCommand(newEntityItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.EntityPropertyModel, "Id", "", "{\"IsKey\":true}"));
      if (newPkProperty != null) { 
        var isPrimaryKeyProp = newPkProperty.Properties.FirstOrDefault(p => p.Name == Cx.ItIsPrimaryKey);
        if (isPrimaryKeyProp != null) {
          isPrimaryKeyProp.Value = "true";
          await isPrimaryKeyProp.SaveProp(newPkProperty, mediator);
        }
      }

      var newDbTableProp = newEntityItem.Properties.FirstOrDefault(p => p.Name == Cx.ItDbTableName);
      if (newDbTableProp != null) {
        string newDbTablename = "";
        if (!string.IsNullOrEmpty(entityDbTableName)) {
          newDbTablename = entityDbTableName;
        } else { 
          newDbTablename = newEntityItem.Name.UrlSafe()+"s";
        }        
        if (newDbTableProp.Value != newDbTablename) {
          newDbTableProp.Value = newDbTablename;
          await newDbTableProp.SaveProp(newEntityItem, mediator);
        }
      }

      var newEntityConfigItem = await mediator.Send(
        new CreateRelatedItemCommand(newEntityItem.Id,
          (int)WeRelationTypes.Contains,
          (int)WeItemType.EntityConfigurationModel,
          $"{name}Config", "", "{}"));
      if (newEntityConfigItem != null) {
        var configFilePathProperty = newEntityConfigItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        var configExtProperty = newEntityConfigItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFileExt);
        if (configFilePathProperty != null && configExtProperty != null) {          
          var newFileExt = configExtProperty.Value ?? ".cs";
          var newConfigName = newEntityConfigItem.Name.UrlSafe();
          var fullPath = Path.Combine(parentFolderPath, newConfigName + newFileExt);
          if (configFilePathProperty.Value != fullPath) {
            configFilePathProperty.Value = fullPath;
            await configFilePathProperty.SaveProp(newEntityConfigItem, mediator);
          }
        }

        var configNamespaceProperty = newEntityConfigItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace || p.Name == Cx.ItNamespaceRoot);
        if (configNamespaceProperty != null) {          
          configNamespaceProperty.Value = newNamespace; // config sit in same namespace as entity.
          await configNamespaceProperty.SaveProp(newEntityConfigItem, mediator);
        }
      }
      return newEntityItem;
    }

    public async Task<ItemDto?> AddEntityClassImportModel(ItemDto classItem, string? importNamespace) {
      var mediator = GetMediator();
      if (classItem.ItemTypeId != (int)WeItemType.EntityClassModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(importNamespace)) nextRank = await mediator.Send(new GetNextItemRankQuery(classItem.Id)) + 1;
      var name = importNamespace == null ? $"Import{nextRank}" : importNamespace;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(classItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.EntityClassImportModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }
      return newSubItem;
    }

    public async Task<ItemDto?> AddEntityPropertyModel(ItemDto classItem, string? propertyName, int? propertyTypeId, bool isNav, int? navEntityClassId) {
      var mediator = GetMediator();
      if (classItem.ItemTypeId != (int)WeItemType.EntityClassModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(propertyName)) nextRank = await mediator.Send(new GetNextItemRankQuery(classItem.Id)) + 1;
      var name = propertyName == null ? $"Property{nextRank}" : propertyName;
      var theNewEntityPropItem = await mediator.Send(
        new CreateRelatedItemCommand(classItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.EntityPropertyModel, name, "", "{}"));
      if (theNewEntityPropItem == null) {
         return null;
      }

      if (propertyTypeId.HasValue) {
        var propTypeProperty = theNewEntityPropItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyDataType);
        if (propTypeProperty != null) {
          propTypeProperty.Value = propertyTypeId.Value.ToString();
          await propTypeProperty.SaveProp(theNewEntityPropItem, mediator);
        }
      }

      if (isNav) {
        var navProp = theNewEntityPropItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation);
        if (navProp != null) {
          navProp.Value = "1";
          await navProp.SaveProp(theNewEntityPropItem, mediator);
          await mediator.Send(new UpdateItemPropertyCommand(navProp.Id, "1")); // this adds an extra navigation child item.
          theNewEntityPropItem = await mediator.Send(new GetItemByIdQuery(theNewEntityPropItem.Id)); // this forces the item to reload with the new navigation child.
          if (navEntityClassId.HasValue && theNewEntityPropItem != null) {
            var navEntityNavId = theNewEntityPropItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationModel)?.RelatedItemId;
            if (navEntityNavId != null) {
              var navEntityNavItem = await mediator.Send(new GetItemByIdQuery(navEntityNavId.Value));
              var propClassProperty = navEntityNavItem?.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType);
              if (propClassProperty != null && navEntityNavItem != null) {
                propClassProperty.Value = navEntityClassId.Value.ToString();
                await propClassProperty.SaveProp(navEntityNavItem, mediator);
                await mediator.Send(new UpdateItemPropertyCommand(propClassProperty.Id, navEntityClassId.Value.ToString())); // this runs the sync on property which picks up on the class set.    
              }
            }
          }
        }

        
      }

      return theNewEntityPropItem;
    }









    // below is end of class and end of namespace, leave it.


  }
}
