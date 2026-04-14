using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using System.Reflection;
using Weavers.Core;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;

namespace TheLoomApp.Extensions {
  public static class AppServiceExts {
    public static IHost BuildHost() {
      ConfigureSerilog();
      var host = Host.CreateDefaultBuilder()
          .ConfigureAppConfiguration((context, config) => {
            config.Sources.Clear();
            // Get the directory where the executable is located
            var exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                               ?? Directory.GetCurrentDirectory();

            Log.Information("Loading configuration from: {ExeDirectory}", exeDirectory);

            // Add custom configuration file relative to executable location
            config.SetBasePath(exeDirectory)
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .AddEnvironmentVariables();
          })
          .ConfigureServices((context, services) => services.AddServices(context.Configuration))
          .Build();

      return host;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config) {
      services.AddWeaversCore<FabricDbContext>(config);
      services.AddSingleton<Form1>();
      return services;
    }

    private static void ConfigureSerilog() {
      var logsPath = WeaverExt.LogsAppPath;
      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
          .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
          .Enrich.FromLogContext()
          .WriteTo.File(
              path: Path.Combine(logsPath, $"{Cx.AppName}App-.log"),
              rollingInterval: RollingInterval.Day,
              retainedFileCountLimit: 7,
              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .WriteTo.File(
              path: Path.Combine(logsPath, $"{Cx.AppName}App-errors-.log"),
              rollingInterval: RollingInterval.Day,
              retainedFileCountLimit: 30,
              restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, // Only warnings and errors
              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .CreateLogger();

      Log.Information("Serilog configured - logging to {LogsPath}", logsPath);
    }


  }
}
