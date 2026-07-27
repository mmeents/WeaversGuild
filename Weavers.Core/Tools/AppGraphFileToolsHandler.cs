using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Tools {
  public interface IAppGraphFileToolsHandler {
    Task<string> AddProjectRoot(string projectRootName);
    Task<string> AddSubFolder(int itemId, string subFolderName);

    Task<string> AddGithubRepo(int folderItemId, string repoUrl);
    Task<string> DoCloneGithubRepoItem(int repoItemId);
    Task<string> DoGitRefreshStatus(int repoItemId);
    Task<string> DoCheckoutBranch(int branchItemId);


    Task<string> AddSolution(int folderItemId, string solutionName);
    Task<string> AddSolutionImport(int solutionItemId, int importLibraryId);
    Task<string> AddMdFile(int folderItemId, string fileName, string fileContent);
    Task<string> AddHtmlFile(int folderItemId, string fileName, string fileContent);
    Task<string> AddConfigFile(int folderItemId, string fileName, string fileContent);

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
          var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
          string defaultPath = settings[Cx.ApsDefaultFolder]?.Value ?? WeaverExt.AppProjectsPath;
          string filePath = Path.Combine(defaultPath, projectRootName.UrlSafe());
          var addedItem = await service.AddProjectRoot(projectRootName, filePath);
          if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddProjectRoot, 0);          
          var opResult = McpOpResult.CreateSuccess(Cx.CmdAddProjectRoot, await context.ToSummary(addedItem, false));
          return opResult.ToString();        

        } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddProjectRoot, 0, $"Failed to add project root {projectRootName}");
      }
    }

    public async Task<string> AddSubFolder(int itemId, string subFolderName) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(itemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddSubFolder, itemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddSubFolder, itemId);
        var addedItem = await service.AddSubFolder(item, subFolderName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddSubFolder, itemId);        
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddSubFolder, await context.ToSummary(addedItem, false));
        return opResult.ToString();        

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddSubFolder, itemId, $"Failed to add folder {subFolderName} to parent item with ID {itemId}");
      }
    }

    public async Task<string> AddGithubRepo(int folderItemId, string repoUrl) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(folderItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddGithubRepo, folderItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddGithubRepo, folderItemId);
        var addedItem = await service.AddGithubRepo(item, repoUrl);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddGithubRepo, folderItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddGithubRepo, await context.ToSummary(addedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddGithubRepo, folderItemId, $"Failed to add GitHub repository {repoUrl} to parent item with ID {folderItemId}");
      }
    }

    public async Task<string> DoCloneGithubRepoItem(int repoItemId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(repoItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdDoGitClone, repoItemId);
        if (item.ItemTypeId != (int)WeItemType.GithubRepoModel) return _logger.DefaultInvalidParentMessage(Cx.CmdDoGitClone, repoItemId);
        var addedItem = await service.DoCloneGithubRepoItem(item);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdDoGitClone, repoItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdDoGitClone, await context.ToSummary(addedItem, false));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdDoGitClone, repoItemId, $"Failed to clone GitHub repository item with ID {repoItemId}");
      }
    }

    public async Task<string> DoGitRefreshStatus(int repoItemId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(repoItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdDoGitRefreshStatus, repoItemId);
        if (item.ItemTypeId != (int)WeItemType.GithubRepoModel) return _logger.DefaultInvalidParentMessage(Cx.CmdDoGitRefreshStatus, repoItemId);
        var updatedItem = await service.DoGitRefreshStatus(item);
        if (updatedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdDoGitRefreshStatus, repoItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdDoGitRefreshStatus, await context.ToSummary(updatedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdDoGitRefreshStatus, repoItemId, $"Failed to refresh Git status for repository item with ID {repoItemId}");
      }
    }

    public async Task<string> DoCheckoutBranch(int branchItemId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(branchItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdDoGitCheckout, branchItemId);
        if (item.ItemTypeId != (int)WeItemType.GithubRepoBranchModel) return _logger.DefaultInvalidParentMessage(Cx.CmdDoGitCheckout, branchItemId);
        var updatedItem = await service.DoCheckoutBranch(item);
        if (updatedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdDoGitCheckout, branchItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdDoGitCheckout, await context.ToSummary(updatedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdDoGitCheckout, branchItemId, $"Failed to checkout branch item with ID {branchItemId}");
      }
    }

    public async Task<string> AddSolution(int folderItemId, string solutionName) {
      try {

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(folderItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddSolution, folderItemId);       
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddSolution, folderItemId);
        var addedItem = await service.AddSolution(item, solutionName);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddSolution, folderItemId);        
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddSolution, await context.ToSummary(addedItem, false));
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
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(solutionItemId);
        var importLib = await context.GetItemDtoById(importLibraryId);
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
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddSolutionImport, await context.ToSummary(addedItem, false));
        return opResult.ToString();

      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddSolutionImport, solutionItemId, $"{Cx.CmdAddSolutionImport} excepted {solutionItemId}, {importLibraryId} ");
      }
    }

    public async Task<string> AddMdFile(int folderItemId, string fileName, string fileContent) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(folderItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddMdFile, folderItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddMdFile, folderItemId);
        var addedItem = await service.AddMdFile(item, fileName, fileContent);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddMdFile, folderItemId);        
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddMdFile, await context.ToSummary(addedItem, true));
        return opResult.ToString();       
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddMdFile, folderItemId, $"Failed to add file {fileName} to parent item with ID {folderItemId}");
      }
    }

    public async Task<string> AddHtmlFile(int folderItemId, string fileName, string fileContent) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(folderItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddHtmlFile, folderItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddHtmlFile, folderItemId);
        var addedItem = await service.AddHtmlFile(item, fileName, fileContent);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddHtmlFile, folderItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddHtmlFile, await context.ToSummary(addedItem, true));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddHtmlFile, folderItemId, $"Failed to add file {fileName} to parent item with ID {folderItemId}");
      }
    }

    public async Task<string> AddConfigFile(int folderItemId, string fileName, string fileContent) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
        var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var item = await context.GetItemDtoById(folderItemId);
        if (item == null) return _logger.DefaultFailToFindMessage(Cx.CmdAddConfigFile, folderItemId);
        if (!item.ItemTypeId.IsFolderType()) return _logger.DefaultInvalidParentMessage(Cx.CmdAddConfigFile, folderItemId);
        var addedItem = await service.AddConfigFile(item, fileName, fileContent);
        if (addedItem == null) return _logger.DefaultAddEmptyMessage(Cx.CmdAddConfigFile, folderItemId);
        var opResult = McpOpResult.CreateSuccess(Cx.CmdAddConfigFile, await context.ToSummary(addedItem, true));
        return opResult.ToString();
      } catch (Exception ex) {
        return ex.ToOpResult(_logger, Cx.CmdAddConfigFile, folderItemId, $"Failed to add file {fileName} to parent item with ID {folderItemId}");
      }
    }




  }
}
