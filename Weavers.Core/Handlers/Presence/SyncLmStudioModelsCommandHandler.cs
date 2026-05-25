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
using Weavers.Core.Models;
using Weavers.Core.Service;
using Microsoft.Extensions.Logging;

namespace Weavers.Core.Handlers.Presence {
  public record SyncLmStudioModelsCommand(int GatewayPresenceId) : IRequest<ItemDto?>;
  internal class SyncLmStudioModelsCommandHandler : IRequestHandler<SyncLmStudioModelsCommand, ItemDto?> {
    private readonly ILmStudioService _lmStudioService;
    private readonly FabricDbContext _fabricDbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<SyncLmStudioModelsCommandHandler> _logger;    

    public SyncLmStudioModelsCommandHandler(ILmStudioService lmStudioService, 
      FabricDbContext fabricDbContext, IMediator mediator, ILogger<SyncLmStudioModelsCommandHandler> logger, ICryptoService cryptoService) {
      _lmStudioService = lmStudioService;
      _fabricDbContext = fabricDbContext;
      _mediator = mediator;
      _logger = logger;
    }

    public async Task<ItemDto?> Handle(SyncLmStudioModelsCommand request, CancellationToken cancellationToken) {
      try {
        var gateway = await _fabricDbContext.GetItemDtoById(request.GatewayPresenceId, cancellationToken);
        if (gateway == null) {
          return null;
        }

        await _mediator.SetProperty(gateway, Cx.ItReSync, "0"); // Reset the re-sync flag

        var lmStudioUrl = gateway.Properties.FirstOrDefault(p => p.Name == Cx.ItUrlBase)?.Value;
        var ApiToken = gateway.Properties.FirstOrDefault(p => p.Name == Cx.ItApiToken)?.Value;
        if (ApiToken == null || lmStudioUrl == null) {
          throw new InvalidOperationException($"bad API token or LM Studio URL for Gateway ID {request.GatewayPresenceId}.");
        }
        
        var existingModels = gateway.Relations
          .Where(r => r.RelatedItemTypeId == (int)WeItemType.PresModelLmStudioModel)
          .Select(r => r.RelatedItemName)
          .ToHashSet();

        var lmStudioModels = await _lmStudioService.GetLlmModelsAsync(request.GatewayPresenceId, cancellationToken);
        Dictionary<string, LmModel> modelDictionary = lmStudioModels.ToDictionary(m => m.DisplayName, m => m);

        foreach (var model in lmStudioModels) {          
          var modelName = model.DisplayName;
          if (!existingModels.Contains(modelName)) {
            var modelItem = await _mediator.Send(
              new CreateRelatedItemCommand(gateway.Id, (int)WeRelationTypes.Contains,
                (int)WeItemType.PresModelLmStudioModel, modelName, "", "{}")).ConfigureAwait(false);
            if (modelItem != null) {
              await _mediator.SetProperty(modelItem, Cx.ItModelName, model.Key); // store the model name as property.
            }
          }
        }

        foreach (string existingModel in existingModels) {
          if (!modelDictionary.ContainsKey(existingModel)) {
            // Handle the case where the existing model is no longer present in LM Studio
            var missingModelRelation = gateway.Relations.FirstOrDefault(r => r.RelatedItemName == existingModel && r.RelatedItemTypeId == (int)WeItemType.PresModelLmStudioModel);
            if (missingModelRelation != null) {
              await _fabricDbContext.Items.FindAsync(missingModelRelation.RelatedItemId).AsTask().ContinueWith(t => {
                if (t.Result != null) {
                  t.Result.IsActive = false; // Soft delete by marking as inactive
                  t.Result.Description = $"Model '{existingModel}' is no longer present in LM Studio as of {DateTime.UtcNow}.";
                  _fabricDbContext.Items.Update(t.Result);
                  _fabricDbContext.SaveChanges();
                }
              });
            }
          }
        }

        gateway = await _fabricDbContext.GetItemDtoById(request.GatewayPresenceId, cancellationToken);
        return gateway;

      } catch (Exception ex) {
        _logger.LogError(ex, "Error syncing LM Studio models for GatewayPresenceId: {GatewayPresenceId}", request.GatewayPresenceId);
        return null;
      }
    }
  }
}
