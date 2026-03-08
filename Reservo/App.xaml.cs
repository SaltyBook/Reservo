using Reservo.Infrastructure;
using Serilog;
using System.Windows;

namespace Reservo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Logging.Init();

            Log.Information("=== Anwendung gestartet ===");

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("=== Anwendung beendet ===");
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}
