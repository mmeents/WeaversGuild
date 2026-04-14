using Weavers.Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Weavers.Core.Extensions;

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
    ILogger<UpdateItemCommandHandler> logger
  ) : IRequestHandler<UpdateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context = context;
    private readonly ILogger<UpdateItemCommandHandler> _logger = logger;

    public async Task<ItemDto?> Handle(UpdateItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.Items.FindAsync(request.Id);

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

      return itemDto;

    }
  }
}
