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
  public record RejectDrawCommand(int DrawId, int TodoId) : IRequest<ItemDto?>;
  public class RejectDrawCommandHandler : IRequestHandler<RejectDrawCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public RejectDrawCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(RejectDrawCommand request, CancellationToken cancellationToken) {

      var drawItem = await _context.GetItemDtoById(request.DrawId);
      if (drawItem == null) {
        throw new ArgumentException($"Draw item with ID {request.DrawId} not found.");
      }
      if (drawItem.ItemTypeId != (int)WeItemType.PatternDrawModel) {
        throw new Exception($"Invalid item type {(WeItemType)drawItem.ItemTypeId} valid types are {WeItemType.PatternDrawModel}");
      }

      var drawStatusProperty = drawItem.Properties.FirstOrDefault(p => p.Name == Cx.ItDrawStatus);
      if (drawStatusProperty != null) {
        drawStatusProperty.Value = ((int)WeItemType.DrawRejected).ToString();
        await drawStatusProperty.SaveProp(drawItem, _mediator);
      }

      var patternId = drawItem.GetParentId();
      var result = await _mediator.Send(new GetNextDrawCommand(patternId, request.TodoId));
      if (result == null) {
        throw new Exception($"No next draw found for pattern ID {patternId} and todo ID {request.TodoId}");
      }

      var refItemProp = drawItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReplacedBy);
      if (refItemProp != null) {
        refItemProp.Value = result.Id.ToString();        
        await refItemProp.SaveProp(drawItem, _mediator);
      }

      return result;
    }
  }
}
