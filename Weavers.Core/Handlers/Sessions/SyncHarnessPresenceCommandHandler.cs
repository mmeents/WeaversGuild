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

namespace Weavers.Core.Handlers.Sessions {
  public record SyncHarnessPresenceCommand(int HarnessId, bool? HasLmStudio) : IRequest<bool>;
  public class SyncHarnessPresenceCommandHandler : IRequestHandler<SyncHarnessPresenceCommand, bool> {
    private readonly FabricDbContext _dbContext;
    private readonly IMediator _mediator;
    public SyncHarnessPresenceCommandHandler(FabricDbContext dbContext, IMediator mediator) {
      _dbContext = dbContext;
      _mediator = mediator;
    }

    public async Task<bool> Handle(SyncHarnessPresenceCommand request, CancellationToken cancellationToken) {
      if (request == null || request.HasLmStudio == null) return false;

      var harness = await _dbContext.GetItemDtoById(request.HarnessId);
      if (harness == null) { return false; }

      var hasLmStudioProp = harness.Properties.FirstOrDefault(p => p.Name == "HasLmStudio");
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
                (int)WeItemType.PresenceLmStudioGatewayModel, "LmStudio Defaults", "", "{}"), cancellationToken).ConfigureAwait(false);
          }
        } else {
          if (presenceItemId != 0) {
            await _mediator.Send(new DeleteItemCommand(presenceItemId), cancellationToken);
          }
        }
        
      }      

      return true;
    }
  }
}
