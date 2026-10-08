#region Usings
using Reservo.Behavior;
using Reservo.ViewModels;
using Serilog;
using System.ComponentModel;
using System.Windows;
#endregion

namespace Reservo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task? _loadTask;
        private bool _saveCompleted;

        public MainWindow()
        {
            InitializeComponent();
        }

        // When the main window is fully loaded, it executes the LoadEntriesCommand
        // from the MainViewModel to load all entries from the data source (e.g., Excel).
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                _loadTask = mainViewModel.LoadWorkbooksAsync();
                try
                {
                    await _loadTask;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Fehler beim Laden der Excel-Dateien");
                    _loadTask = Task.CompletedTask;
                    MessageBox.Show(this, "Die Excel-Dateien konnten nicht vollständig geladen werden.",
                        "Fehler beim Laden", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Before the application closes, it executes the SaveEntriesCommand
        // from the MainViewModel to persist all entries to the data source.
        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_saveCompleted)
                return;

            e.Cancel = true;
            IsEnabled = false;

            if (DataContext is MainViewModel mainViewModel)
            {
                try
                {
                    if (_loadTask is not null)
                    {
                        try
                        {
                            await _loadTask;
                        }
                        catch (Exception ex)
                        {
                            // Window_Loaded reports the load failure; still allow shutdown to continue.
                            Log.Warning(ex, "Laden war vor dem Beenden nicht erfolgreich");
                        }
                    }

                    if (DataGridColumnOrderBehavior.LoadedGrid is not null)
                        ColumnOrderService.Save(DataGridColumnOrderBehavior.LoadedGrid);

                    await mainViewModel.SaveWorkbooksAsync();
                    _saveCompleted = true;
                    Close();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Fehler beim Speichern beim Beenden");
                    MessageBox.Show(this, "Die Daten konnten nicht gespeichert werden. Die Anwendung bleibt geöffnet.",
                        "Fehler beim Speichern", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsEnabled = true;
                }
            }
            else
            {
                _saveCompleted = true;
                Close();
            }
        }
    }
}