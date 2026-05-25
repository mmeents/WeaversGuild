using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Sessions {
  public record GetMcpSessionCommand(string ProviderType) : IRequest<McpSessionResponse?>;

  public class McpSessionResponse {    
    public int OrganizationId { get; set; } = 0;
    public int HarnessId { get; set; } = 0;
    public int SessionId { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }

  public class GetMcpSessionCommandHandler : IRequestHandler<GetMcpSessionCommand, McpSessionResponse?> {
    private readonly FabricDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IAppSettingService _settingService;
    public GetMcpSessionCommandHandler(FabricDbContext dbContext, IMediator mediator, IAppSettingService settingService) {
      _dbContext = dbContext;
      _mediator = mediator;
      _settingService = settingService;
    }

    public async Task<McpSessionResponse?> Handle(GetMcpSessionCommand request, CancellationToken cancellationToken) {
      McpSessionResponse result = new McpSessionResponse();
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

      var harnessName = $"{Cx.AppHarnessMcpName}On{machineName}";
      var harnessType = await _dbContext.Items.FirstOrDefaultAsync(i => i.ItemTypeId == (int)WeItemType.HarnessMcpModel && i.Name == harnessName);
      result.HarnessId = harnessType?.Id ?? 0;
      ItemDto? harnessItem = null;
      if (result.HarnessId == 0) {
        harnessItem = await _mediator.Send(
          new CreateRelatedItemCommand(result.OrganizationId, (int)WeRelationTypes.Contains, 
            (int)WeItemType.HarnessMcpModel, harnessName, $"{Cx.AppHarnessMcpName}", "{}")).ConfigureAwait(false);
        result.HarnessId = harnessItem?.Id ?? 0;
        if (harnessItem == null || result.HarnessId == 0) {
          throw new Exception("Failed to create harness item");
        }
        await _mediator.SetProperty(harnessItem, Cx.ItMachineName, machineName).ConfigureAwait(false);
        await _mediator.SetProperty(harnessItem, Cx.ItUserName, userName).ConfigureAwait(false);
      }

      ItemDto? sessionItem = await _mediator.Send(
        new CreateRelatedItemCommand(result.HarnessId, (int)WeRelationTypes.Contains, 
          (int)WeItemType.HarnessMcpSessionModel, $"{request.ProviderType} at {DateTime.UtcNow}", "", "{}")).ConfigureAwait(false);
      result.SessionId = sessionItem?.Id ?? 0;
      if (sessionItem == null || result.SessionId == 0) {
        throw new Exception("Failed to create session item");
      }
      await _mediator.SetProperty(sessionItem, Cx.ItProcessId, processId.ToString()).ConfigureAwait(false);
      await _mediator.SetProperty(sessionItem, Cx.ItProviderType, request.ProviderType).ConfigureAwait(false);

      return result;
    }
  }
}
