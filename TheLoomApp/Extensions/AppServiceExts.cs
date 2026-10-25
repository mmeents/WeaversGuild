using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Weavers.Core;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;

namespace TheLoomApp.Extensions {

  public sealed class UiLogSink : ILogEventSink {
    public static readonly ConcurrentQueue<string> Queue = new();
    private readonly ITextFormatter _fmt =
      new MessageTemplateTextFormatter("[{Timestamp:HH:mm:ss}] {Level:u3} {Message:lj}{NewLine}{Exception}");

    public void Emit(LogEvent e) {
      using var sw = new StringWriter();
      _fmt.Format(e, sw);
      Queue.Enqueue(sw.ToString().TrimEnd());
    }
  }


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
          .UseSerilog()
          .Build();

      return host;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config) {
      services.AddWeaversCore<FabricDbContext>(config);
      services.AddSingleton<Form1>();
      return services;
    }

    public static readonly LoggingLevelSwitch LevelSwitch = new(LogEventLevel.Information);
    private const string Template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    private static void ConfigureSerilog() {
      Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
      var logsPath = WeaverExt.LogsAppPath;

      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.ControlledBy(LevelSwitch)                              // flip at runtime, e.g. from a menu
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information) // keep startup/bind lines
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.File(Path.Combine(logsPath, $"{Cx.AppName}App-.log"),
            rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7,
            shared: true, outputTemplate: Template)
        .WriteTo.File(Path.Combine(logsPath, $"{Cx.AppName}App-errors-.log"),
            rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30,
            restrictedToMinimumLevel: LogEventLevel.Warning,
            shared: true, outputTemplate: Template)
        .WriteTo.Sink(new UiLogSink(), LogEventLevel.Information)                                       // see below
        .CreateLogger();

      Log.Information("Serilog configured - logging to {LogsPath}", logsPath);
    }


  }



}
