using System.Net.Http;
using System.Net.Http.Json;

namespace Reservo.Services.Credentials
{
    public static class CredentialsService
    {
        // API Base-URL (Raspberry Pi)
        private static readonly string apiBaseUrl = "http://192.168.2.213:5000/api/credentials";

        // HttpClient
        private static readonly HttpClient client = new HttpClient();

        // API Key        
        public static readonly string apiKey = Environment.GetEnvironmentVariable("CREDENTIAL_API_KEY") ?? "CREDENTIAL_KEY_NOT_SET";

        // Credentials
        public static SmtpCredentials? creds = null;

        public static void Load()
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        }

        public static async Task ReadCredentials()
        {
            try
            {
                var existsResponse = await client.GetFromJsonAsync<ExistsResponse>(apiBaseUrl + "/exists");

                if (existsResponse == null || !existsResponse.Exists)
                {
                    Console.WriteLine("Keine Credentials vorhanden.\n");
                    return;
                }

                creds = await client.GetFromJsonAsync<SmtpCredentials>(apiBaseUrl);
                if (creds != null)
                {
                    Console.WriteLine("=== Gefundene Credentials ===");
                    Console.WriteLine($"SMTP Host: {creds.SmtpHost}");
                    Console.WriteLine($"Port: {creds.Port}");
                    Console.WriteLine($"Username: {creds.Username}");
                    Console.WriteLine($"Password: {creds.Password}");
                    Console.WriteLine("============================\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Lesen der Credentials: " + ex.Message + "\n");
            }
        }

        public static async Task WriteCredentials(string username, string password)
        {
            string smtpHost = "secureimap.t-online.de";

            int port = 993;

            var creds = new SmtpCredentials
            {
                SmtpHost = smtpHost,
                Port = port,
                Username = username,
                Password = password
            };

            try
            {
                var response = await client.PostAsJsonAsync(apiBaseUrl, creds);
                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Credentials erfolgreich gespeichert!\n");
                else
                    Console.WriteLine("Fehler beim Speichern: " + response.StatusCode + "\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Speichern: " + ex.Message + "\n");
            }
        }
    }

    class ExistsResponse
    {
        public bool Exists { get; set; }
    }
}
