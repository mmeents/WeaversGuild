using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Weavers.Core.Tools {
  public interface IStorytimeToolsHandler {    
    Task<string> GetItemById(int id);
    Task<string> GetSubgraph(int itemId, int depth = 2);
    Task<string> CreateRelatedItem(int parentItemId, int relationTypeId, int itemTypeId, string name, string description, string data);
    Task<string> CreateItem(string name, int itemTypeId, string description, string data);    
    Task<string> UpdateItem(int id, string name, int itemTypeId, string description, string data);
    Task<string> GetRelationById(int id);
    Task<string> CreateRelation(int fromItemId, int toItemId, int relationTypeId);
    Task<string> UpdateRelation(int relationId, int fromItemId, int toItemId, int relationTypeId, int rank);    
  }


  public class StorytimeToolsHandler : IStorytimeToolsHandler {
    private IServiceScopeFactory _serviceScopeFactory;
    private ILogger<StorytimeToolsHandler> _logger;

    public StorytimeToolsHandler(IServiceScopeFactory serviceScopeFactory, ILogger<StorytimeToolsHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    public async Task<string> GetItemById(int id) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetItemByIdQuery(id, true);
        var result = await mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess("get-item-by-id", "Successfully retrieved item", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving item by id");
        var opResult = McpOpResult.CreateFailure("get-item-by-id", "Failed to retrieve item by id", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> CreateRelatedItem(int parentItemId, int relationTypeId, int itemTypeId, string name, string description, string data) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new CreateRelatedItemCommand(parentItemId, relationTypeId, itemTypeId, name, description, data);
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess("create-related-item", "Successfully created related item", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error creating related item");
        var opResult = McpOpResult.CreateFailure("create-related-item", "Failed to create related item", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> CreateItem(string name, int itemTypeId, string description, string data) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new CreateItemCommand(name, itemTypeId, description, data);
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess("create-item", "Successfully created", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error creating item");
        var opResult = McpOpResult.CreateFailure("create-item", "Failed to create item", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> UpdateItem(int id, string name, int itemTypeId, string description, string data) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new UpdateItemCommand(id, itemTypeId, name, description, data, true);
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess("update-item", "Successfully updated", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error updating item");
        var opResult = McpOpResult.CreateFailure("update-item", "Failed to update item", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> GetRelationById(int id) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetItemRelationsQuery(id, null, null, null);
        var result = await mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess("get-relation-by-id", "Successfully retrieved relation", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving relation by id");
        var opResult = McpOpResult.CreateFailure("get-relation-by-id", "Failed to retrieve relation by id", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> CreateRelation(int fromItemId, int toItemId, int relationTypeId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new CreateItemRelationCommand(fromItemId, relationTypeId, toItemId);
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess("create-relation", "Successfully created relation", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error creating item relation");
        var opResult = McpOpResult.CreateFailure("create-relation", "Failed to create item relation", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> UpdateRelation(int relationId, int fromItemId, int toItemId, int relationTypeId, int rank) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new UpdateItemRelationCommand(relationId, fromItemId, relationTypeId, toItemId, rank);
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess("update-relation", "Successfully updated relation", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error updating item relation");
        var opResult = McpOpResult.CreateFailure("update-relation", "Failed to update item relation", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> GetSubgraph(int itemId, int depth) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetSubgraphQuery(itemId, depth);
        var result = await mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess("get-subgraph", "Successfully retrieved subgraph", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving subgraph");
        var opResult = McpOpResult.CreateFailure("get-subgraph", "Failed to retrieve subgraph", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

  }
}
