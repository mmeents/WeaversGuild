using TheLoomApp.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace TheLoomApp {
  internal static class Program {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      ApplicationConfiguration.Initialize();
      using var host = AppServiceExts.BuildHost();
      var form1 = host.Services.GetRequiredService<Form1>();
      Application.Run(form1);
    }
  }
}