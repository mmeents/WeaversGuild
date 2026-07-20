using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Models;
using Weavers.Core.Service;


namespace Weavers.Core.Tools {
  public interface IAppGraphClassToolsHandler {
    Task<string> AddClassModel(int parentItemId, string? className, bool generateInterface, bool registerDI);
    Task<string> AddClassImportModel(int classItemId, int importClassId);
    Task<string> AddClassPropModel(int classItemId, string? propertyName, int? propertyTypeId, int? propertyClassId);
    Task<string> AddClassMethodModel(int classItemId, string? methodName, bool? isAsync, int? returnTypeId, int? returnClassId);
    Task<string> AddClassMethodParam(int methodItemId, string? paramName, int? paramTypeId, int? paramClassId);
  }


  public class AppGraphClassToolsHandler : IAppGraphClassToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AppGraphClassToolsHandler> _logger;

    public AppGraphClassToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<AppGraphClassToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    public async Task<string> AddClassModel(int parentItemId, string? className, bool generateInterface, bool registerDI) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var parentItem = await context.GetItemDtoById(parentItemId);
        if (parentItem == null || !parentItem.IsValidNamespaceParent()) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddClass, parentItemId);
        }

        var addedItem = await service.AddClassModel(parentItem, className);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddClass, parentItemId);

        var propGenerateInterface = addedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface);
        if (propGenerateInterface != null) {
          propGenerateInterface.Value = generateInterface ? "1" : "0";
          addedItem = await mediator.UpdateItemProp(addedItem, propGenerateInterface);
        }

        var propRegisterDi = addedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRegisterDi);
        if (propRegisterDi != null) {
          propRegisterDi.Value = registerDI ? "1" : "0";
          addedItem = await mediator.UpdateItemProp(addedItem, propRegisterDi);
          if (registerDI) {
            await mediator.Send(new AddRemoveClassToLibDiCommand(addedItem.Id, registerDI, generateInterface));
          }
        }

        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddClass, await context.ToSummary(addedItem, true));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddClass, parentItemId, $"Failed to add Class {className}");
      }
    }

    public async Task<string> AddClassImportModel(int classItemId, int importClassId) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var classItem = await context.GetItemDtoById(classItemId);
        if (classItem == null || classItem.ItemTypeId != (int)WeItemType.ClassModel) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddClassImport, classItemId);
        }

        var importClassItem = await context.GetItemDtoById(importClassId);
        if (importClassItem == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddClassImport, importClassId);
        var importNamespace = importClassItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace)?.Value ?? "missingNamespace";
        var importClassUsesInterface = importClassItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface)?.Value == "1";

        var addedItem = await service.AddClassImportModel(classItem, importNamespace);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddClassImport, classItemId);

        var importClassProp = addedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItImportObject);
        if (importClassProp != null) {
          importClassProp.Value = importClassItem.Id.ToString();
          addedItem = await mediator.UpdateItemProp(addedItem, importClassProp);
        }

        var importClassInterfaceProp = addedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItImportUseInterface);
        if (importClassInterfaceProp != null) {
          importClassInterfaceProp.Value = importClassUsesInterface ? "1" : "0";
          addedItem = await mediator.UpdateItemProp(addedItem, importClassInterfaceProp);
        }

        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddClassImport, await context.ToSummary(addedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddClassImport, classItemId, $"Failed to add Class Import id:{importClassId}");
      }
    }

    public async Task<string> AddClassPropModel(int classItemId, string? propertyName, int? propertyTypeId, int? propertyClassId) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();        

        var classItem = await context.GetItemDtoById(classItemId);
        if (classItem == null || classItem.ItemTypeId != (int)WeItemType.ClassModel) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddClassProperty, classItemId);
        }

        var addedItem = await service.AddClassPropModel(classItem, propertyName, propertyTypeId, propertyClassId);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddClassProperty, classItemId);

        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddClassProperty, await context.ToSummary(addedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddClassProperty, classItemId, $"Failed to add Class Property id:{classItemId}");
      }
    }

    public async Task<string> AddClassMethodModel(int classItemId, string? methodName, bool? isAsync, int? returnTypeId, int? returnClassId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();

        var classItem = await context.GetItemDtoById(classItemId);
        if (classItem == null || classItem.ItemTypeId != (int)WeItemType.ClassModel) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddClassMethod, classItemId);
        }

        var addedItem = await service.AddClassMethodModel(classItem, methodName, isAsync, returnTypeId, returnClassId);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddClassMethod, classItemId);

        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddClassMethod, await context.ToSummary(addedItem, true));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddClassMethod, classItemId, $"Failed to add Class Method id:{classItemId}");
      }
    }

    public async Task<string> AddClassMethodParam(int methodItemId, string? paramName, int? paramTypeId, int? paramClassId) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var classItem = await context.GetItemDtoById(methodItemId);
        if (classItem == null || !classItem.IsValidNamespaceParent() || classItem.ItemTypeId != (int)WeItemType.ClassModel) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddClassMethodParam, methodItemId);
        }

        var addedItem = await service.AddClassMethodParam(classItem, paramName, paramTypeId, paramClassId);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddClassMethodParam, methodItemId);

        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddClassMethodParam, await context.ToSummary(addedItem, true));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddClassMethodParam, methodItemId, $"Failed to add Class Method Parameter id:{methodItemId}");
      }
    }

  }


}



