#region Usings
using Reservo.Behavior;
using Reservo.ViewModels;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        // When the main window is fully loaded, it executes the LoadEntriesCommand
        // from the MainViewModel to load all entries from the data source (e.g., Excel).
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.LoadWorkbooks();
            }
        }

        // Before the application closes, it executes the SaveEntriesCommand
        // from the MainViewModel to persist all entries to the data source.
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.SaveWorkbooks();
                if (DataGridColumnOrderBehavior.LoadedGrid is not null)
                    ColumnOrderService.Save(DataGridColumnOrderBehavior.LoadedGrid);
            }
        }
    }
}