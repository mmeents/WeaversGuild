using MCPSharp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weavers.Core.Constants;
using Weavers.Core.Tools;

namespace Weavers.Core.Service {
  public class WeaversMcpHostedService : BackgroundService {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WeaversMcpHostedService> _logger;
    public WeaversMcpHostedService(IServiceProvider serviceProvider, ILogger<WeaversMcpHostedService> logger) {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      await Task.Delay(1500);
      _logger.LogInformation("🚀 Weavers MCP Server starting");
      DiBridgeService.Initialize(_serviceProvider);
      MCPServer.Register<BaseTools>();
      await MCPServer.StartAsync(Cx.McpAppName, Cx.AppVersion);
    }
  }
}
