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
            ServerPath = serverPath;
            TrelloApiKey = trelloAPIKey;
            TrelloApiToken = trelloAPIToken;
        }

        public static void Save()
        {
            Settings.Default.Save();
        }
    }
}
