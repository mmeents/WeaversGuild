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
}
