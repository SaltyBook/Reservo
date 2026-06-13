#region Usings
using Reservo.Commands;
using Reservo.Enums;
using Reservo.Services.Dialog;
using Reservo.Views;
using System.Drawing;
#endregion

namespace Reservo.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;

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

        private string _serverReachable = "#FF0000";
        public string ServerReachable
        {
            get => _serverReachable;
            set => SetProperty(ref _serverReachable, value);
        }

        private readonly TenantViewModel _tenantViewModel;
        private readonly StatisticViewModel _statisticViewModel;
        private readonly CalendarViewModel _calendarViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        public RelayCommand ShowTenantCommand { get; }
        public RelayCommand ShowStatisticCommand { get; }
        public RelayCommand ShowCalendarCommand { get; }
        public RelayCommand ShowSettingsCommand { get; }
        public RelayCommand FeedBackCommand { get; }

        #region Test
        public int Counter { get; private set; }

        public void Increment()
        {
            Counter++;
        }
        #endregion

        public MainViewModel()
        {
            _dialogService = new DialogService();

            _tenantViewModel = new TenantViewModel();
            _statisticViewModel = new StatisticViewModel();
            _calendarViewModel = new CalendarViewModel();
            _settingsViewModel = new SettingsViewModel();

            _currentViewModel = _tenantViewModel;
            _selectedMenuItem = MenuItemType.Tenant;

            ShowTenantCommand = new RelayCommand(_ => ShowView(_tenantViewModel, MenuItemType.Tenant));
            ShowStatisticCommand = new RelayCommand(_ => ShowView(_statisticViewModel, MenuItemType.Statistic));
            ShowCalendarCommand = new RelayCommand(_ => ShowView(_calendarViewModel, MenuItemType.Calendar));
            ShowSettingsCommand = new RelayCommand(_ => ShowView(_settingsViewModel, MenuItemType.Settings));

            FeedBackCommand = new RelayCommand(OpenFeedbackDialog);

            Init();
        }

        //Init StartUp
        private async void Init()
        {
            var credentialResult = await StartupService.RunAsync();

            if (!credentialResult.Success)
            {
                // Fehler
                _dialogService.ShowError("Fehler", credentialResult.Message);
            }
            else
            {
                ServerReachable = "#00FF00";
            }           
        }

        // Load Workbooks (.xslx files)
        public void LoadWorkbooks()
        {
            _ = _tenantViewModel.LoadWorkbooks(_statisticViewModel);
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

            if(SelectedMenuItem == MenuItemType.Statistic)
            {
                _statisticViewModel.CheckForUpdates(_tenantViewModel.Workbooks);
            }
            else if (SelectedMenuItem == MenuItemType.Calendar)
            {
                _calendarViewModel.Refresh(_tenantViewModel.Workbooks);
            }
        }

        //Opens Feedback View
        private void OpenFeedbackDialog(object? obj)
        {
            FeedBackWindow feedbackWindow = new FeedBackWindow(new FeedBackViewModel());
            feedbackWindow.ShowDialog();
        }
    }
}
