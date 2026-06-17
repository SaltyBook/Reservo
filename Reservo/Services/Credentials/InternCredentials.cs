using Reservo.Infrastructure;
using Reservo.Models;
using Serilog;
using System.IO;
using System.Text.Json;

namespace Reservo.Services.Credentials
{
    public static class InternCredentials
    {
        private static readonly string _internCredentialsPath = Path.Combine(Paths.ResourcesPath, "internCredentials.json");

        private static InternCredentialsModel _data = new();

        public static string DatabasePath
        {
            get => _data.DatabasePath;
            set => _data.DatabasePath = value;
        }

        public static string CalendarID
        {
            get => _data.CalendarID;
            set => _data.CalendarID = value;
        }

        public static string ServerPath
        {
            get => _data.ServerPath;
            set => _data.ServerPath = value;
        }

        public static string TrelloApiKey
        {
            get => _data.TrelloApiKey;
            set => _data.TrelloApiKey = value;
        }

        public static string TrelloApiToken
        {
            get => _data.TrelloApiToken;
            set
            {
                _data.TrelloApiToken =
                    string.IsNullOrWhiteSpace(value)
                        ? string.Empty
                        : CryptoHelper.Encrypt(value);
            }
        }

        public static string Username
        {
            get => _data.Username;
            set => _data.Username = value;
        }

        public static string Password
        {
            get => _data.Password;
            set => _data.Password =
                string.IsNullOrWhiteSpace(value)
                        ? string.Empty
                        : CryptoHelper.Encrypt(value);
        }

        public static string SmtpHost
        {
            get => _data.SmtpHost;
            set => _data.SmtpHost = value;
        }

        public static int Port
        {
            get => _data.Port;
            set => _data.Port = value;
        }      

        static InternCredentials()
        {
            Load();
        }

        private static void Load()
        {
            try
            {
                if (!File.Exists(_internCredentialsPath))
                {
                    _data = new InternCredentialsModel();
                    return;
                }

                string json = File.ReadAllText(_internCredentialsPath);

                _data = JsonSerializer.Deserialize<InternCredentialsModel>(json)
                        ?? new InternCredentialsModel();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Laden der internen Konfiguration.");
                _data = new InternCredentialsModel();
            }
        }

        public static ServiceResult Save()
        {
            Log.Information("Speichere interne Konfiguration.");

            try
            {
                string json = JsonSerializer.Serialize(_data,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(_internCredentialsPath, json);

                Log.Information("Interne Konfiguration erfolgreich gespeichert.");

                return ServiceResult.Ok(
                    "Einstellungen wurden erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Fehler beim Speichern der internen Konfiguration.");

                return ServiceResult.Fail(
                    "Die Einstellungen konnten nicht gespeichert werden.");
            }
        }

        public static void WriteGeneralCredentials(string databasePath, string calendarID)
        {
            DatabasePath = databasePath;
            CalendarID = calendarID;
        }

        public static void WriteServerCredentials(string serverPath, string trelloApiKey, string trelloApiToken)
        {
            ServerPath = serverPath;
            TrelloApiKey = trelloApiKey;
            TrelloApiToken = trelloApiToken;
        }

        public static void WriteEmailCredentials(string username, string password, string smtpHost, int port)
        {
            Username = username;
            Password = password;
            SmtpHost = smtpHost;
            Port = port;
        }
    }
}
