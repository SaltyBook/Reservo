namespace Reservo.Models
{
    public class InternCredentialsModel
    {
        public string DatabasePath { get; set; } = string.Empty;
        public string CalendarID { get; set; } = string.Empty;
        public string ServerPath { get; set; } = string.Empty;
        public string TrelloApiKey { get; set; } = string.Empty;
        public string TrelloApiToken { get; set; } = string.Empty;
        public string SmtpHost { get; set; } = "secureimap.t-online.de";
        public int Port { get; set; } = 993;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
