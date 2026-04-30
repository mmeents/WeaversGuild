using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Service {
  public interface IAppGraphFileService {
    Task<ItemDto?> AddProjectRoot(string? projectName, string projectPath);
    Task<ItemDto?> AddSubFolder(ItemDto parentItem, string? subFolderName);
    Task<ItemDto?> AddSolution(ItemDto projectFolderItem, string? solutionName);
    Task<ItemDto?> AddSolutionImport(ItemDto slnItem, string? importName);
    Task<ItemDto?> AddFile(ItemDto folderItem, string? fileName);
  }


  public class AppGraphFileService : IAppGraphFileService {
    private readonly IServiceScopeFactory _scopeFactory;
    public AppGraphFileService(IServiceScopeFactory scopeFactory) { 
      _scopeFactory = scopeFactory;
    }
    private IMediator GetMediator() {
      var scope = _scopeFactory.CreateScope();
      return scope.ServiceProvider.GetRequiredService<IMediator>(); 
    }

    public async Task<ItemDto?> AddProjectRoot(string? projectName, string projectPath) {      
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery((int?)null))+1;
      var name = projectName == null ? $"Project {nextRank}" : projectName;      
      var newItem = await mediator.Send(new CreateItemCommand(name, (int)WeItemType.ProjectFolderModel, "", "{}"));
      if (newItem == null) return null;

      var rootFolderProperty = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        var defaultFolder = string.IsNullOrEmpty(projectPath) ? WeaverExt.AppProjectsPath : projectPath;
        rootFolderProperty.Value = Path.Combine(defaultFolder, newItem.Name.UrlSafe());
        await rootFolderProperty.SaveProp(newItem, mediator);        
      }
      return newItem;
    }

    public async Task<ItemDto?> AddSubFolder(ItemDto parentItem, string? subFolderName) {
      var mediator = GetMediator();      
      ItemDto item = parentItem;
      if (!item.IsValidFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(subFolderName)) nextRank = await mediator.Send(new GetNextItemRankQuery(item.Id)) + 1;
      var name = subFolderName == null ? $"Folder {nextRank}" : subFolderName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(item.Id, (int)WeRelationTypes.Contains, (int)WeItemType.RelativeFolderModel, name, "", "{}"));

      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fullPath = Path.Combine(parentFolderPath, newSubItem.Name.UrlSafe());
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);        
      }
      return newSubItem;
    }

    public async Task<ItemDto?> AddSolution(ItemDto projectFolderItem, string? solutionName) {
      var mediator = GetMediator();
      ItemDto item = projectFolderItem;
      if (!item.IsValidFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(solutionName)) nextRank = await mediator.Send(new GetNextItemRankQuery(item.Id)) + 1;
      var name = solutionName == null ? $"{item.Name.UrlSafe()}{nextRank}" : solutionName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(item.Id, (int)WeRelationTypes.Contains, (int)WeItemType.SolutionModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fileName = newSubItem.Name.UrlSafe() + ".sln";
        var fullPath = Path.Combine(parentFolderPath, fileName);
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);
      }

      var slnGuidProp = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSolutionGuid);
      if (slnGuidProp != null) {
        string? val = slnGuidProp?.Value ?? "";
        if (slnGuidProp != null && string.IsNullOrEmpty(val)) {
          slnGuidProp.Value = Guid.NewGuid().ToString("B").ToUpper();
          await slnGuidProp.SaveProp(newSubItem, mediator);          
        }
      }

      return newSubItem;
    }

    public async Task<ItemDto?> AddSolutionImport(ItemDto slnItem, string? importName) {
      var mediator = GetMediator();
      if (slnItem.ItemTypeId != (int)WeItemType.SolutionModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(importName)) nextRank = await mediator.Send(new GetNextItemRankQuery(slnItem.Id)) + 1;
      var name = importName == null ? $"Import {nextRank}" : importName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(slnItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.SolutionImportModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var projGuidProp = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItProjectGuid);
      if (projGuidProp != null) {
        string? val = projGuidProp?.Value ?? "";
        if (projGuidProp != null && string.IsNullOrEmpty(val)) {
          projGuidProp.Value = Guid.NewGuid().ToString("B").ToUpper();
          await projGuidProp.SaveProp(newSubItem, mediator);          
        }
      }

      return newSubItem;
    }

    public async Task<ItemDto?> AddFile(ItemDto folderItem, string? fileName) {
      var mediator = GetMediator();
      if (!folderItem.IsValidFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(fileName)) nextRank = await mediator.Send(new GetNextItemRankQuery(folderItem.Id)) + 1;
      var name = fileName == null ? $"File {nextRank}" : fileName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(folderItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.FileModel, name, "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = folderItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fileExt = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFileExt)?.Value ?? ".md";
        var filesName = newSubItem.Name.Contains('.') ? newSubItem.Name : newSubItem.Name.UrlSafe() + fileExt;
        var fullPath = Path.Combine(parentFolderPath, filesName);
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);
      }
      return newSubItem;
    }   

  }
}
