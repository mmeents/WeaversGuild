using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Weavers.Api.Extensions;
using Weavers.Core;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;

namespace Weavers.Api {
  public class Program {
    public static async Task Main(string[] args) {
      ConfigureSerilog();
      var builder = WebApplication.CreateBuilder(args);

      builder.Configuration.Sources.Clear();
      builder.Configuration
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables()
          .AddCommandLine(args);

      builder.Logging.ClearProviders();
      builder.Logging.AddSerilog();
      builder.Services.AddMvc();
      builder.Services.AddWeaversCore<FabricDbContext>(builder.Configuration);
      builder.Services.AddSwaggerGen(options => {
        options.SwaggerDoc(Cx.ApiVersion, new OpenApiInfo { Title = $"{Cx.AppName} API", Version = Cx.ApiVersion });
      });

      builder.Services.AddCors(options => {
        options.AddPolicy("LocalDev", policy => {
          policy.WithOrigins(Cx.ApiLocalhostUrl)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
      });

      var app = builder.Build();
      app.UseCors("LocalDev");      
      if (app.Environment.IsDevelopment()) {
        app.MapSwagger();
        app.UseSwagger();
        app.UseSwaggerUI(options => {
          options.SwaggerEndpoint("v1/swagger.json", $"{Cx.AppName} {Cx.AppVersion}");
        });
      }
      app.UseHttpsRedirection();
      app.MapSummaryEndpoints()
         .MapGraphFileEndpoints()
         .MapMarkTodoEndpoint()
         .MapChessGameEndpoints();

      await app.RunAsync();
    }

    private static void ConfigureSerilog() {
      var logsPath = WeaverExt.LogsAppPath;
      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
          .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
          .Enrich.FromLogContext()
          .WriteTo.File(
              path: Path.Combine(logsPath, $"{Cx.AppName}-.log"),
              rollingInterval: RollingInterval.Day,
              retainedFileCountLimit: 7,
              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .WriteTo.File(
              path: Path.Combine(logsPath, $"{Cx.AppName}-errors-.log"),
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
