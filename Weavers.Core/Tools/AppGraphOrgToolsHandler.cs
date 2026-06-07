using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Tools {
  public interface IAppGraphOrgToolsHandler {

    Task<string> AddOrgDesk(int orgChartId, string deskName);
    Task<string> AddDeskTodo(int orgDeskId, string todoName);
    Task<string> AddDigitalOperator(int parentItemId, string operatorName);
    Task<string> AddOrgFolder(int parentItemId, string subFolderName);
    Task<string> AddOrgFile(int folderItemId, string fileName, string fileContent);
  }
  public class AppGraphOrgToolsHandler : IAppGraphOrgToolsHandler {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AppGraphOrgToolsHandler> _logger;

    public AppGraphOrgToolsHandler(IServiceScopeFactory scopeFactory, ILogger<AppGraphOrgToolsHandler> logger) {
      _scopeFactory = scopeFactory;
      _logger = logger;
    }

    public async Task<string> AddOrgDesk(int orgChartId, string deskName) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(orgChartId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddOrgDesk, orgChartId);
        if (item.ItemTypeId != (int)WeItemType.OrgChartModel) return _logger.DefaultInvalidParentMessage(Cx.CmdAddOrgDesk, orgChartId);
        var addedItem = await service.AddOrgDesk(item, deskName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddOrgDesk, orgChartId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddOrgDesk, await context.ToSummary(addedItem));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddOrgDesk, orgChartId, $"Failed to add desk {deskName} to parent item with ID {orgChartId}");
      }
    }

    public async Task<string> AddDeskTodo(int orgDeskId, string todoName) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(orgDeskId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddDeskTodo, orgDeskId);
        if (item.ItemTypeId != (int)WeItemType.DeskModel) return _logger.DefaultInvalidParentMessage(Cx.CmdAddDeskTodo, orgDeskId);
        var addedItem = await service.AddDeskTodo(item, todoName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddDeskTodo, orgDeskId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddDeskTodo, await context.ToSummary(addedItem));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddDeskTodo, orgDeskId, $"Failed to add todo {todoName} to parent item with ID {orgDeskId}");
      }
    }

    public async Task<string> AddDigitalOperator(int parentItemId, string operatorName) {
      try {

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(parentItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddDigitalOperator, parentItemId);
        if (item.ItemTypeId != (int)WeItemType.DigitalOperatorPoolModel) return _logger.DefaultInvalidParentMessage(Cx.CmdAddDigitalOperator, parentItemId);
        var addedItem = await service.AddDigitalOperator(item, operatorName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddDigitalOperator, parentItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddDigitalOperator, await context.ToSummary(addedItem));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddDigitalOperator, parentItemId, $"Failed to add digital operator {operatorName} to parent item with ID {parentItemId}");
      }
    }
    public async Task<string> AddOrgFolder(int parentItemId, string subFolderName) {
      try {

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(parentItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddOrgFolder, parentItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddOrgFolder, parentItemId);
        var addedItem = await service.AddOrgFolder(item, subFolderName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddOrgFolder, parentItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddOrgFolder, await context.ToSummary(addedItem));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddOrgFolder, parentItemId, $"Failed to add folder {subFolderName} to parent item with ID {parentItemId}");
      }
    }
    public async Task<string> AddOrgFile(int folderItemId, string fileName, string fileContent) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(folderItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddOrgFile, folderItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddOrgFile, folderItemId);
        var addedItem = await service.AddOrgFile(item, fileName, fileContent);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddOrgFile, folderItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddOrgFile, await context.ToSummary(addedItem));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddOrgFile, folderItemId, $"Failed to add file {fileName} to parent item with ID {folderItemId}");
      }
    }


  }
}
