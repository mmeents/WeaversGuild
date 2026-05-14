using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Weavers.Core.Handlers.ItemSummaries;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.ItemTypes;
using Weavers.Core.Extensions;
using MediatR;
using Weavers.Core.Models;
using System.Text.Json;
using Weavers.Core.Enums;

namespace Weavers.Core.Tools {

  public interface ISummaryToolsHandler {

    Task<string> ListProjects(); 
    Task<string> Search(string searchTerms, int byType = 0, int maxResults = 10);
    Task<string> GetSummaryDtoById(int id, bool nodesUp = false, bool includeProps = true);
    Task<string> GetSummaryByIdRecursive(int id);
    Task<string> GetTypeDetails(int itemTypeId = 0);
    Task<string> UpdateItemName(int id, string name);
    Task<string> UpdateItemProperty(int itemPropertyId, string propertyValue);
  }

  public class SummaryToolsHandler : ISummaryToolsHandler {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SummaryToolsHandler> _logger;

    public SummaryToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<SummaryToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    public async Task<string> ListProjects() {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();        
        var ItemDtoList = await mediator.Send(new GetRootProjectsQuery());
        List<ItemSummaryDto> result = new List<ItemSummaryDto>();
        foreach (var item in ItemDtoList) {
          result.Add(item.ToSummary()); 
        }
        var opResult = McpOpResult.CreateSuccess("ListProjects", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error listing projects");
        var opResult = McpOpResult.CreateFailure("ListProjects", "Failed to list projects", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> Search(string searchTerms, int byType = 0, int maxResults = 10) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new SearchSummaryQuery(searchTerms, byType, maxResults);
        var result = await mediator.Send(query);
        if (result == null) {
          var notFoundResult = McpOpResult.CreateFailure("Search", $"No summary found for query {searchTerms}");
          return JsonSerializer.Serialize(notFoundResult);
        }
        var opResult = McpOpResult.CreateSuccess("Search", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, $"Error searching for query {searchTerms}", searchTerms);
        var opResult = McpOpResult.CreateFailure("Search", $"Failed searching for query {searchTerms}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> GetSummaryDtoById(int id, bool nodesUp = false, bool includeProps = true) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetSummaryByIdQuery(id, nodesUp, includeProps);
        var result = await mediator.Send(query);
        if (result == null) {
          var notFoundResult = McpOpResult.CreateFailure("GetSummaryById", $"No summary found for id {id}");
          return JsonSerializer.Serialize(notFoundResult);
        }
        var opResult = McpOpResult.CreateSuccess("GetSummaryById", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving summary by id");
        var opResult = McpOpResult.CreateFailure("GetSummaryById", "Failed to retrieve summary by id", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> GetSummaryByIdRecursive(int id) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetSummaryByIdQuery(id, true, true);
        var result = await mediator.Send(query);
        if (result == null) {
          var notFoundResult = McpOpResult.CreateFailure("GetClassSummaryById", $"No summary found for id {id}");
          return JsonSerializer.Serialize(notFoundResult);
        }
        if (result.TypeId == (int)WeItemType.ClassModel || result.TypeId == (int)WeItemType.EntityClassModel) {
          result = await mediator.Send(new LoadSummaryRecursivlyQuery(result.Id));
        }
        var opResult = McpOpResult.CreateSuccess("GetSummaryByIdRecursive", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving summary by id recursively");
        var opResult = McpOpResult.CreateFailure("GetSummaryByIdRecursive", "Failed to retrieve summary by id recursively", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> GetTypeDetails(int itemTypeId = 0) { 
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetTypeDetailsQuery(itemTypeId);
        var result = await mediator.Send(query);  
        if (result == null) {
          var notFoundResult = McpOpResult.CreateFailure("GetTypeDetails", $"No type details found for item type id {itemTypeId}");
          return JsonSerializer.Serialize(notFoundResult);
        }
        var opResult = McpOpResult.CreateSuccess("GetTypeDetails", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, $"Error retrieving type details for item type id {itemTypeId}", itemTypeId);
        var opResult = McpOpResult.CreateFailure("GetTypeDetails", $"Failed to retrieve type details for item type id {itemTypeId}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> UpdateItemName(int id, string newName) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var item = await mediator.Send(new GetItemByIdQuery(id));
        if (item == null) {
          var notFoundResult = McpOpResult.CreateFailure("UpdateSummary", $"No item found for id {id}");
          return JsonSerializer.Serialize(notFoundResult);
        }
        item.Name = newName;
        var updatedItem = await mediator.Send(item.ToUpdateCmd());
        if (updatedItem == null) {
          var updateFailedResult = McpOpResult.CreateFailure("UpdateSummary", $"Failed to update item with id {id}");
          return JsonSerializer.Serialize(updateFailedResult);
        }
        var opResult = McpOpResult.CreateSuccess("UpdateSummary", updatedItem.ToSummary());
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error updating summary");
        var opResult = McpOpResult.CreateFailure("UpdateSummary", "Failed to update summary", ex);
        return JsonSerializer.Serialize(opResult);
      }

    }

    public async Task<string> UpdateItemProperty(int itemPropertyId, string propertyValue) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new UpdateItemPropertyCommand(itemPropertyId, propertyValue);
        var result = await mediator.Send(command);
        if (result == null) {
          var notFoundResult = McpOpResult.CreateFailure("UpdateItemProperty", $"No item property found for id {itemPropertyId}");
          return JsonSerializer.Serialize(notFoundResult);
        }
        var opResult = McpOpResult.CreateSuccess("UpdateItemProperty", result.ToSummary());
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error updating item property");
        var opResult = McpOpResult.CreateFailure("UpdateItemProperty", "Failed to update item property", ex);
        return JsonSerializer.Serialize(opResult);
      }




    }
  }
}
