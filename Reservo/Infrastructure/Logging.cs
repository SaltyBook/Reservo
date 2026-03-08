using Serilog;
using System.IO;

namespace Reservo.Infrastructure
{
    public static class Logging
    {
        public static void Init()
        {
            var basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Verwaltung",
                "Logs"
            );

            Directory.CreateDirectory(
                Path.GetDirectoryName(basePath)!
            );

            var fileName = $"reservo-{DateTime.Now:dd-MM-yyyy}.log";
            var logPath = Path.Combine(basePath, fileName);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.File(
                    path: logPath,
                    shared: true,
                    outputTemplate:
                        "{Timestamp:dd.MM.yyyy HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            Log.Information("Logger initialisiert. Pfad: {Path}", logPath);
        }
    }
}
