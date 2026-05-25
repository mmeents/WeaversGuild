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

    public GetAppSessionCommandHandler(FabricDbContext fabricDbContext, IMediator mediator, IAppSettingService settingService) {
      _dbContext = fabricDbContext;
      _mediator = mediator;
      _settingService = settingService;
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
           

      var orgRoot = await _dbContext.Items.FirstOrDefaultAsync(i => i.ItemTypeId == (int)WeItemType.OrganizationModel);
      result.OrganizationId = orgRoot?.Id ?? 0;
      ItemDto? orgItem = null;
      if (result.OrganizationId == 0) {
        orgItem = await _mediator.Send(new CreateItemCommand(Cx.AppName, (int)WeItemType.OrganizationModel, $"{Cx.AppName} - {Cx.AppDescription}", "{}")).ConfigureAwait(false);
        result.OrganizationId = orgItem?.Id ?? 0;
        if (orgItem == null || result.OrganizationId == 0) {
          throw new Exception("Failed to create organization root");
        }
        string orgRootFolder = _settingService.DefaultProjectsPath;
        await _mediator.SetProperty(orgItem, Cx.ItRootFolder, orgRootFolder).ConfigureAwait(false);        
        await _mediator.SetProperty(orgItem, Cx.ItOrgCharter, Cx.OrgCharter).ConfigureAwait(false);
        
        ItemDto? DocsItem = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains,
            (int)WeItemType.OrgDocFolderModel, $"Documents", "", "{}")).ConfigureAwait(false);
        if (DocsItem != null) {          
          var folderPath = Path.Combine(orgRootFolder, "docs");            
          await _mediator.SetProperty(DocsItem, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);          
        }
        ItemDto? DigitalOperatorPool = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains,
            (int)WeItemType.DigitalOperatorPoolModel, $"Digital Operators", "", "{}")).ConfigureAwait(false);

      }

      if (orgItem != null) {
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
      }

      var harnessName = $"{Cx.AppHarnessAppName}On{machineName}";
      var harnessType = await _dbContext.Items.FirstOrDefaultAsync(i => i.ItemTypeId == (int)WeItemType.HarnessAppModel && i.Name == harnessName);
      result.HarnessId = harnessType?.Id ?? 0;
      ItemDto? harnessItem = null;
      if (result.HarnessId == 0) {
        harnessItem = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains, (int)WeItemType.HarnessAppModel, harnessName, $"{Cx.AppHarnessAppName}", "{}")).ConfigureAwait(false);
        result.HarnessId = harnessItem?.Id ?? 0;
        if (harnessItem == null || result.HarnessId == 0) {
          throw new Exception("Failed to create harness item");
        }
        await _mediator.SetProperty(harnessItem, Cx.ItMachineName, machineName).ConfigureAwait(false);        
        await _mediator.SetProperty(harnessItem, Cx.ItUserName, userName).ConfigureAwait(false);        
      }

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
