using MCPSharp;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weavers.Core.Constants;
using Weavers.Core.Handlers.Sessions;
using Weavers.Core.Tools;

namespace Weavers.Core.Service {
  public class WeaversMcpHostedService(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    IServiceProvider serviceProvider,
    IAppSessionService appSessionService,
    ILogger<WeaversMcpHostedService> logger) : BackgroundService 
  {
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IAppSessionService _appSessionService = appSessionService;
    private readonly ILogger<WeaversMcpHostedService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      await Task.Delay(1500, stoppingToken);
      _logger.LogInformation("🚀 Weavers MCP Server starting");
      DiBridgeService.Initialize(_serviceProvider);
      MCPServer.Register<SummaryTools>();
      MCPServer.Register<AppGraphOrgTools>();
      MCPServer.Register<AppGraphFileTools>();
      MCPServer.Register<AppGraphLibraryTools>();
      MCPServer.Register<AppGraphClassTools>();
      MCPServer.Register<AppGraphEntityTools>();
      MCPServer.Register<TodoTools>();

      var mcpDriver = _configuration[Cx.Provider] ?? "McpPilotNameNotSet";
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var result = await mediator.Send(new GetAppSessionCommand(mcpDriver), stoppingToken);
      if (result == null) {
        _logger.LogError("Failed to get MCP session for driver {Driver}. MCP Server cannot start without a valid session.", mcpDriver);
        return;
      }
      _appSessionService.Initialize(mcpDriver, result.OrganizationId, result.HarnessId, result.SessionId);

      await MCPServer.StartAsync(Cx.McpAppName, Cx.AppVersion);
    }
  }
}
