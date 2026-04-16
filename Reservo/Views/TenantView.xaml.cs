using Reservo.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reservo.Views
{
    /// <summary>
    /// Interaktionslogik für TenantView.xaml
    /// </summary>
    public partial class TenantView : UserControl
    {
        public TenantView()
        {
            InitializeComponent();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                if (DataContext is TenantViewModel vm)
                    vm.CopyCommand.Execute(null);

                e.Handled = true;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                if (DataContext is TenantViewModel vm)
                    vm.PasteCommand.Execute(null);

                e.Handled = true;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X)
            {
                if (DataContext is TenantViewModel vm)
                    vm.CutCommand.Execute(null);

                e.Handled = true;
            }
        }
    }
}
