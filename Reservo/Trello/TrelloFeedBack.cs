#region Usings
using Reservo.Services.Credentials;
using Serilog;
using System.Net.Http;
using System.Text.Json;
#endregion

namespace Reservo.Trello
{
    public static class TrelloFeedBack
    {
        public static async Task<ServiceResult> SendCardAsync(string subject, string message)
        {
            Log.Information("Sende Feedback an Trello. Betreff: {Subject}", subject);

            if (string.IsNullOrWhiteSpace(InternCredentials.TrelloApiKey) || string.IsNullOrWhiteSpace(InternCredentials.TrelloApiToken))
            {
                Log.Warning("Trello-Feedback konnte nicht gesendet werden, da API-Key oder Token fehlen");
                return ServiceResult.Fail("Trello ist nicht vollständig konfiguriert.");
            }

            var listId = "69aabdd49d14296b88814bc6";

            try
            {
                using var httpClient = new HttpClient();

                var createdCard = await CreateCardAsync(
                    httpClient,
                    InternCredentials.TrelloApiKey,
                    CryptoHelper.Decrypt(InternCredentials.TrelloApiToken),
                    listId,
                    subject,
                    message
                );
                Log.Information("Trello-Karte erfolgreich erstellt. Id: {Id}, Name: {Name}", createdCard.Id, createdCard.Name);

                return ServiceResult.Ok("Fehler/Feedback wurde erfolgreich gesendet.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Senden des Feedbacks an Trello");
                return ServiceResult.Fail("Das Feedback konnte nicht an Trello gesendet werden.");
            }
        }

        private static async Task<TrelloCardResponse> CreateCardAsync(HttpClient httpClient, string apiKey, string apiToken, string listId, string name, string description)
        {
            var query = new Dictionary<string, string>
            {
                ["key"] = apiKey,
                ["token"] = apiToken,
                ["idList"] = listId,
                ["name"] = name,
                ["desc"] = description
            };

            var queryString = string.Join("&", query.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var url = $"https://api.trello.com/1/cards?{queryString}";

            var response = await httpClient.PostAsync(url, null);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("Trello API Fehler. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, content);
                throw new Exception($"Trello API Fehler ({(int)response.StatusCode})");
            }

            var result = JsonSerializer.Deserialize<TrelloCardResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                Log.Warning("Trello-Antwort konnte nicht deserialisiert werden");
                throw new Exception("Antwort von Trello konnte nicht gelesen werden.");
            }

            return result;
        }

        public record TrelloCardResponse(string Id, string Name, string Url);
    }
}
