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
  public record AddPatDimensionCommand(int PatternId, string DimensionName, string CommaDelimOptions) : IRequest<ItemDto?>;
  public class AddPatDimensionCommandHandler : IRequestHandler<AddPatDimensionCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddPatDimensionCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AddPatDimensionCommand request, CancellationToken cancellationToken) {
      var parentItem = await _context.GetItemDtoById(request.PatternId);
      if (parentItem == null) { throw new Exception($"Parent item with id {request.PatternId} not found"); }
      if (parentItem.ItemTypeId != (int)WeItemType.PatternModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId} valid types are {WeItemType.PatternModel}");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.PatternDimensionModel, request.DimensionName, "", "{}"));

      if (newItem == null) { throw new Exception($"Failed to create new item for parent item with id {request.PatternId}"); }
      var options  = request.CommaDelimOptions.Parse(",\"");
      foreach(var option in options) {
        var newOptionItem = await _mediator.Send(
          new CreateRelatedItemCommand(newItem.Id, (int)WeRelationTypes.Contains,
            (int)WeItemType.PatternOptionModel, option.Trim(), "", "{}"));
        if (newOptionItem == null) { throw new Exception($"Failed to create new option item for parent item with id {newItem.Id}"); }
      }

      return newItem;
    }
  }
}
