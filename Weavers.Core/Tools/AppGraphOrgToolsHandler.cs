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
    Task<string> AddOrgDeskRole(int orgDeskRolesId, string? roleName);
    Task<string> AddOrgDesk(int workGroupItemId, string deskName);
    Task<string> AddDeskTodo(int orgDeskId, string todoName, int? refId, string? promptTemplate);
    Task<string> AddDigitalOperator(int parentItemId, string operatorName);
    Task<string> AddOrgFolder(int parentItemId, string subFolderName);
    Task<string> AddOrgFile(int folderItemId, string fileName, string fileContent);

    Task<string> AddRssFolder(int parentItemId, string subFolderName);
    Task<string> AddRssChannel(int folderItemId, string channelName, string channelUrl);
    Task<string> RssResyncChannel(int rssChannelId);
    Task<string> RssResolveLink(int rssLinkedHtmlItemId);
    Task<string> RssExtractLinks(int rssLinkedHtmlItemId);

    Task<string> AppendGuildNote(int rssItemId, string noteContent);
    Task<string> UpdateGuildNote(int rssItemId, string noteContent);
    Task<string> ArchiveItem(int itemId);
    Task<string> UnarchiveItem(int itemId);

  }
  public class AppGraphOrgToolsHandler : IAppGraphOrgToolsHandler {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AppGraphOrgToolsHandler> _logger;

    public AppGraphOrgToolsHandler(IServiceScopeFactory scopeFactory, ILogger<AppGraphOrgToolsHandler> logger) {
      _scopeFactory = scopeFactory;
      _logger = logger;
    }

    public async Task<string> AddOrgDeskRole(int orgDeskRolesId, string? roleName) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(orgDeskRolesId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddOrgDeskRole, orgDeskRolesId);
        if (item.ItemTypeId != (int)WeItemType.OrgDeskRolesModel) return _logger.DefaultInvalidParentMessage(Cx.CmdAddOrgDeskRole, orgDeskRolesId);
        var addedItem = await service.AddOrgDeskRole(item, roleName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddOrgDeskRole, orgDeskRolesId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddOrgDeskRole, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddOrgDeskRole, orgDeskRolesId, $"Failed to add role {roleName} to parent item with ID {orgDeskRolesId}");
      }
    }

    public async Task<string> AddOrgDesk(int workGroupItemId, string deskName) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(workGroupItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddOrgDesk, workGroupItemId);
        if (item.ItemTypeId != (int)WeItemType.WorkGroupModel) return _logger.DefaultInvalidParentMessage(Cx.CmdAddOrgDesk, workGroupItemId);
        var addedItem = await service.AddOrgDesk(item, deskName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddOrgDesk, workGroupItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddOrgDesk, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddOrgDesk, workGroupItemId, $"Failed to add desk {deskName} to parent item with ID {workGroupItemId}");
      }
    }

    public async Task<string> AddDeskTodo(int orgDeskId, string todoName, int? refId, string? promptTemplate) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(orgDeskId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddDeskTodo, orgDeskId);
        if (item.ItemTypeId != (int)WeItemType.DeskModel) return _logger.DefaultInvalidParentMessage(Cx.CmdAddDeskTodo, orgDeskId);
        var addedItem = await service.AddDeskTodo(item, todoName, refId, promptTemplate);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddDeskTodo, orgDeskId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddDeskTodo, await context.ToSummary(addedItem, false));
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
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddDigitalOperator, await context.ToSummary(addedItem, false));
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
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddOrgFolder, await context.ToSummary(addedItem, false));
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
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddOrgFile, await context.ToSummary(addedItem, true));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddOrgFile, folderItemId, $"Failed to add file {fileName} to parent item with ID {folderItemId}");
      }
    }

    public async Task<string> AddRssFolder(int parentItemId, string subFolderName) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(parentItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddRssFolder, parentItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddRssFolder, parentItemId);
        var addedItem = await service.AddRssFolder(item, subFolderName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddRssFolder, parentItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddRssFolder, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddRssFolder, parentItemId, $"Failed to add RSS folder {subFolderName} to parent item with ID {parentItemId}");
      }
    }
    public async Task<string> AddRssChannel(int parentItemId, string channelName, string channelUrl) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(parentItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddRssChannel, parentItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddRssChannel, parentItemId);
        var addedItem = await service.AddRssChannel(item, channelName, channelUrl);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddRssChannel, parentItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddRssChannel, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddRssChannel, parentItemId, $"Failed to add RSS channel {channelName} to parent item with ID {parentItemId}");
      }
    }

    public async Task<string> RssResyncChannel(int rssChannelId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(rssChannelId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdRssResyncChannel, rssChannelId);
        if (item.ItemTypeId != (int)WeItemType.RssChannelModel) return _logger.DefaultInvalidParentMessage(Cx.CmdRssResyncChannel, rssChannelId);
        var resyncResult = await service.RssResyncChannel(item);
        if (resyncResult == null) return _logger.DefaultAddEmptyMessage(Cx.CmdRssResyncChannel, rssChannelId);
        var summary = await context.ToSummary(resyncResult, false);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdRssResyncChannel, summary);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdRssResyncChannel, rssChannelId, $"Failed to resync RSS channel with ID {rssChannelId}");
      }
    }

    public async Task<string> RssResolveLink(int rssLinkedHtmlItemId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(rssLinkedHtmlItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdRssResolveLink, rssLinkedHtmlItemId);
        if (item.ItemTypeId != (int)WeItemType.RssLinkedHtmlModel && item.ItemTypeId != (int)WeItemType.RssItemModel) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdRssResolveLink, rssLinkedHtmlItemId);
        }
        var resolveResult = await service.RssResolveLink(item);
        if (resolveResult == null) return _logger.DefaultAddEmptyMessage(Cx.CmdRssResolveLink, rssLinkedHtmlItemId);
        var summary = await context.ToSummary(resolveResult, true);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdRssResolveLink, summary);
        return opResult.ToString();
      } catch (Exception ex) { 
        return ex.ToOpResult(_logger, Cx.CmdRssResolveLink, rssLinkedHtmlItemId, $"Failed to resolve RSS link with ID {rssLinkedHtmlItemId}");
      }
    }

    public async Task<string> RssExtractLinks(int rssLinkedHtmlItemId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(rssLinkedHtmlItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdRssExtractLinks, rssLinkedHtmlItemId);
        if (item.ItemTypeId != (int)WeItemType.RssLinkedHtmlModel && item.ItemTypeId != (int)WeItemType.RssItemModel) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdRssExtractLinks, rssLinkedHtmlItemId);
        }
        var extractResult = await service.RssExtractLinks(item);
        if (extractResult == null) return _logger.DefaultAddEmptyMessage(Cx.CmdRssExtractLinks, rssLinkedHtmlItemId);
        var summary = await context.ToSummary(extractResult, false);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdRssExtractLinks, summary);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdRssExtractLinks, rssLinkedHtmlItemId, $"Failed to extract RSS links from item with ID {rssLinkedHtmlItemId}");
      }
    }


    public async Task<string> AppendGuildNote(int rssItemId, string noteContent) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();

        var item = await context.GetItemDtoById(rssItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAppendGuildNote, rssItemId);
        if (item.ItemTypeId != (int)WeItemType.RssLinkedHtmlModel 
          && item.ItemTypeId != (int)WeItemType.RssItemModel
          && item.ItemTypeId != (int)WeItemType.RssChannelModel
          && item.ItemTypeId != (int)WeItemType.RssFolderModel) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAppendGuildNote, rssItemId);
        }
        var extractResult = await service.AppendGuildNote(item, noteContent);
        if (extractResult == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAppendGuildNote, rssItemId);
        var summary = await context.ToSummary(extractResult, false);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAppendGuildNote, summary);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAppendGuildNote, rssItemId, $"Failed to append to guild note property with ID {rssItemId}");
      }
    }

    public async Task<string> UpdateGuildNote(int rssItemId, string noteContent) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();

        var item = await context.GetItemDtoById(rssItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdUpdateGuildNote, rssItemId);
        if (item.ItemTypeId != (int)WeItemType.RssLinkedHtmlModel
          && item.ItemTypeId != (int)WeItemType.RssItemModel
          && item.ItemTypeId != (int)WeItemType.RssChannelModel
          && item.ItemTypeId != (int)WeItemType.RssFolderModel) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdUpdateGuildNote, rssItemId);
        }
        var extractResult = await service.UpdateGuildNote(item, noteContent);
        if (extractResult == null) return _logger.DefaultAddEmptyMessage(Cx.CmdUpdateGuildNote, rssItemId);
        var summary = await context.ToSummary(extractResult, false);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdUpdateGuildNote, summary);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdUpdateGuildNote, rssItemId, $"Failed to update guild note property with ID {rssItemId}");
      }
    }

    public async Task<string> ArchiveItem(int itemId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(itemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdArchiveItem, itemId);
        var archiveResult = await service.ArchiveItem(item);        
        var summary = archiveResult ? "Archived" : "Failed to archive";
        var opResult = McpOpResult.CreateSuccess(Cx.CmdArchiveItem, summary);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdArchiveItem, itemId, $"Failed to archive item with ID {itemId}");
      }
    }

    public async Task<string> UnarchiveItem(int itemId) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();        
        var unarchiveResult = await service.UnarchiveItem(itemId);
        var summary = unarchiveResult ? "Unarchived" : "Failed to unarchive";
        var opResult = McpOpResult.CreateSuccess(Cx.CmdUnarchiveItem, summary);
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdUnarchiveItem, itemId, $"Failed to unarchive item with ID {itemId}");
      }
    }   


  }
}
