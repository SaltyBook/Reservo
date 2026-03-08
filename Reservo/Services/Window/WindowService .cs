using Reservo.Services.Window;
using System.Windows;

namespace Reservo
{
    public class WindowService : IWindowService
    {
        public void Close(object viewModel)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == viewModel);

            window?.Close();
        }
    }
}
