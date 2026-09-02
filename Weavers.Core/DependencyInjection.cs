using MediatR;
using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Hash;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;
using Rudzoft.ChessLib.Validation;
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
      services.AddScoped<IFileSystemValidationService, FileSystemValidationService>();
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
      services.AddSingleton<IStorytimeToolsHandler, StorytimeToolsHandler>();
      services.AddSingleton<IChessToolsHandler, ChessToolsHandler>();

      services.AddHttpClient("RssResolver", c => {
        c.Timeout = TimeSpan.FromSeconds(30);        
        c.DefaultRequestHeaders.UserAgent.ParseAdd("WeaversGuild/1.0 (+RSS capture)");
        c.MaxResponseContentBufferSize = 10 * 1024 * 1024; // 10 MB cap, free size guard
        c.DefaultRequestHeaders.Accept.ParseAdd("text/markdown, text/html;q=0.9, text/plain;q=0.8");        
      });      

      services.AddTransient<IBoard, Board>()
      .AddSingleton<IPieceValue, PieceValue>()
      .AddSingleton<IBoard, Board>()
      .AddScoped<IPosition, Position>()
      .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>()
      .AddSingleton(static serviceProvider => {
        var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
        var policy = new DefaultPooledObjectPolicy<MoveList>();
        return provider.Create(policy);
      });

      return services;
    }

    public static IServiceCollection AddWeaversMCPCore(this IServiceCollection services, IConfiguration configuration) {
      AddWeaversCore<FabricDbContext>(services, configuration);      
      services.AddHostedService<WeaversMcpHostedService>();
      return services;
    }

  }

}
