using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using System.Threading; 
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Comfy {
  public record AddComfyWorkflowCommand(int ParentItemId, string Name, string? ComfyExportFilePath) : IRequest<ItemDto?>;
  public class AddComfyWorkflowCommandHandler : IRequestHandler<AddComfyWorkflowCommand, ItemDto?> {
    private readonly IServiceScopeFactory _scopeFactory;
    public AddComfyWorkflowCommandHandler(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
    }
    public async Task<ItemDto?> Handle(AddComfyWorkflowCommand request, CancellationToken cancellationToken) {

      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();

      var fileContent = "{}";
      if (!string.IsNullOrWhiteSpace(request.ComfyExportFilePath)) {
        if (!File.Exists(request.ComfyExportFilePath)) {
          throw new ArgumentException($"Comfy export file at path '{request.ComfyExportFilePath}' does not exist", nameof(request.ComfyExportFilePath));
        }
        fileContent = await File.ReadAllTextAsync(request.ComfyExportFilePath);
      }

      if (request.ParentItemId <= 0) {
        throw new ArgumentException("ParentItemId must be greater than 0", nameof(request.ParentItemId));
      }
      var parentItem = await context.GetItemDtoById(request.ParentItemId);
      if (parentItem == null) {
        throw new ArgumentException($"Parent item with id {request.ParentItemId} not found", nameof(request.ParentItemId));
      }
      if (parentItem.ItemTypeId != (int)WeItemType.ComfyWorkflowFolderModel) {
        throw new ArgumentException($"Parent item with id {request.ParentItemId} is not a Workflow folder", nameof(request.ParentItemId));
      }

      var newItem = await mediator.Send(new CreateRelatedItemCommand(request.ParentItemId, 
        (int)WeRelationTypes.Contains, (int)WeItemType.ComfyWorkflowTemplate, 
          request.Name, "", fileContent));

      if (newItem == null) {
        throw new Exception("Failed to create new workflow item");
      }

      return newItem;
    }
  }
}
