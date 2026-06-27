using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Sessions {
  public record SyncHarnessPresenceCommand(int HarnessGatewayId, bool? HasLmStudio, bool? HasClaude) : IRequest<bool>;
  public class SyncHarnessPresenceCommandHandler : IRequestHandler<SyncHarnessPresenceCommand, bool> {
    private readonly FabricDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IClaudeCodeService _claudeCodeService;
    public SyncHarnessPresenceCommandHandler(FabricDbContext dbContext, IMediator mediator, IClaudeCodeService claudeCodeService) {
      _dbContext = dbContext;
      _mediator = mediator;
      _claudeCodeService = claudeCodeService;
    }

    public async Task<bool> Handle(SyncHarnessPresenceCommand request, CancellationToken cancellationToken) {
      if (request == null || request.HasLmStudio == null) return false;

      var harness = await _dbContext.GetItemDtoById(request.HarnessGatewayId);
      if (harness == null) { return false; }




      var hasLmStudioProp = harness.Properties.FirstOrDefault(p => p.Name == Cx.ItHasLmStudioPresence);
      if (hasLmStudioProp != null) {
        var hasLmStudioValue = hasLmStudioProp.Value.AsBoolean();
        if (hasLmStudioValue != request.HasLmStudio) {  // different
          hasLmStudioProp.Value = request.HasLmStudio.ToString();
          await hasLmStudioProp.SaveProp(harness, _mediator);
        }

        var presenceItemId = harness.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.PresenceLmStudioGatewayModel)?.RelatedItemId ?? 0;
        if (request.HasLmStudio.Value) {            
          if (presenceItemId == 0) {
            var presenceItem = await _mediator.Send(
              new CreateRelatedItemCommand(harness.Id, (int)WeRelationTypes.Contains,
                (int)WeItemType.PresenceLmStudioGatewayModel, "LmStudio Gateway", "", "{}"), cancellationToken).ConfigureAwait(false);
          }
        } else {
          if (presenceItemId != 0) {
            await _mediator.Send(new DeleteItemCommand(presenceItemId), cancellationToken);
          }
        }        
      }





      var hasClaudeProp = harness.Properties.FirstOrDefault(p => p.Name == Cx.ItHasClaudePresence);
      var hasClaudeValue = hasClaudeProp?.Value.AsBoolean() ?? false;
      if (hasClaudeProp != null) {        

        var gatewayItemId = harness.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.PresenceClaudeGatewayModel)?.RelatedItemId ?? 0;

        if (hasClaudeValue) {            
          if (gatewayItemId == 0) {
            var presenceItem = await _mediator.Send(
              new CreateRelatedItemCommand(harness.Id, (int)WeRelationTypes.Contains,
                (int)WeItemType.PresenceClaudeGatewayModel, "Claude Gateway", "", "{}"), cancellationToken).ConfigureAwait(false);

            if (presenceItem != null) {
              var models = await _claudeCodeService.GetLlmModelsAsync(harness.Id, cancellationToken);
              foreach (var model in models) {
                if (model != null) {
                  var modelItem = await _mediator.Send(
                    new CreateRelatedItemCommand(presenceItem.Id, (int)WeRelationTypes.Contains,
                      (int)WeItemType.PresModelClaudeModel, model.DisplayName, "", "{}"), cancellationToken).ConfigureAwait(false);
                  if (modelItem != null) {
                    await _mediator.SetProperty(modelItem, Cx.ItModelName, model.Key); // store the model name as property.                   
                  }
                }
              }
            }

          }
        } else { 
          if (gatewayItemId != 0) {
            await _mediator.Send(new DeleteItemCommand(gatewayItemId), cancellationToken);
          }
        }

        
      }    
      


      return true;
    }
  }
}
