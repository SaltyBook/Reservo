using Reservo.Infrastructure;
using Reservo.Services.BillingFactor;
using Serilog;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Reservo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IBillingFactorService BillingFactorService { get; } = new BillingFactorService();

        protected override void OnStartup(StartupEventArgs e)
        {
            Logging.Init();

            Log.Information("=== Anwendung gestartet ===");

            base.OnStartup(e);

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            );
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("=== Anwendung beendet ===");
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}
