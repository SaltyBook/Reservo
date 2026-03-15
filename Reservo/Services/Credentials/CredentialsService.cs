using Serilog;
using System.Net.Http;
using System.Net.Http.Json;

namespace Reservo.Services.Credentials
{
    public static class CredentialsService
    {
        // API Base-URL (Raspberry Pi)
        private static readonly string apiBaseUrl = InternCredentials.ServerPath ?? "http://192.168.2.213:5000/api/credentials";

        // HttpClient
        private static readonly HttpClient client = new HttpClient();

        // API Key        
        public static readonly string apiKey = Environment.GetEnvironmentVariable("CREDENTIAL_API_KEY") ?? "CREDENTIAL_KEY_NOT_SET";

        // Credentials
        public static SmtpCredentials? creds = null;

        public static void Load()
        {
            Log.Information("Initialisiere CredentialsService");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Log.Warning("API-Key für CredentialsService ist nicht gesetzt");
                return;
            }

            if (!client.DefaultRequestHeaders.Contains("X-API-KEY"))
            {
                client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            }

            Log.Information("CredentialsService erfolgreich initialisiert");
        }

        public static async Task<ServiceResult> ReadCredentials()
        {
            Log.Information("Lese SMTP-Credentials von externer API");

            try
            {
                var existsResponse = await client.GetFromJsonAsync<ExistsResponse>(apiBaseUrl + "/exists");

                if (existsResponse == null)
                {
                    Log.Warning("Antwort auf /exists war null");
                    return ServiceResult.Fail("Der Status der Zugangsdaten konnte nicht geprüft werden.");
                }

                if (!existsResponse.Exists)
                {
                    Log.Information("Keine SMTP-Credentials auf dem Server vorhanden");
                    return ServiceResult.Fail("Es sind keine gespeicherten Zugangsdaten vorhanden.");
                }

                creds = await client.GetFromJsonAsync<SmtpCredentials>(apiBaseUrl);

                if (creds == null)
                {
                    Log.Warning("SMTP-Credentials konnten nicht deserialisiert werden");
                    return ServiceResult.Fail("Die Zugangsdaten konnten nicht gelesen werden.");
                }

                Log.Information("SMTP-Credentials erfolgreich gelesen für Benutzer {Username}", creds.Username);

                return ServiceResult.Ok("Zugangsdaten wurden erfolgreich geladen.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Lesen der SMTP-Credentials");
                return ServiceResult.Fail("Beim Laden der Zugangsdaten ist ein Fehler aufgetreten.");
            }
        }

        public static async Task<ServiceResult> WriteCredentials(string username, string password)
        {
            Log.Information("Speichere SMTP-Credentials für Benutzer {Username}", username);

            string smtpHost = "secureimap.t-online.de";
            int port = 993;

            var smtpCredentials = new SmtpCredentials
            {
                SmtpHost = smtpHost,
                Port = port,
                Username = username,
                Password = password
            };

            try
            {
                var response = await client.PostAsJsonAsync(apiBaseUrl, smtpCredentials);

                if (response.IsSuccessStatusCode)
                {
                    Log.Information("SMTP-Credentials erfolgreich gespeichert für Benutzer {Username}", username);
                    return ServiceResult.Ok("Zugangsdaten wurden erfolgreich gespeichert.");
                }

                Log.Warning("Speichern der SMTP-Credentials fehlgeschlagen. StatusCode: {StatusCode}", response.StatusCode);
                return ServiceResult.Fail("Die Zugangsdaten konnten nicht gespeichert werden.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Speichern der SMTP-Credentials für Benutzer {Username}", username);
                return ServiceResult.Fail("Beim Speichern der Zugangsdaten ist ein Fehler aufgetreten.");
            }
        }
    }

    class ExistsResponse
    {
        public bool Exists { get; set; }
    }
}
