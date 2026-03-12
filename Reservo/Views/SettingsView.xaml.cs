using Reservo.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Reservo.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (DataContext as SettingsViewModel)!.Password = ((PasswordBox)sender).Password;
        }

        private void PasswordBox_APITokenChanged(object sender, RoutedEventArgs e)
        {
            (DataContext as SettingsViewModel)!.TrelloAPIToken = ((PasswordBox)sender).Password;
        }
    }
}
