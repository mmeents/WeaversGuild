using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Comfy {
  public record AddComfyWfParamCommand(int ParentItemId, string Name) : IRequest<ItemDto?>;

  public class AddComfyWfParamCommandHandler : IRequestHandler<AddComfyWfParamCommand, ItemDto?> {
    private readonly IServiceScopeFactory _scopeFactory;
    public AddComfyWfParamCommandHandler(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
    }
    public async Task<ItemDto?> Handle(AddComfyWfParamCommand request, CancellationToken cancellationToken) {

      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();

      if (request.ParentItemId <= 0) {
        throw new ArgumentException("ParentItemId must be greater than 0", nameof(request.ParentItemId));
      }
      var parentItem = await context.GetItemDtoById(request.ParentItemId);
      if (parentItem == null) {
        throw new ArgumentException($"Parent item with id {request.ParentItemId} not found", nameof(request.ParentItemId));
      }
      if (parentItem.ItemTypeId != (int)WeItemType.ComfyWorkflowTemplate) {
        throw new ArgumentException($"Parent item with id {request.ParentItemId} is not a Workflow template", nameof(request.ParentItemId));
      }

      var newItem = await mediator.Send(new CreateRelatedItemCommand(request.ParentItemId,
        (int)WeRelationTypes.Contains, (int)WeItemType.ComfyWfParamModel, 
          request.Name, "", "{}"));

      if (newItem == null) {
        throw new Exception("Failed to create new workflow parameter override.");
      }

      return newItem;
    }
  }
}
