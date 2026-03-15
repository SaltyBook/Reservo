using Serilog;

namespace Reservo.Services.Credentials
{
    public static class InternCredentials
    {
        public static string ServerPath
        {
            get => Settings.Default.ServerPath;
            set => Settings.Default.ServerPath = value;
        }

        public static string TrelloApiKey
        {
            get => Settings.Default.TrelloAPIKey;
            set => Settings.Default.TrelloAPIKey = value;
        }

        public static string TrelloApiToken
        {
            get => Settings.Default.TrelloAPIToken;
            set
            {
                Settings.Default.TrelloAPIToken =
                    string.IsNullOrWhiteSpace(value)
                        ? string.Empty
                        : CryptoHelper.Encrypt(value);
            }
        }

        public static void Write(string serverPath, string trelloAPIKey, string trelloAPIToken)
        {
            Log.Information("Schreibe interne Konfiguration");

            ServerPath = serverPath;
            TrelloApiKey = trelloAPIKey;
            TrelloApiToken = trelloAPIToken;
        }

        public static ServiceResult Save()
        {
            Log.Information("Speichere interne Konfiguration in Settings");

            try
            {
                Settings.Default.Save();
                Log.Information("Interne Konfiguration erfolgreich gespeichert");
                return ServiceResult.Ok("Einstellungen wurden erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Speichern der internen Konfiguration");
                return ServiceResult.Fail("Die Einstellungen konnten nicht gespeichert werden.");
            }
        }
    }
}
