using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Service;
using MediatR;

namespace Weavers.Core.Tools {
  public interface IAppGraphFileToolsHandler {
    Task<string> AddProjectRoot(string projectRootName);
    Task<string> AddSubFolder(int itemId, string subFolderName);
    Task<string> AddSolution(int folderItemId, string solutionName);
    Task<string> AddSolutionImport(int solutionItemId, int importLibraryId);
    Task<string> AddFile(int folderItemId, string fileName, string fileContent);

  }

  public class AppGraphFileToolsHandler : IAppGraphFileToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AppGraphFileToolsHandler> _logger;
    public AppGraphFileToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<AppGraphFileToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    public async Task<string> AddProjectRoot(string projectRootName) { 
        try {

          using var scope = _serviceScopeFactory.CreateScope();
          var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
          var settings = scope.ServiceProvider.GetRequiredService<IAppSettingService>();
          string defaultPath = settings[Cx.ApsDefaultFolder]?.Value ?? WeaverExt.AppProjectsPath;
          string filePath = Path.Combine(defaultPath, projectRootName.UrlSafe());
          var addedItem = await service.AddProjectRoot(projectRootName, filePath);
          if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddProjectRoot, 0);          
          var opResult = McpOpResult.CreateSuccess(Cx.CmdAddProjectRoot, addedItem.ToSummary());
          return opResult.ToString();        

        } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddProjectRoot, 0, $"Failed to add project root {projectRootName}");
      }
    }

    public async Task<string> AddSubFolder(int itemId, string subFolderName) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await dbContext.GetItemDtoById(itemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddSubFolder, itemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddSubFolder, itemId);
        var addedItem = await service.AddSubFolder(item, subFolderName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddSubFolder, itemId);        
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddSubFolder, addedItem.ToSummary());
        return opResult.ToString();        

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddSubFolder, itemId, $"Failed to add folder {subFolderName} to parent item with ID {itemId}");
      }
    }

    public async Task<string> AddSolution(int folderItemId, string solutionName) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await dbContext.GetItemDtoById(folderItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddSolution, folderItemId);       
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddSolution, folderItemId);
        var addedItem = await service.AddSolution(item, solutionName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddSolution, folderItemId);        
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddSolution, addedItem.ToSummary());
        return opResult.ToString();       

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddSolution, folderItemId, $"Failed to add solution {solutionName} to parent item with ID {folderItemId}");
      }
    }

    public async Task<string> AddSolutionImport(int solutionItemId, int importLibraryId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await dbContext.GetItemDtoById(solutionItemId);
        var importLib = await dbContext.GetItemDtoById(importLibraryId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddSolutionImport, solutionItemId);
        if (importLib == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddSolutionImport, importLibraryId);
        if (item.ItemTypeId != (int)WeItemType.SolutionModel) {
          string msg = $"yea sorry, {Cx.CmdAddSolutionImport} failed itemid:{solutionItemId} is not a solution model type.";
          _logger.LogError(msg);
          var opR = McpOpResult.CreateFailure(Cx.CmdAddSolutionImport, msg);
          return JsonSerializer.Serialize(opR);
        }       
        string importNameToUse = importLib == null ? "HelpRegisterObjectNotSet" : importLib.Name;
        var addedItem = await service.AddSolutionImport(item, importNameToUse);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddSolutionImport, solutionItemId);

        var LibraryProp = addedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRegisterObject);
        if (LibraryProp != null && importLib != null) { 
          LibraryProp.Value = importLib.Id.ToString();
          addedItem = await mediator.UpdateItemProp(addedItem, LibraryProp);
        }
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddSolutionImport, addedItem.ToSummary());
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddSolutionImport, solutionItemId, $"{Cx.CmdAddSolutionImport} excepted {solutionItemId}, {importLibraryId} ");
      }
    }

    public async Task<string> AddFile(int folderItemId, string fileName, string fileContent) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await dbContext.GetItemDtoById(folderItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddFile, folderItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddFile, folderItemId);
        var addedItem = await service.AddFile(item, fileName, fileContent);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddFile, folderItemId);        
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddFile, addedItem.ToSummary());
        return opResult.ToString();       
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddFile, folderItemId, $"Failed to add file {fileName} to parent item with ID {folderItemId}");
      }
    }

  }
}
