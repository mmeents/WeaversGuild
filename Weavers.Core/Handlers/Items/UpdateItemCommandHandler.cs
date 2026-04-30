using MediatR;
using Microsoft.Extensions.Logging;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.DepItems;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record UpdateItemCommand(
     int Id,
     int ItemTypeId,
     string Name,
     string Description,
     string Data,
     bool IsActive
   ) : IRequest<ItemDto?>;


  public class UpdateItemCommandHandler(
    FabricDbContext context, 
    IMediator mediator,
    ILogger<UpdateItemCommandHandler> logger
  ) : IRequestHandler<UpdateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context = context;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<UpdateItemCommandHandler> _logger = logger;

    public async Task<ItemDto?> Handle(UpdateItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.Items.FindAsync(request.Id);
      var nameWas = item?.Name;

      if (item == null) {
        var error = new KeyNotFoundException($"Item with id {request.Id} not found");
        _logger.LogError(error, "Failed to update item with id {ItemId}", request.Id);
        throw error;
      }

      item.Name = request.Name;
      item.Description = request.Description;
      item.Data = request.Data;
      item.ItemTypeId = request.ItemTypeId;
      item.IsActive = request.IsActive;

      await _context.SaveChangesAsync(cancellationToken);

      var itemDto = await _context.GetItemDtoById(item.Id, cancellationToken);

      if (itemDto.ItemTypeId == (int)WeItemType.EntityClassModel && nameWas != itemDto.Name) {
        // EntityClassModel has dependent EntityConfigurationModel and DbContextEntityImportModel names that also need to change.
        var libraryItem = await _mediator.Send(new GetLibDiModelCommand(itemDto.Id), cancellationToken);
        if (libraryItem != null) { 
          var dbContextItem = await _context.ResolveDbContextFromLib(libraryItem, cancellationToken);
          if (dbContextItem != null) {
            var dbContextEntityItem = await _context.FindRelatedDbContextEntity(dbContextItem, itemDto, cancellationToken);
            if (dbContextEntityItem != null) {
             var entity = await _context.Items.FindAsync(dbContextEntityItem.Id);
              if (entity != null) {
                entity.Name = item.Name;
                _context.Items.Update(entity);
                await _context.SaveChangesAsync(cancellationToken);
              }
            }
          }
        }

        var entityConfigItemRef = itemDto.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityConfigurationModel);
        if (entityConfigItemRef != null) {
          var itemId = entityConfigItemRef?.RelatedItemId ?? 0;
          if (itemId != 0) {
            var entityConfig = await _context.Items.FindAsync(itemId);
            if (entityConfig != null) {
              entityConfig.Name = item.Name + "Config";
              _context.Items.Update(entityConfig);
              await _context.SaveChangesAsync(cancellationToken); 
            }
          }
        }
      }     

      return itemDto;

    }
  }
}
