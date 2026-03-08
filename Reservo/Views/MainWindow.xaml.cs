#region Usings
using Reservo.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            this.DataContext = new MainViewModel();
            StartupService.Run();
            Loaded += MainWindow_Loaded;
            Closing += Window_Closing;
        }

        // When the main window is fully loaded, it executes the LoadEntriesCommand
        // from the MainViewModel to load all entries from the data source (e.g., Excel).
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadEntriesCommand.Execute(null);
            }
        }

        // Before the application closes, it executes the SaveEntriesCommand
        // from the MainViewModel to persist all entries to the data source.
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SaveEntriesCommand.Execute(null);

                var grid = FindVisualChild<DataGrid>(this);
                if (grid != null)
                    ColumnOrderService.Save(grid);
            }
        }

        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}