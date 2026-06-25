using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Service;
using Weavers.Core.Tools;


namespace Weavers.Core {
  public static class DependencyInjection {
    public static IServiceCollection AddWeaversCore<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext {
      services.AddDbContext<TContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

      services.AddMediatR(cfg => {
        cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(McpLoggingBehavior<,>));
      });

      services.AddDataProtection()
        .SetApplicationName(Cx.AppName)
        .PersistKeysToFileSystem(new DirectoryInfo(WeaverExt.KeysAppPath)) // or config-driven path
        .SetDefaultKeyLifetime(TimeSpan.FromDays(Cx.KeyLifetimeDays));

      services.AddScoped<ICryptoService, CryptoService>();
      services.AddSingleton<ISessionItemCacheService, SessionItemCacheService>();

      services.AddSingleton<INotificationHandler<ItemUpdatedNotification>, ItemUpdatedNotificationHandler>();
      services.AddSingleton<IGraphItemUpdateService, GraphItemUpdateService>();
      services.AddSingleton<IAppSessionService, AppSessionService>();
      services.AddSingleton<IGatewayRunRegistry, GatewayRunRegistry>();

      services.AddScoped<IAppSettingService, AppSettingService>();
      services.AddScoped<IAppDataService, AppDataService>();
      services.AddScoped<IAppGraphOrgService, AppGraphOrgService>();
      services.AddScoped<IAppGraphFileService, AppGraphFileService>();
      services.AddScoped<IAppGraphClassService, AppGraphClassService>();
      services.AddScoped<IAppItemTemplateService, AppItemTemplateService>();
      services.AddScoped<IItemTypeLookupComboProvider, ItemTypeLookupComboProvider>();
      services.AddScoped<ILmStudioService, LmStudioService>();  
      services.AddScoped<IClaudeCodeService, ClaudeCodeService>();

      services.AddSingleton<IBaseToolsHandler, BaseToolsHandler>();
      services.AddSingleton<ISummaryToolsHandler, SummaryToolsHandler>();
      services.AddSingleton<IAppGraphOrgToolsHandler, AppGraphOrgToolsHandler>();
      services.AddSingleton<IAppGraphFileToolsHandler, AppGraphFileToolsHandler>();
      services.AddSingleton<IAppGraphLibraryToolsHandler, AppGraphLibraryToolsHandler>();
      services.AddSingleton<IAppGraphClassToolsHandler, AppGraphClassToolsHandler>();
      services.AddSingleton<IAppGraphEntityToolsHandler, AppGraphEntityToolsHandler>();      
      services.AddSingleton<ITodoToolsHandler, TodoToolsHandler>();

      return services;
    }

    public static IServiceCollection AddWeaversMCPCore(this IServiceCollection services, IConfiguration configuration) {
      AddWeaversCore<FabricDbContext>(services, configuration);      
      services.AddHostedService<WeaversMcpHostedService>();
      return services;
    }

  }

}
