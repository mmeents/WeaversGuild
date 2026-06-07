using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Service {
  public interface IAppGraphOrgService {
    Task<ItemDto?> AddOrgDesk(ItemDto OrgChart, string? deskName);
    Task<ItemDto?> AddDeskTodo(ItemDto OrgDesk, string? todoName);

    Task<ItemDto?> AddDigitalOperator(ItemDto parentItem, string? operatorName);
    Task<ItemDto?> AddOrgFolder(ItemDto parentItem, string? subFolderName);
    Task<ItemDto?> AddOrgFile(ItemDto folderItem, string? fileName, string? fileContent);
  }
  public class AppGraphOrgService : IAppGraphOrgService {
    private readonly IServiceScopeFactory _scopeFactory;
    public AppGraphOrgService(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
    }
    private IMediator GetMediator() {
      var scope = _scopeFactory.CreateScope();
      return scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public async Task<ItemDto?> AddOrgDesk(ItemDto OrgChart, string? deskName) {
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery(OrgChart.Id)) + 1;
      var name = deskName == null ? $"Desk {nextRank}" : deskName;
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(OrgChart.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.DeskModel, name, "", "{}"));
      if (newItem != null) {
        var itsFilePathProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (itsFilePathProp != null && string.IsNullOrEmpty(itsFilePathProp.Value)) {
          string parentFolderPath = OrgChart.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
          var fullPath = Path.Combine(parentFolderPath, newItem.Name.UrlSafe()+".json");
          itsFilePathProp.Value = fullPath;
          await itsFilePathProp.SaveProp(newItem, mediator);
        }
      }
      return newItem;
    }

    public async Task<ItemDto?> AddDeskTodo(ItemDto OrgDesk, string? todoName) {
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery(OrgDesk.Id)) + 1;
      var name = todoName == null ? $"Todo {nextRank}" : todoName;
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(OrgDesk.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.TodoModel, name, "", "{}"));
      return newItem;
    }

    public async Task<ItemDto?> AddDigitalOperator(ItemDto parentItem, string? operatorName) {
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery((int?)null)) + 1;
      var name = operatorName == null ? $"Operator {nextRank}" : operatorName;
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains, 
          (int)WeItemType.DigitalOperatorModel, name, "", "{}"));
      if (newItem != null) {
        var itsFilePathProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (itsFilePathProp != null && string.IsNullOrEmpty(itsFilePathProp.Value)) {
          string parentFolderPath = parentItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
          var fullPath = Path.Combine(parentFolderPath, newItem.Name.UrlSafe() + ".json");
          itsFilePathProp.Value = fullPath;
          await itsFilePathProp.SaveProp(newItem, mediator);
        }
      }
      return newItem;
    }

    public async Task<ItemDto?> AddOrgFolder(ItemDto parentItem, string? subFolderName) {
      var mediator = GetMediator();
      ItemDto item = parentItem;
      if (!item.IsValidFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(subFolderName)) nextRank = await mediator.Send(new GetNextItemRankQuery(item.Id)) + 1;
      var name = subFolderName == null ? $"Folder {nextRank}" : subFolderName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.OrgDocFolderModel, name, "", "{}"));

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

    public async Task<ItemDto?> AddOrgFile(ItemDto folderItem, string? fileName, string? fileContent) {
      var mediator = GetMediator();
      if (!folderItem.IsValidFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(fileName)) nextRank = await mediator.Send(new GetNextItemRankQuery(folderItem.Id)) + 1;
      var name = fileName == null ? $"File {nextRank}" : fileName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(folderItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.OrgDocModel, name, fileContent ?? "", "{}"));
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
