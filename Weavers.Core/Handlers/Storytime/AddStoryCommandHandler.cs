using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Storytime {
  public record AddStoryCommand(int realmId, string name, string description, int povTypeId, int sceneCount, int todoId) : IMcpRequest, IRequest<ItemDto?>;

  public class AddStoryCommandHandler : IRequestHandler<AddStoryCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddStoryCommandHandler(IMediator mediator, FabricDbContext context) {
      this._mediator = mediator;
      this._context = context;
    }

    public async Task<ItemDto?> Handle(AddStoryCommand request, CancellationToken cancellationToken) {
      var parentItem = await _context.GetItemDtoById(request.realmId);
      if (parentItem == null) { throw new Exception($"Parent item with id {request.realmId} not found"); }
      if (parentItem.ItemTypeId != (int)WeItemType.RealmModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)parentItem.ItemTypeId}; requires a {WeItemType.RealmModel} type {(int)WeItemType.RealmModel} parent.");
      }

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.StoryModel, request.name, request.description, "{}"));

      if (newItem != null) {

        if (request.povTypeId >= (int)WeItemType.PovUndefined && request.povTypeId <= (int)WeItemType.PovThirdPersonOmniscient) {
          var povDefaultProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPovDefault);
          if (povDefaultProp != null) {
            povDefaultProp.Value = request.povTypeId.ToString();
            await povDefaultProp.SaveProp(newItem, _mediator);
          }
        }

        var sceneCountProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTargetSceneCount);
        if (sceneCountProp != null) {
          if (request.sceneCount <= 0 || request.sceneCount > 10) {
            sceneCountProp.Value = 5.ToString();
          } else {
            sceneCountProp.Value = request.sceneCount.ToString();
          }
          await sceneCountProp.SaveProp(newItem, _mediator);
        }

        var addedByProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
        if (addedByProp != null && request.todoId > 0) {
          var attribution = await _mediator.Send(new ResolveAttributionQuery(request.todoId));
          addedByProp.Value = attribution.PresenceModelKey;
          await addedByProp.SaveProp(newItem, _mediator);
        }
      }
      return newItem;

    }
  }
}
