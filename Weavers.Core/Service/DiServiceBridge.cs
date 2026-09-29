using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Service {
  public static class DiBridgeService {
    private static IServiceProvider? _serviceProvider;
    public static void Initialize(IServiceProvider serviceProvider) {
      _serviceProvider = serviceProvider;
    }
    public static T GetService<T>() where T : notnull {
      if (_serviceProvider == null) {
        throw new InvalidOperationException("Service provider has not been set. Call Initialize before useage.");
      }
      return _serviceProvider.GetRequiredService<T>();
    }
  }


  public static class DiBridgeService2 {
    private static IServiceScopeFactory? _serviceScopeFactory;
    public static void Initialize(IServiceProvider root)
      => _serviceScopeFactory = root.GetRequiredService<IServiceScopeFactory>();

    public static async Task<T> Send<T>(IRequest<T> request, CancellationToken ct = default) {
      if (_serviceScopeFactory is null) throw new InvalidOperationException("Call Initialize first.");
      await using var scope = _serviceScopeFactory.CreateAsyncScope();
      return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request, ct);
    }

    public static async Task Send(IRequest request, CancellationToken ct = default) {
      if (_serviceScopeFactory is null) throw new InvalidOperationException("Call Initialize first.");
      await using var scope = _serviceScopeFactory.CreateAsyncScope();
      await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request, ct);
    }

  }


}
