using MediatR;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Sessions {

  public record GetAppSessionCommand() : IRequest<AppSessionResponse?>;

  public class AppSessionResponse {
    public int OrganizationId { get; set; } = 0;
    public int HarnessId { get; set; } = 0;
    public int SessionId { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }

  internal class GetAppSessionCommandHandler : IRequestHandler<GetAppSessionCommand, AppSessionResponse?> {
    private readonly FabricDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IAppSettingService _settingService;
    private readonly ISessionItemCacheService _sessionCache;

    public GetAppSessionCommandHandler(FabricDbContext fabricDbContext, IMediator mediator, IAppSettingService settingService, ISessionItemCacheService sessionCache) {
      _dbContext = fabricDbContext;
      _mediator = mediator;
      _settingService = settingService;
      _sessionCache = sessionCache;
    }

    public async Task<AppSessionResponse?> Handle(GetAppSessionCommand request, CancellationToken cancellationToken) {

      // so app starts up, it's a harness. there should be only 1 Organization root. need to find or add it.
      // need to figure out how to identify the instance of the harness that is self so that we can find the model of it.
      // if harness is not there add it.
      // then we can create a session item as child of harness and return the set as the session response.
      AppSessionResponse result = new AppSessionResponse();
      var machineName = Environment.MachineName.ToLower().AsUpperCaseFirstLetter();
      var userName = Environment.UserName;
      var processId = Environment.ProcessId;
      string orgRootFolder = _settingService.DefaultProjectsPath;

      var orgRoot = await _dbContext.Items.FirstOrDefaultAsync(i => i.ItemTypeId == (int)WeItemType.OrganizationModel);
      result.OrganizationId = orgRoot?.Id ?? 0;
      ItemDto? orgItem = null;
      if (result.OrganizationId == 0) {
        orgItem = await _mediator.Send(new CreateItemCommand(Cx.AppName, (int)WeItemType.OrganizationModel, $"{Cx.AppName} - {Cx.AppDescription}", "{}")).ConfigureAwait(false);
        result.OrganizationId = orgItem?.Id ?? 0;
        if (orgItem == null || result.OrganizationId == 0) {
          throw new Exception("Failed to create organization root");
        }

        await _mediator.SetProperty(orgItem, Cx.ItRootFolder, orgRootFolder).ConfigureAwait(false);
        await _mediator.SetProperty(orgItem, Cx.ItCharter, Cx.OrgCharter).ConfigureAwait(false);
      } else { 
        orgItem = await _dbContext.GetItemDtoById(result.OrganizationId, cancellationToken);
      }

      if (orgItem == null) { throw new Exception("Failed to get Organization from database."); }

      // inforce retention policy, delete stale sessions if any. retention days is configurable at org level, if not set, default to 30 days.
      var retentionProp = orgItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRetentionDays)?.Value;
      if (int.TryParse(retentionProp, out int days) && days > 0) {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        var stale = await _dbContext.Items
            .Where(i => (i.ItemTypeId == (int)WeItemType.HarnessAppSessionModel
                      || i.ItemTypeId == (int)WeItemType.HarnessMcpSessionModel)
                      && i.Established < cutoff)
            .ToListAsync(cancellationToken);
        _dbContext.Items.RemoveRange(stale);
        await _dbContext.SaveChangesAsync(cancellationToken);
      }
      
      // session base object exist?
      var harnessName = $"{Cx.AppHarnessAppName}On{machineName}";
      var aHarnessTypeId = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.HarnessAppModel && r.RelatedItemName == harnessName)?.RelatedItemId ?? 0;
      if (aHarnessTypeId == 0) {
        var harnessItem = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains, (int)WeItemType.HarnessAppModel, harnessName, $"{Cx.AppHarnessAppName}", "{}")).ConfigureAwait(false);
        result.HarnessId = harnessItem?.Id ?? 0;
        if (harnessItem == null || result.HarnessId == 0) {
          throw new Exception("Failed to create harness item");
        }
        await _mediator.SetProperty(harnessItem, Cx.ItMachineName, machineName).ConfigureAwait(false);
        await _mediator.SetProperty(harnessItem, Cx.ItUserName, userName).ConfigureAwait(false);
      } else {
        result.HarnessId = aHarnessTypeId;
      }

      // digital Operators
      var DigitalOperatorPoolRelation = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.DigitalOperatorPoolModel);
      if (DigitalOperatorPoolRelation == null) {
        ItemDto? DoPoolDto = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains,
            (int)WeItemType.DigitalOperatorPoolModel, $"Digital Operators", "", "{}")).ConfigureAwait(false);
        if (DoPoolDto != null) {
          var folderPath = Path.Combine(orgRootFolder, Cx.OrgDigiOpPoolFolder);
          await _mediator.SetProperty(DoPoolDto, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);
        }
      }

      // org chart
      var OrgChartRelation = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgChartModel);
      if (OrgChartRelation == null) {
        ItemDto? OrgChart = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains,
            (int)WeItemType.OrgChartModel, $"Org Chart", "", "{}")).ConfigureAwait(false);
        if (OrgChart != null) {
          var folderPath = Path.Combine(orgRootFolder, Cx.OrgChartFolder);
          await _mediator.SetProperty(OrgChart, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);
          ItemDto? defaultLogDesk = await _mediator.Send(
            new CreateRelatedItemCommand(OrgChart.Id, (int)WeRelationTypes.Contains,
              (int)WeItemType.DeskLogModel, $"TheLoomAppSyncDesk", "", "{}")).ConfigureAwait(false);
          if (defaultLogDesk != null) {
            var folderPath2 = Path.Combine(folderPath, "TheLoomAppSyncDesk");
            await _mediator.SetProperty(OrgChart, Cx.ItRelativeFolder, folderPath2).ConfigureAwait(false);
          }
        }
      }

      // docs folder
      var DocsRelation = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgDocFolderModel);
      if (DocsRelation == null) {
        ItemDto? DocsItem = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains,
            (int)WeItemType.OrgDocFolderModel, Cx.OrgDocsFolder, "", "{}")).ConfigureAwait(false);

        if (DocsItem != null) {
          var folderPath = Path.Combine(orgRootFolder, Cx.OrgDocsFolder);
          await _mediator.SetProperty(DocsItem, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);
        }
      }

      // finally, org looks good, create session level 3 under harness.

      ItemDto? sessionItem = await _mediator.Send(
        new CreateRelatedItemCommand(result.HarnessId, (int)WeRelationTypes.Contains, (int)WeItemType.HarnessAppSessionModel, $"{harnessName} at {DateTime.UtcNow}", "", "{}")).ConfigureAwait(false);
      result.SessionId = sessionItem?.Id ?? 0;
      if (sessionItem == null || result.SessionId == 0) {
        throw new Exception("Failed to create session item");
      }
      await _mediator.SetProperty(sessionItem, Cx.ItProcessId, processId.ToString()).ConfigureAwait(false);

      return result;
    }

  }
}
