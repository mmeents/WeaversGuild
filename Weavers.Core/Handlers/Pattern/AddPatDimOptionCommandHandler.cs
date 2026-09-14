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
  public record AddPatDimOptionCommand(int PatternDimensionId, string Option) : IMcpRequest, IRequest<ItemDto?>;
  public class AddPatDimOptionCommandHandler : IRequestHandler<AddPatDimOptionCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddPatDimOptionCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(AddPatDimOptionCommand request, CancellationToken cancellationToken) {
      var parentItem = await _context.GetItemDtoById(request.PatternDimensionId);
      if (parentItem == null) { throw new Exception($"Parent item with id {request.PatternDimensionId} not found"); }
      if (parentItem.ItemTypeId != (int)WeItemType.PatternDimensionModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId} valid types are {WeItemType.PatternDimensionModel}");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.PatternOptionModel, request.Option, "", "{}"));      

      return newItem;
    }
  }
}
