using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Handlers.Pipeline;
using MediatR;
using Weavers.Core.Enums;
using Weavers.Core.Models;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Todo {
  public record SetTodoReadyCommand(int Id) : IMcpRequest, IRequest<ItemDto?>;

  public class SetTodoReadyCommandHandler : IRequestHandler<SetTodoReadyCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly ISessionItemCacheService _sessionCache;
    public SetTodoReadyCommandHandler(IMediator mediator, ISessionItemCacheService sessionCache) {
      _mediator = mediator;
      _sessionCache = sessionCache;
    }

    public async Task<ItemDto?> Handle(SetTodoReadyCommand request, CancellationToken cancellationToken) {
      var item = await _mediator.Send(new GetItemByIdQuery(request.Id), cancellationToken);
      if (item == null) {
        throw new ArgumentException($"Item Id {request.Id} not found.");
      }
      if (item.ItemTypeId != (int)WeItemType.TodoModel) {
        throw new Exception($"Item Id {request.Id} is not a Todo item.");
      }
      var isReadyProperty = item.Properties.FirstOrDefault(p => p.Name == Cx.ItConfirmedReady);
      if (isReadyProperty == null) { 
        throw new Exception($"Item Id {request.Id} does not have a property named '{Cx.ItConfirmedReady}'."); 
      } 
      if (isReadyProperty.Value.AsBoolean()) {
        throw new Exception($"Item Id {request.Id} is already marked as ready.");
      }

      var statusProperty = item.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);

      isReadyProperty.Value = "1";
      var updatedItem = await _mediator.Send(new UpdateItemPropertyCommand(isReadyProperty.Id, "1"), cancellationToken);
      if (updatedItem == null) { 
        throw new Exception($"Failed to update the property '{Cx.ItConfirmedReady}' for Item Id {request.Id}.");
      }
      _sessionCache.RemoveCacheItem(updatedItem.Id);
      return updatedItem;
    }
  }

}
