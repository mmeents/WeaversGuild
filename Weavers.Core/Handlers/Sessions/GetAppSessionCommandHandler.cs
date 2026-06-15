using MediatR;
using Microsoft.EntityFrameworkCore;
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

  internal class GetAppSessionCommandHandler(
    FabricDbContext fabricDbContext,
    IMediator mediator,
    IAppSettingService settingService,
    ISessionItemCacheService sessionCache,
    IAppSessionService session
    ) : IRequestHandler<GetAppSessionCommand, AppSessionResponse?> {
    private readonly FabricDbContext _dbContext = fabricDbContext;
    private readonly IMediator _mediator = mediator;
    private readonly IAppSettingService _settingService = settingService;
    private readonly ISessionItemCacheService _sessionCache = sessionCache;
    private readonly IAppSessionService _session = session;

    public async Task<AppSessionResponse?> Handle(GetAppSessionCommand request, CancellationToken cancellationToken) {

      // so app starts up, it's a harness. there should be only 1 Organization root. need to find or add it.
      // need to figure out how to identify the instance of the harness that is self so that we can find the model of it.
      // if harness is not there add it.
      // then we can create a session item as child of harness and return the set as the session response.
      AppSessionResponse result = new();
      var machineName = Environment.MachineName.ToLower().AsUpperCaseFirstLetter();
      var userName = Environment.UserName;
      var processId = Environment.ProcessId;
      string orgRootFolder = _settingService.DefaultProjectsPath;

      var orgRoot = await _dbContext.Items.FirstOrDefaultAsync(i => i.ItemTypeId == (int)WeItemType.OrganizationModel, cancellationToken);
      result.OrganizationId = orgRoot?.Id ?? 0;
      ItemDto? orgItem = null;
      if (result.OrganizationId == 0) {
        _session.Initialize(userName, 1, 2, 3);
        orgItem = await _mediator.Send(new CreateItemCommand(Cx.AppName, (int)WeItemType.OrganizationModel, $"{Cx.AppName} - {Cx.AppDescription}", "{}"), cancellationToken).ConfigureAwait(false);
        result.OrganizationId = orgItem?.Id ?? 0;
        if (orgItem == null || result.OrganizationId == 0) {
          throw new Exception("Failed to create organization root");
        }

        await _mediator.SetProperty(orgItem, Cx.ItRootFolder, orgRootFolder).ConfigureAwait(false);
        await _mediator.SetProperty(orgItem, Cx.ItCharter, Cx.OrgCharter).ConfigureAwait(false);
      } else { 
        orgItem = await _sessionCache.GetItemAsync(result.OrganizationId, cancellationToken).ConfigureAwait(false);        
      }

      if (orgItem == null) { throw new Exception("Failed to get Organization from database."); }
      
      // session base object exist?
      var harnessName = $"{Cx.AppHarnessAppName}On{machineName}";
      var aHarnessTypeId = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.HarnessAppModel && r.RelatedItemName == harnessName)?.RelatedItemId ?? 0;
      if (aHarnessTypeId == 0) {
        var harnessItem = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains, (int)WeItemType.HarnessAppModel, harnessName, $"{Cx.AppHarnessAppName}", "{}"), cancellationToken).ConfigureAwait(false);
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
            (int)WeItemType.DigitalOperatorPoolModel, $"Digital Operators", "", "{}"), cancellationToken).ConfigureAwait(false);
        if (DoPoolDto != null) {
          var folderPath = Path.Combine(orgRootFolder, Cx.OrgDigiOpPoolFolder);
          await _mediator.SetProperty(DoPoolDto, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);
        }
      }

      // Orgs Desk Roles folder
      var DeskRolesRelation = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgDeskRolesModel);
      if (DeskRolesRelation == null) {
        ItemDto? DeskRoles = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains,
            (int)WeItemType.OrgDeskRolesModel, $"Org Desk Roles", "", "{}"), cancellationToken).ConfigureAwait(false);
        if (DeskRoles != null) {
          var folderPath = Path.Combine(orgRootFolder, Cx.OrgDeskRolesFolder);
          await _mediator.SetProperty(DeskRoles, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);
        }
      }

      // org chart
      var OrgChartRelation = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgChartModel);
      if (OrgChartRelation == null) {
        ItemDto? OrgChart = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains,
            (int)WeItemType.OrgChartModel, $"Org Chart", "", "{}"), cancellationToken).ConfigureAwait(false);
        if (OrgChart != null) {
          var folderPath = Path.Combine(orgRootFolder, Cx.OrgChartFolder);
          await _mediator.SetProperty(OrgChart, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);
          ItemDto? defaultLogDesk = await _mediator.Send(
            new CreateRelatedItemCommand(OrgChart.Id, (int)WeRelationTypes.Contains,
              (int)WeItemType.DeskLogModel, $"TheLoomAppSyncDesk", "", "{}"), cancellationToken).ConfigureAwait(false);
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
            (int)WeItemType.OrgDocFolderModel, Cx.OrgDocsFolder, "", "{}"), cancellationToken).ConfigureAwait(false);

        if (DocsItem != null) {
          var folderPath = Path.Combine(orgRootFolder, Cx.OrgDocsFolder);
          await _mediator.SetProperty(DocsItem, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);
        }
      }

      // finally, org looks good, create session level 3 under harness.

      ItemDto? sessionItem = await _mediator.Send(
        new CreateRelatedItemCommand(result.HarnessId, (int)WeRelationTypes.Contains, (int)WeItemType.HarnessAppSessionModel, $"{harnessName} at {DateTime.UtcNow}", "", "{}"), cancellationToken).ConfigureAwait(false);
      result.SessionId = sessionItem?.Id ?? 0;
      if (sessionItem == null || result.SessionId == 0) {
        throw new Exception("Failed to create session item");
      }
      await _mediator.SetProperty(sessionItem, Cx.ItProcessId, processId.ToString()).ConfigureAwait(false);

      _session.Initialize(userName, result.OrganizationId, result.HarnessId, result.SessionId);
      return result;
    }

  }
}
