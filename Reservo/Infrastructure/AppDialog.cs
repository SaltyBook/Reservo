using Reservo.Enums;
using Reservo.Views;
using System.Windows;

namespace Reservo.Infrastructure
{
    public static class AppDialog
    {
        public static void ShowInfo(string title, string message, Window owner = null)
        {
            Show(title, message, DialogType.Info, false, owner);
        }

        public static void ShowSuccess(string title, string message, Window owner = null)
        {
            Show(title, message, DialogType.Success, false, owner);
        }

        public static void ShowWarning(string title, string message, Window owner = null)
        {
            Show(title, message, DialogType.Warning, false, owner);
        }

        public static void ShowError(string title, string message, Window owner = null)
        {
            Show(title, message, DialogType.Error, false, owner);
        }

        public static bool ShowConfirm(string title, string message, Window owner = null)
        {
            return Show(title, message, DialogType.Confirm, true, owner);
        }

        private static bool Show(string title, string message, DialogType dialogType, bool showCancel, Window owner)
        {
            var dialog = new DialogWindow(title, message, dialogType, showCancel);

            if (owner is not null)
                dialog.Owner = owner;

            return dialog.ShowDialog() == true;
        }
    }
}
