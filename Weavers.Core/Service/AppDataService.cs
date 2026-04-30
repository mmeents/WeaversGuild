using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Handlers.RelationTypes;
using Weavers.Core.Handlers.ItemTypes;
using Weavers.Core.Handlers.DepItems;
using System.Runtime.CompilerServices;


namespace Weavers.Core.Service {

  public interface IAppDataService {

    Task<List<ItemDto>> GetRootProjectsAsync();
    Task<ItemDto?> GetItemById(int itemId);
    Task<ItemDto?> CreateItemAsync(ItemDto itemDto);
    Task<ItemDto?> CreateRelatedItemAsync(int parentItemId, int relationTypeId, int ItemTypeId, string itemName, string itemDescription, string itemData);
    Task<bool> DeleteItemAsync(int itemId);
    Task<ItemDto?> UpdateItemAsync(ItemDto request);
    Task<RelationDto?> UpdateRelationAsync(RelationDto relation);
    Task<List<RelationTypeDto>> GetRelationTypesAsync();
    Task<List<ItemTypeDto>> GetAllItemTypesAsync();
    Task<ItemPropertyDto?> AddUpdateItemPropertyAsync(ItemPropertyDto? itemProperty);
    Task<List<ItemLookup>> GetItemsByItemType(int itemTypeId);
    Task<int> GetNextItemRank(int? itemId = null);
    Task UpdateItemPropertyPathRecursive(int itemId, string oldPath, string newPath);
    Task UpdateItemPropertyNamespaceRecursive(int itemId, string oldNamespace, string newNamespace);
    Task AddRemoveClassToLibDi(int ClassItemId, bool add, bool GenerateInterface);
    Task AddRemoveDbContextToLibDi(int DiItemId, bool add);
    //Task AddRemoveEntityToDbContext(int entityItemId, bool add);
    Task ProcessPropertyUpdate(ItemDto entityItem, ItemDto propertyItem);

  }
  public class AppDataService : IAppDataService {
    private readonly IServiceScopeFactory _scopeFactory;
    public AppDataService(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
    }
    private IMediator GetMediator() {
      var scope = _scopeFactory.CreateScope();
      return scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public async Task<List<ItemDto>> GetRootProjectsAsync() {
      var mediator = GetMediator();
      var query = new GetRootProjectsQuery();
      var result = await mediator.Send(query);
      return result;
    }

    public async Task<ItemDto?> GetItemById(int itemId) {
      var mediator = GetMediator();
      var query = new GetItemByIdQuery(itemId);
      var result = await mediator.Send(query);
      return result;
    }

    public async Task<ItemDto?> CreateItemAsync(ItemDto itemDto) {
      var mediator = GetMediator();      
      var command = new CreateItemCommand(itemDto.Name, itemDto.ItemTypeId, itemDto.Description, itemDto.Data);
      var result = await mediator.Send(command);
      return result;
    }

    public async Task<ItemDto?> CreateRelatedItemAsync(int parentItemId, int relationTypeId, int ItemTypeId, string itemName, string itemDescription, string itemData) {
      var mediator = GetMediator();
      var command = new CreateRelatedItemCommand(parentItemId, relationTypeId, ItemTypeId, itemName, itemDescription, itemData);
      var result = await mediator.Send(command);
      return result;
    }

    public async Task<bool> DeleteItemAsync(int itemId) {
      var mediator = GetMediator();
      var command = new DeleteItemCommand(itemId);
      return await mediator.Send(command);
    }
    public async Task<ItemDto?> UpdateItemAsync(ItemDto request) {
      var mediator = GetMediator();
      var command = new UpdateItemCommand(request.Id, request.ItemTypeId, request.Name, request.Description, request.Data, request.IsActive);
      var result = await mediator.Send(command);
      return result;
    }

    public async Task<RelationDto?> UpdateRelationAsync(RelationDto relation) { 
      if (relation == null) {
        throw new ArgumentNullException(nameof(relation));
      }
      if (relation.RelatedItemId == null) {
        throw new ArgumentException("RelatedItemId cannot be null for updating a relation.");
      }

      var mediator = GetMediator();
      var command = new UpdateRelationCommand(relation.Id, relation.ItemId, relation.RelationTypeId, relation.RelatedItemId ?? 0, relation.Rank);
      var result = await mediator.Send(command);
      return result;  
    } 

    public async Task<List<RelationTypeDto>> GetRelationTypesAsync() {
      var mediator = GetMediator();
      var query = new GetAllRelationTypesQuery();
      var result = await mediator.Send(query);
      return result;
    }

    public async Task<List<ItemTypeDto>> GetAllItemTypesAsync() {
      var mediator = GetMediator();
      var query = new GetAllItemTypesQuery();
      var result = await mediator.Send(query);
      return result;
    }

    public async Task<ItemPropertyDto?> AddUpdateItemPropertyAsync(ItemPropertyDto? itemProperty) {
      if (itemProperty == null) {
        throw new ArgumentNullException(nameof(itemProperty));
      }
      var mediator = GetMediator();
      var command = new AddUpdateItemPropertyCommand(
        itemProperty.Id,
        itemProperty.ItemId,
        itemProperty.Name,
        itemProperty.Value,
        itemProperty.ValueDataTypeId,
        itemProperty.EditorTypeId,
        itemProperty.ReferenceItemTypeId
      );
      var result = await mediator.Send(command);
      return result;
    }

    public async Task<List<ItemLookup>> GetItemsByItemType(int itemTypeId) {
      var mediator = GetMediator();
      var query = new GetItemsByItemTypeQuery(itemTypeId);
      var result = await mediator.Send(query);
      return result;
    }

    public async Task<int> GetNextItemRank(int? itemId = null) { 
        var mediator = GetMediator();
        var query = new GetNextItemRankQuery(itemId);
        var result = await mediator.Send(query);
        return result;
    }

    public async Task UpdateItemPropertyPathRecursive(int itemId, string oldPath, string newPath) { 
      var mediator = GetMediator();
      var command = new UpdateItemPropertyPathRecursiveCommand(itemId, oldPath, newPath);
      await mediator.Send(command);   
    }

    public async Task UpdateItemPropertyNamespaceRecursive(int itemId, string oldNamespace, string newNamespace) {
      var mediator = GetMediator();
      var command = new UpdateItemPropertyNamespaceRecursiveCommand(itemId, oldNamespace, newNamespace);
      await mediator.Send(command);
    }

    public async Task AddRemoveClassToLibDi(int ClassItemId, bool add, bool generateInterface) {
      var mediator = GetMediator();
      var command = new AddRemoveClassToLibDiCommand(ClassItemId, add, generateInterface);
      await mediator.Send(command);
    }

    public async Task AddRemoveDbContextToLibDi(int DiItemId, bool add) {
      var mediator = GetMediator();
      var command = new AddRemoveDbContextToLibDiCommand(DiItemId, add);
      await mediator.Send(command);
    }

    //public async Task AddRemoveEntityToDbContext(int entityItemId, bool add) {
    //  var mediator = GetMediator();
    //  var command = new AddRemoveEntityToDbContextCommand(entityItemId, add);
    //  await mediator.Send(command);
    //}

    public async Task ProcessPropertyUpdate(ItemDto entityItem, ItemDto propertyItem) {
      var mediator = GetMediator();
      var command = new ProcessPropertyUpdateCommand(entityItem, propertyItem);
      await mediator.Send(command);
    }



  }
}
