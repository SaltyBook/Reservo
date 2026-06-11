using Reservo.Commands;
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.BillingFactor;
using Reservo.Services.Credentials;
using Reservo.Services.Dialog;
using Serilog;
using System.IO;
using System.Text.Json;

namespace Reservo.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IBillingFactorService _billingFactorService;

        private static readonly string _billingFactorPath = Path.Combine(Paths.ResourcesPath, "billingFactor.json");

        //General

        private string _databasePath = string.Empty;
        public string DatabasePath
        {
            get => _databasePath;
            set
            {
                if (SetProperty(ref _databasePath, value))
                {
                    SaveGeneralCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _calendarID = string.Empty;
        public string CalendarID
        {
            get => _calendarID;
            set
            {
                if (SetProperty(ref _calendarID, value))
                {
                    SaveGeneralCommand.RaiseCanExecuteChanged();
                }
            }
        }

        //Credentials

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

        //Trello

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

        private BillingFactorModel _billingFactorData = new();
        public BillingFactorModel BillingFactorData
        {
            get => _billingFactorData;
            set => SetProperty(ref _billingFactorData, value);
        }
       
        public RelayCommand SaveGeneralCommand { get; }
        public AsyncRelayCommand SaveCredentialsCommand { get; }
        public RelayCommand SaveServerCommand { get; }
        public RelayCommand SaveBillingFactorCommand { get; }

        public SettingsViewModel() : this(new DialogService(), App.BillingFactorService) { }

        public SettingsViewModel(IDialogService dialog, IBillingFactorService billingFactorService)
        {
            _dialogService = dialog;
            _billingFactorService = billingFactorService;

            SaveGeneralCommand = new RelayCommand(SaveGeneral, CanSaveGeneral);
            SaveCredentialsCommand = new AsyncRelayCommand(SaveCredentialsAsync, CanSaveCredentials);
            SaveServerCommand = new RelayCommand(SaveServer, CanSaveServer);
            SaveBillingFactorCommand = new RelayCommand(SaveBillingFactor, CanSaveBillingFactor);

            if (!String.IsNullOrEmpty(InternCredentials.DatabasePath))
            {
                DatabasePath = InternCredentials.DatabasePath;
            }

            if (!String.IsNullOrEmpty(InternCredentials.CalendarID))
            {
                CalendarID = InternCredentials.CalendarID;
            }

            LoadBillingFactor();
        }

        //Saves database path
        private void SaveGeneral(object? obj)
        {
            InternCredentials.WriteGeneralCredentials(DatabasePath, CalendarID);
            var credentialResult = InternCredentials.Save();
            if (!credentialResult.Success)
            {
                // Fehler
                _dialogService.ShowError("Fehler", credentialResult.Message);
            }
        }

        //Checks whether the database configuration are set
        private bool CanSaveGeneral(object? obj)
        {
            return true;
            return !string.IsNullOrWhiteSpace(DatabasePath);
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
            InternCredentials.WriteServerCredentials(ServerPath, TrelloApiKey, TrelloApiToken);
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
            return true;
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

        private void LoadBillingFactor()
        {
            try
            {
                if (!File.Exists(_billingFactorPath))
                {
                    BillingFactorData = new BillingFactorModel();
                    return;
                }

                string json = File.ReadAllText(_billingFactorPath);

                BillingFactorData = JsonSerializer.Deserialize<BillingFactorModel>(json)
                        ?? new BillingFactorModel();

                _billingFactorService.SaveBillingFactors(BillingFactorData);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Laden der internen Konfiguration.");
                BillingFactorData = new BillingFactorModel();
                _billingFactorService.SaveBillingFactors(BillingFactorData);
            }
        }

        public void SaveBillingFactor(object? obj)
        {
            Log.Information("Speichere interne Konfiguration.");

            try
            {
                string json = JsonSerializer.Serialize(BillingFactorData,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(_billingFactorPath, json);

                Log.Information("Interne Konfiguration erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Speichern der internen Konfiguration.");
            }
        }

        private bool CanSaveBillingFactor(object? obj)
        {
            return true;
        }
    }
}
