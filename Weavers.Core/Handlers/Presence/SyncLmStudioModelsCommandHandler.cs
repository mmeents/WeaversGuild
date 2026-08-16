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
using System.Text.Json;

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
        
        var existingModelIds = gateway.Relations
          .Where(r => r.RelatedItemTypeId == (int)WeItemType.PresModelLmStudioModel)
          .Select(r => r.RelatedItemId)
          .ToHashSet();
        var existingKeys = new HashSet<string>();        
        foreach (var existingModelId in existingModelIds) { 
          var model = await _fabricDbContext.GetItemDtoById(existingModelId!.Value, cancellationToken);
          if (model != null) {
            var modelKey = model.Properties.FirstOrDefault(p => p.Name == Cx.ItModelKey)?.Value;
            if (modelKey != null) {
              existingKeys.Add(modelKey);
            }
          }
        }

        var lmStudioModels = await _lmStudioService.GetLlmModelsAsync(request.GatewayPresenceId, cancellationToken);
        lmStudioModels = lmStudioModels.DistinctBy(m => m.Key).ToList(); // Ensure unique models by Key
        Dictionary<string, LmModel> modelDictionary = lmStudioModels.ToDictionary(m => m.Key, m => m);

        foreach (var model in lmStudioModels) {          
          var modelKey = model.Key;
          if (!existingKeys.Contains(modelKey)) {
            var modelItem = await _mediator.Send(
              new CreateRelatedItemCommand(gateway.Id, (int)WeRelationTypes.Contains,
                (int)WeItemType.PresModelLmStudioModel, model.DisplayName, "", "{}")).ConfigureAwait(false);
            if (modelItem != null) {
              await _mediator.SetProperty(modelItem, Cx.ItModelKey, model.Key); // store the model ID as property.
              await _mediator.SetProperty(modelItem, Cx.ItModelName, model.DisplayName); // store the model name as property.
              var modelDetails = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
              await _mediator.SetProperty(modelItem, Cx.ItModelDetails, modelDetails); // store the model type as property.
            }
          }
        }

        foreach (string existingModel in existingKeys) {
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
