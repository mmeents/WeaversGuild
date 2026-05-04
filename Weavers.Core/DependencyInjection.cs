using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weavers.Core.Service;


namespace Weavers.Core {
  public static class DependencyInjection {
    public static IServiceCollection AddWeaversCore<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext {
      services.AddDbContext<TContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

      services.AddMediatR(cfg => {
        cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);  
      });

      services.AddSingleton<INotificationHandler<ItemUpdatedNotification>, ItemUpdatedNotificationHandler>();
      services.AddSingleton<IGraphItemUpdateService, GraphItemUpdateService>();

      services.AddScoped<IAppSettingService, AppSettingService>();
      services.AddScoped<IAppDataService, AppDataService>();
      services.AddScoped<IAppGraphFileService, AppGraphFileService>();
      services.AddScoped<IAppGraphClassService, AppGraphClassService>();
      services.AddScoped<IAppItemTemplateService, AppItemTemplateService>();
      services.AddScoped<IItemTypeLookupComboProvider, ItemTypeLookupComboProvider>();      

      return services;
    }

    public static IServiceCollection AddWeaversMCPCore(this IServiceCollection services, IConfiguration configuration) {
      AddWeaversCore<FabricDbContext>(services, configuration);

      services.AddHostedService<WeaversMcpHostedService>();
      return services;
    }

  }

}
