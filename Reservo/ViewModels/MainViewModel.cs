#region Usings
using Reservo.Commands;
using Reservo.Enums;
using Reservo.Views;
#endregion

namespace Reservo.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        private MenuItemType _selectedMenuItem;
        public MenuItemType SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => SetProperty(ref _selectedMenuItem, value);
        }

        private readonly TenantViewModel _tenantViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        public RelayCommand ShowTenantCommand { get; }
        public RelayCommand ShowSettingsCommand { get; }
        public RelayCommand FeedBackCommand { get; }

        public MainViewModel()
        {
            _tenantViewModel = new TenantViewModel();
            _settingsViewModel = new SettingsViewModel();

            _currentViewModel = _tenantViewModel;
            _selectedMenuItem = MenuItemType.Tenant;

            ShowTenantCommand = new RelayCommand(_ => ShowView(_tenantViewModel, MenuItemType.Tenant));
            ShowSettingsCommand = new RelayCommand(_ => ShowView(_settingsViewModel, MenuItemType.Settings));

            FeedBackCommand = new RelayCommand(OpenFeedbackDialog);
        }

        // Load Workbooks (.xslx files)
        public void LoadWorkbooks()
        {
            _ = _tenantViewModel.LoadWorkbooks();
        }

        // Save Workbooks (.xslx files)
        public void SaveWorkbooks()
        {
            _tenantViewModel.SaveWorkbooks();
        }

        //Sets the active view and synchronizes the selected menu item
        private void ShowView(BaseViewModel viewModel, MenuItemType menuItem)
        {
            CurrentViewModel = viewModel;
            SelectedMenuItem = menuItem;
        }

        //Opens Feedback View
        private void OpenFeedbackDialog(object? obj)
        {
            FeedBack feedbackWindow = new FeedBack(new FeedBackViewModel(new WindowService()));
            feedbackWindow.ShowDialog();
        }
    }
}
