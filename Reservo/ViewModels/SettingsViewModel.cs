using Reservo.Commands;
using Reservo.Services.Credentials;
using Reservo.Services.Dialog;

namespace Reservo.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    SaveCredentialsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    SaveCredentialsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _useServer = false;
        public bool UseServer
        {
            get => _useServer;
            set => SetProperty(ref _useServer, value);
        }

        private string _serverPath = string.Empty;
        public string ServerPath
        {
            get => _serverPath;
            set
            {
                if (SetProperty(ref _serverPath, value))
                {
                    SaveServerCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _trelloApiKey = string.Empty;
        public string TrelloApiKey
        {
            get => _trelloApiKey;
            set
            {
                if (SetProperty(ref _trelloApiKey, value))
                {
                    SaveServerCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _trelloApiToken = string.Empty;
        public string TrelloApiToken
        {
            get => _trelloApiToken;
            set
            {
                if (SetProperty(ref _trelloApiToken, value))
                {
                    SaveServerCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public AsyncRelayCommand SaveCredentialsCommand { get; }
        public RelayCommand SaveServerCommand { get; }

        public SettingsViewModel() : this(new DialogService()) { }

        public SettingsViewModel(IDialogService dialog)
        {
            _dialogService = dialog;
            SaveCredentialsCommand = new AsyncRelayCommand(SaveCredentialsAsync, CanSaveCredentials);
            SaveServerCommand = new RelayCommand(SaveServer, CanSaveServer);
        }

        //Stores username and encrypted password
        private async Task SaveCredentialsAsync()
        {
            var credentialResult = await CredentialsService.WriteCredentials(Username, CryptoHelper.Encrypt(Password));
            if (!credentialResult.Success)
            {
                // Fehler
                _dialogService.ShowError("Fehler", credentialResult.Message);
            }
            var credentialResult2 = await CredentialsService.ReadCredentials();
            if (!credentialResult2.Success)
            {
                // Fehler
                _dialogService.ShowError("Fehler", credentialResult2.Message);
            }
            ClearAll();
        }

        //Check whether the user name and password are set
        private bool CanSaveCredentials()
        {
            return !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password);
        }

        //Saves server path and Trello login details
        private void SaveServer(object? obj)
        {
            InternCredentials.Write(ServerPath, TrelloApiKey, TrelloApiToken);
            var credentialResult = InternCredentials.Save();
            if (!credentialResult.Success)
            {
                // Fehler
                _dialogService.ShowError("Fehler", credentialResult.Message);
            }
            ClearAll();
        }

        //Checks whether the server configuration are set
        private bool CanSaveServer(object? obj)
        {
            return !string.IsNullOrWhiteSpace(ServerPath)
               && !string.IsNullOrWhiteSpace(TrelloApiKey)
               && !string.IsNullOrWhiteSpace(TrelloApiToken);
        }

        //Resets all input fields
        private void ClearAll()
        {
            Username = string.Empty;
            Password = string.Empty;
            ServerPath = string.Empty;
            TrelloApiKey = string.Empty;
            TrelloApiToken = string.Empty;
        }
    }
}
