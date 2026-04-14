using MediatR;
using Weavers.Core.Models;
using Weavers.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Weavers.Core.Handlers.Items {
  public record UpdateRelationCommand(
      int Id,
      int ItemId,
      int RelationTypeId,
      int RelatedItemId,
      int? Rank = null
  ) : IRequest<RelationDto?>;

  public class UpdateRelationCommandHandler(
    FabricDbContext context,
    ILogger<UpdateRelationCommandHandler> logger
  ) : IRequestHandler<UpdateRelationCommand, RelationDto?> {
    private readonly FabricDbContext _context = context;
    private readonly ILogger<UpdateRelationCommandHandler> _logger = logger;

    public async Task<RelationDto?> Handle(UpdateRelationCommand request, CancellationToken cancellationToken) {
      var itemRelation = await _context.Relations.FindAsync(request.Id);
      if (itemRelation == null) {
        var error = new KeyNotFoundException($"Item relation with ID {request.Id} not found");
        _logger.LogError(error, "Failed to update item relation with ID {Id}", request.Id);
        throw error;
      }
      try { 

        itemRelation.ItemId = request.ItemId;
        itemRelation.RelationTypeId = request.RelationTypeId;
        itemRelation.RelatedItemId = request.RelatedItemId;
        itemRelation.Rank = request.Rank;
        await _context.SaveChangesAsync(cancellationToken);
            
        var result = await _context.GetRelationDtoById(itemRelation.Id, cancellationToken);
        return result;

      } catch (Exception ex) {
        _logger.LogError(ex, "An error occurred while updating item relation with ID {Id}", request.Id);
        throw;
      }
    }
  }
}
