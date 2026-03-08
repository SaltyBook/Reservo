using Reservo.Commands;
using Reservo.Services.Credentials;
using Reservo.Services.Window;

namespace Reservo.ViewModels
{
    public class CredentialsViewModel : BaseViewModel
    {
        private readonly IWindowService _windowService;

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
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
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public AsyncRelayCommand SaveCommand { get; }

        public CredentialsViewModel(IWindowService windowService)
        {
            _windowService = windowService;

            SaveCommand = new AsyncRelayCommand(Save, CanSave);
        }

        private async Task Save()
        {
            await CredentialsService.WriteCredentials(Username, CryptoHelper.Encrypt(Password));
            await CredentialsService.ReadCredentials();
            _windowService.Close(this);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password);
        }

    }
}
