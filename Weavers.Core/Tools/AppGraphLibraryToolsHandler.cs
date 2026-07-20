using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Tools {

  public interface IAppGraphLibraryToolsHandler {
    Task<string> AddLibrary(int folderItemId, string? libraryName);
    Task<string> AddDiModel(int libraryItemId, bool hasDbContext);
    Task<string> AddNamespace(int parentItemId, string? namespaceName);
    
  }

  public class AppGraphLibraryToolsHandler : IAppGraphLibraryToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AppGraphLibraryToolsHandler> _logger;

    public AppGraphLibraryToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<AppGraphLibraryToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }


    public async Task<string> AddLibrary(int folderItemId, string? libraryName) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();

        var parentItem = await context.GetItemDtoById(folderItemId);
        if (parentItem == null || !parentItem.IsValidFolderParent()) { 
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddLibrary, folderItemId); 
        }

        var addedItem = await service.AddLibrary(parentItem, libraryName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddLibrary, folderItemId);

        await this.AddDiModel(addedItem.Id, true);

        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddLibrary, await context.ToSummary(addedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddLibrary, folderItemId, $"Failed to add library {libraryName}");
      }
    }


    public async Task<string> AddDiModel(int libraryItemId, bool hasDbContext) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();        
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var parentItem = await context.GetItemDtoById(libraryItemId);
        if (parentItem == null || parentItem.ItemTypeId != (int)WeItemType.LibraryModel) { 
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddDiModel, libraryItemId); 
        }

        var addedItem = await service.AddDiModel(parentItem, (string?)null);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddDiModel, libraryItemId);

        var mediatorProp = addedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasMediator);
        if (mediatorProp != null) { 
          mediatorProp.Value = "1";
          addedItem = await mediator.UpdateItemProp(addedItem, mediatorProp);
        }

        var dbContextProp = addedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasDbContext);
        if (dbContextProp != null && hasDbContext) {
          dbContextProp.Value = "1";          
          addedItem = await mediator.Send(new UpdateItemPropertyCommand( dbContextProp.Id, "1"));          
        }

        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddDiModel, await context.ToSummary(addedItem!, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddDiModel, libraryItemId, $"Failed to add DI model");
      }
    }    


    public async Task<string> AddNamespace(int parentItemId, string? namespaceName) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();

        var parentItem = await context.GetItemDtoById(parentItemId);
        if (parentItem == null || !parentItem.IsValidNamespaceParent()) {
          return _logger.DefaultInvalidParentMessage(Cx.CmdAddNamespace, parentItemId);
        }

        var addedItem = await service.AddNamespaceModel(parentItem, namespaceName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddNamespace, parentItemId);

        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddNamespace, await context.ToSummary(addedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddNamespace, parentItemId, $"Failed to add Namespace {namespaceName}");
      }
    }

  }
}
