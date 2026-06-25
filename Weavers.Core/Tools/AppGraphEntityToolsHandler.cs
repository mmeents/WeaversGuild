using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Models;
using Weavers.Core.Service;


namespace Weavers.Core.Tools {
  public interface IAppGraphEntityToolsHandler {
    Task<string> AddEntityClassModel(int parentItemId, string? className, string? entityDbTableName);
    //Task<string> AddEntityClassImportModel(int classItemId, string? importNamespace);
    Task<string> AddEntityPropertyModel(int entityClassId, string? propertyName, int? propertyTypeId, bool isNav, int? navEntityClassId);
  }


  public class AppGraphEntityToolsHandler : IAppGraphEntityToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AppGraphEntityToolsHandler> _logger;

    public AppGraphEntityToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<AppGraphEntityToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }



    public async Task<string> AddEntityClassModel(int parentItemId, string? className, string? entityDbTableName) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var parentItem = await context.GetItemDtoById(parentItemId);
        if (parentItem == null || !parentItem.IsValidNamespaceParent()) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddClass, parentItemId);
        }

        var addedItem = await service.AddEntityClassModel(parentItem, className, entityDbTableName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddClass, parentItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddEntityClass, await context.ToSummary(addedItem));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddEntityClass, parentItemId, $"Failed to add Entity Class {className}");
      }
    }

    /*  public async Task<string> AddEntityClassImportModel(int classItemId, string? importNamespace) {
          try {

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
            var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var parentItem = await context.GetItemDtoById(classItemId);
            if (parentItem == null || !parentItem.IsValidNamespaceParent()) {
              return _logger.DefaultInvalidParentMessage(Cx.CmdAddClass, classItemId);
            }

            var addedItem = await service.AddEntityClassModel(parentItem, className);
            if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddClass, classItemId);
            var opResult = McpOpResult.CreateSuccess(Cx.CmdAddEntityClass, await context.ToSummary(addedItem));
            return opResult.ToString();
          } catch (Exception ex) {
            return ex.ToOpResult(_logger, Cx.CmdAddEntityClass, classItemId, $"Failed to add Class Import {importNamespace}");
          }
        }  */

    public async Task<string> AddEntityPropertyModel(int entityClassId, string? propertyName, int? propertyTypeId, bool isNav, int? navEntityClassId) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var parentItem = await context.GetItemDtoById(entityClassId);
        if (parentItem == null || !parentItem.IsValidNamespaceParent()) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddEntityProperty, entityClassId);
        }

        int? newNavEntityClassId = navEntityClassId.HasValue && navEntityClassId.Value == 0 ? null : navEntityClassId;
        var addedItem = await service.AddEntityPropertyModel(parentItem, propertyName, propertyTypeId, isNav, newNavEntityClassId);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddEntityProperty, entityClassId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddEntityProperty, await context.ToSummary(addedItem));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddEntityProperty, entityClassId, $"Failed to add Property {propertyName}");
      }
    }
  }
}
