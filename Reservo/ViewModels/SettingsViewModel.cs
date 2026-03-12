using Reservo.Commands;
using Reservo.Services.Credentials;

namespace Reservo.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                SaveCredentialsCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                SaveCredentialsCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _useServer = false;
        public bool UseServer
        {
            get => _useServer;
            set
            {
                _useServer = value;
                OnPropertyChanged();
            }
        }

        private string _serverPath = "";
        public string ServerPath
        {
            get => _serverPath;
            set
            {
                _serverPath = value;
                OnPropertyChanged();
            }
        }

        private string _trelloAPIKey = "";
        public string TrelloAPIKey
        {
            get => _trelloAPIKey;
            set
            {
                _trelloAPIKey = value;
                OnPropertyChanged();
            }
        }

        private string _trelloAPIToken = "";
        public string TrelloAPIToken
        {
            get => _trelloAPIToken;
            set
            {
                _trelloAPIToken = value;
                OnPropertyChanged();
            }
        }

        public AsyncRelayCommand SaveCredentialsCommand { get; }
        public RelayCommand SaveServerCommand { get; }

        public SettingsViewModel()
        {
            SaveCredentialsCommand = new AsyncRelayCommand(SaveCredentials, CanSaveCredentials);
            SaveServerCommand = new RelayCommand(SaveServer, CanSaveServer);
        }

        private async Task SaveCredentials()
        {
            await CredentialsService.WriteCredentials(Username, CryptoHelper.Encrypt(Password));
            await CredentialsService.ReadCredentials();
            ClearAll();
        }

        private bool CanSaveCredentials()
        {
            return !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password);
        }

        private void SaveServer(object? obj)
        {
            InternCredentials.Write(ServerPath, TrelloAPIKey, TrelloAPIToken);
            InternCredentials.Save();
            ClearAll();
        }

        private bool CanSaveServer(object? obj)
        {
            return !string.IsNullOrWhiteSpace(ServerPath)
               && !string.IsNullOrWhiteSpace(TrelloAPIKey)
               && !string.IsNullOrWhiteSpace(TrelloAPIToken);
        }

        private void ClearAll()
        {
            Username = "";
            Password = "";
            ServerPath = "";
            TrelloAPIKey = "";
            TrelloAPIToken = "";
        }
    }
}
