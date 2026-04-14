using MediatR;
using Weavers.Core.Entities;
using Weavers.Core.Models;
using Weavers.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Weavers.Core.Handlers.Items {
  public record CreateRelationCommand(
      int ItemId,
      int RelationTypeId,
      int RelatedItemId
  ) : IRequest<RelationDto?>;

  public class CreateRelationCommandHandler (
    FabricDbContext context,
    ILogger<CreateRelationCommandHandler> logger
  ) : IRequestHandler<CreateRelationCommand, RelationDto?> {
    private readonly FabricDbContext _context=context;
    private readonly ILogger<CreateRelationCommandHandler> _logger=logger;

    public async Task<RelationDto?> Handle(CreateRelationCommand request, CancellationToken cancellationToken) {

      try {

        var nextRank = await _context.GetItemsNextRankId(request.ItemId, cancellationToken);

        var itemRelation = new Relation {
          ItemId = request.ItemId,
          RelationTypeId = request.RelationTypeId,
          RelatedItemId = request.RelatedItemId,
          Rank = nextRank
        };

        _context.Relations.Add(itemRelation);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.GetRelationDtoById(itemRelation.Id, cancellationToken);
        return result;

      } catch (Exception ex) {
        _logger.LogError(ex, "Error creating item relation for ItemId: {ItemId}, RelationTypeId: {RelationTypeId}, RelatedItemId: {RelatedItemId}", request.ItemId, request.RelationTypeId, request.RelatedItemId);
        throw;
      }
    }


  }
}
