using Reservo.Infrastructure;

namespace Reservo.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public void ShowInfo(string title, string message)
        {
            //MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            AppDialog.ShowInfo(title, message);
        }

        public void ShowError(string title, string message)
        {
            //MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            AppDialog.ShowError(title, message);
        }
    }
}
