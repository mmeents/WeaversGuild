using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Sessions {
  public record EnforceDataRetentionOrgPolicyCommand() : IRequest<bool>;
  public class EnforceDataRetentionOrgPolicyCommandHandler : IRequestHandler<EnforceDataRetentionOrgPolicyCommand, bool> {
    private readonly FabricDbContext _dbContext;
    private readonly IAppSessionService _session;
    private readonly IMediator _mediator;
    public EnforceDataRetentionOrgPolicyCommandHandler(FabricDbContext dbContext, IAppSessionService sessionService, IMediator mediator) {
      _dbContext = dbContext;
      _session = sessionService;
      _mediator = mediator;
    }

    public async Task<bool> Handle(EnforceDataRetentionOrgPolicyCommand request, CancellationToken cancellationToken) {
      
      var orgItem = await _dbContext.GetItemDtoById(_session.OrganizationId, cancellationToken);
      if (orgItem == null) {
        return false;
      }

      var retentionProp = orgItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRetentionDays)?.Value;
      if (int.TryParse(retentionProp, out int days) && days > 0) {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        var stale = await _dbContext.Items
            .Where(i => (i.ItemTypeId == (int)WeItemType.HarnessAppSessionModel)
                      && i.Established < cutoff)
            .ToListAsync(cancellationToken);
        foreach (var item in stale) {
          await _mediator.Send(new DeleteItemCommand(item.Id)).ConfigureAwait(false);
        }
      }

      return await Task.FromResult(true);
    }
  }
}
