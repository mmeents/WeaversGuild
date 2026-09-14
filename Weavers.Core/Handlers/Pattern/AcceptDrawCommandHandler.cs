using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Pattern {

  public record AcceptDrawCommand(int DrawId, int RefItemId) : IRequest<ItemDto?>;
  public class AcceptDrawCommandHandler : IRequestHandler<AcceptDrawCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AcceptDrawCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AcceptDrawCommand request, CancellationToken cancellationToken) {

      var drawItem = await _context.GetItemDtoById(request.DrawId);
      if (drawItem == null) {
        throw new ArgumentException($"Draw item with ID {request.DrawId} not found.");
      }
      if (drawItem.ItemTypeId != (int)WeItemType.PatternDrawModel) {
        throw new Exception($"Invalid item type {(WeItemType)drawItem.ItemTypeId} valid types are {WeItemType.PatternDrawModel}");
      }

      var refItem = await _context.GetItemDtoById(request.RefItemId);
      if (refItem != null) {
        var refItemProp = drawItem.Properties.FirstOrDefault(p => p.Name == Cx.ItProduced);
        if (refItemProp != null) {
          refItemProp.Value = refItem.Id.ToString();
          refItemProp.ReferenceItemTypeId = refItem.ItemTypeId;
          await refItemProp.SaveProp(drawItem, _mediator);
        }
      }

      var drawStatusProperty = drawItem.Properties.FirstOrDefault(p => p.Name == Cx.ItDrawStatus);
      if (drawStatusProperty != null) {
        drawStatusProperty.Value = ((int)WeItemType.DrawAccepted).ToString();
        await drawStatusProperty.SaveProp(drawItem, _mediator);
      }
      
      return drawItem;
    }
  }
}
