using Reservo.Services.Credentials;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace Reservo.Trello
{
    public static class TrelloFeedBack
    {
        public static async void SentCard(string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(InternCredentials.TrelloApiKey) || string.IsNullOrWhiteSpace(InternCredentials.TrelloApiToken))
            {
                Console.WriteLine("Bitte TRELLO_API_KEY und TRELLO_API_TOKEN setzen.");
            }

            var listId = "69aabdd49d14296b88814bc6";

            using var httpClient = new HttpClient();

            var createdCard = await CreateCardAsync(
                httpClient,
                InternCredentials.TrelloApiKey,
                CryptoHelper.Decrypt(InternCredentials.TrelloApiToken),
                listId,
                subject,
                message
            );

            MessageBox.Show("Fehler/Feedback erfolgreich gesendet", "Nachricht gesendet");
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

            var queryString = string.Join("&", query.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var url = $"https://api.trello.com/1/cards?{queryString}";

            var response = await httpClient.PostAsync(url, null);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Trello API Fehler ({(int)response.StatusCode}): {content}");
            }

            var result = JsonSerializer.Deserialize<TrelloCardResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
            {
                throw new Exception("Antwort von Trello konnte nicht gelesen werden.");
            }

            return result;
        }

        public record TrelloCardResponse(string Id, string Name, string Url);
    }
}
