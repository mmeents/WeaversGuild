using KB.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Weavers.Core {
  public static class DependencyInjection {
    public static IServiceCollection AddWeaversCore<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext {
      services.AddDbContext<TContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

      services.AddMediatR(cfg => {
        cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);  
      });
      return services;
    }
  }
}
