#region Usings
using Reservo.Commands;
using Reservo.Enums;
#endregion

namespace Reservo.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        private MenuItemType _selectedMenuItem;
        public MenuItemType SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ShowTenantCommand { get; }
        public RelayCommand ShowSettingsCommand { get; }

        private readonly TenantViewModel _tenantViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        public MainViewModel()
        {
            _tenantViewModel = new TenantViewModel();
            _settingsViewModel = new SettingsViewModel();

            _currentViewModel = _tenantViewModel;
            _selectedMenuItem = MenuItemType.Tenant;

            ShowTenantCommand = new RelayCommand(_ =>
            {
                CurrentViewModel = _tenantViewModel;
                SelectedMenuItem = MenuItemType.Tenant;
            });

            ShowSettingsCommand = new RelayCommand(_ =>
            {
                CurrentViewModel = _settingsViewModel;
                SelectedMenuItem = MenuItemType.Settings;
            });
        }

        public void LoadEntries()
        {
            _ = _tenantViewModel.LoadEntries();
        }

        public void SaveEntries()
        {
            _tenantViewModel.SaveEntries();
        }
    }
}
