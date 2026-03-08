using Reservo.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Reservo.Views
{
    /// <summary>
    /// Interaction logic for Credentials.xaml
    /// </summary>
    public partial class Credentials : Window
    {
        public Credentials(CredentialsViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (DataContext as CredentialsViewModel)!.Password = ((PasswordBox)sender).Password; ;
        }
    }
}
