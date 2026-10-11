using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Items {
  public record DuplicateItemCommand(int ItemId) : IMcpRequest, IRequest<ItemDto?>;
  public class DuplicateItemCommandHandler : IRequestHandler<DuplicateItemCommand, ItemDto?> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public DuplicateItemCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<ItemDto?> Handle(DuplicateItemCommand request, CancellationToken cancellationToken) {

      var item = await _context.GetItemDtoById(request.ItemId, cancellationToken);
      if (item == null) { 
        throw new Exception($"Item with ID {request.ItemId} not found.");
      }

      if (item.ItemTypeId.CanDuplicate() == false) {
        throw new Exception($"Item with ID {request.ItemId} cannot be duplicated.");
      }

      var parentItemId = item.GetParentId();
      var parent = await _context.GetItemDtoById(parentItemId, cancellationToken);
      if (parent == null) { 
        throw new Exception($"Parent item with ID {parentItemId} not found.");
      }
      var ir = item.IncomingRelations.FirstOrDefault(r => r.RelationTypeId == (int)WeRelationTypes.Contains && r.ItemId == parentItemId);
      
      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItemId,
          ir?.RelationTypeId ?? (int)WeRelationTypes.Contains, 
          item.ItemTypeId,       
          item.Name, 
          item.Description, 
          item.Data));

      if (newItem != null) { 
        foreach(var prop in item.Properties) {
          var itemProp = newItem.Properties.FirstOrDefault(p => p.Name == prop.Name);
          await _mediator.Send(new AddUpdateItemPropertyCommand(
              itemProp?.Id ?? 0, newItem.Id, prop.Name, prop.Value, prop.ValueDataTypeId, prop.EditorTypeId, prop.ReferenceItemTypeId, prop.Rank));

        }
      }

      if (item.ItemTypeId == (int)WeItemType.ComfyOpTodoModel) { 
        var itemPropIds = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.ComfyOpParamModel).Select(r => r.RelatedItemId).ToList();               
        List<int> ids = itemPropIds.Where(i => i.HasValue).Select(i => i!.Value).ToList();
        var items = await _mediator.Send(new GetItemsByIdsQuery(ids), cancellationToken);
        foreach (var itemPropId in itemPropIds) {
          int id = itemPropId ?? 0;
          if (id != 0) {
            var itemProp = items.FirstOrDefault(i => i.Id == id);

            if (itemProp != null) {
              var newItemProp = await _mediator.Send(
                new CreateRelatedItemCommand(newItem!.Id,
                  (int)WeRelationTypes.Contains,
                  itemProp.ItemTypeId,
                  itemProp.Name,
                  itemProp.Description,
                  itemProp.Data));
              if (newItemProp != null) {
                foreach (var prop in itemProp.Properties) {
                  var itemProp2 = newItemProp.Properties.FirstOrDefault(p => p.Name == prop.Name);
                  await _mediator.Send(new AddUpdateItemPropertyCommand(
                      itemProp2?.Id ?? 0, newItemProp.Id, prop.Name, prop.Value, prop.ValueDataTypeId, prop.EditorTypeId, prop.ReferenceItemTypeId, prop.Rank));
                }
              }
            }

          }
        }
      }

      return await _context.GetItemDtoById(newItem!.Id, cancellationToken);

    }
  }
}
