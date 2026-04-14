using Weavers.Core.Entities;
using Weavers.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Weavers.Core.Handlers.Items {
  public record CreateItemCommand(
   string Name,
   int ItemTypeId,
   string Description,
   string Data
 ) : IRequest<ItemDto?>;


  public class CreateItemCommandHandler(FabricDbContext context, ILogger<CreateItemCommandHandler> logger) : IRequestHandler<CreateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context=context;
    private readonly ILogger<CreateItemCommandHandler> _logger=logger;
    public async Task<ItemDto?> Handle(CreateItemCommand request, CancellationToken cancellationToken) {

      var itemType = await _context.ItemTypes.FindAsync( request.ItemTypeId, cancellationToken);

      if (itemType == null) {
        var exception = new Exception($"ItemType with id {request.ItemTypeId} not found");
        _logger.LogError(exception, "ItemType with id {ItemTypeId} not found", request.ItemTypeId);
        throw exception;
      }

      var item = new Item {
        Name = request.Name,
        ItemTypeId = itemType.Id,
        Description = request.Description,
        Data = request.Data,
        IsActive = true
      };

      
      using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
      try {
        _context.Items.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        await _context.SyncDefaultsByModelIdAsync(item.Id, item.ItemTypeId, cancellationToken);        
        await transaction.CommitAsync(cancellationToken);
      } catch (Exception ex) {
        await transaction.RollbackAsync(cancellationToken);
        _logger.LogError(ex, "Error creating item of type {ItemTypeId}, tx rolled back.", request.ItemTypeId);
        throw;
      }

      ItemDto? result = null;
      try { 
        result = await _context.GetItemDtoById(item.Id, cancellationToken);
        return result;
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving created item with id {ItemId}", item.Id);
        throw new Exception($"Error retrieving created item with id {item.Id}", ex);
      }      
    }

   
  }
}
