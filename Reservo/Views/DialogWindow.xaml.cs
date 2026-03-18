using Reservo.Enums;
using System.Windows;
using System.Windows.Media;

namespace Reservo.Views
{
    /// <summary>
    /// Interaktionslogik für DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow(string title, string message, DialogType dialogType, bool showCancel = false)
        {
            InitializeComponent();

            Title = title;
            TitleText.Text = title;
            MessageText.Text = message;
            CancelButton.Visibility = showCancel ? Visibility.Visible : Visibility.Collapsed;

            ApplyDialogType(dialogType);
        }

        private void ApplyDialogType(DialogType dialogType)
        {
            switch (dialogType)
            {
                case DialogType.Info:
                    IconBackground.Background = (Brush)FindResource("Brush.InfoSoft");
                    IconPath.Fill = (Brush)FindResource("Brush.InfoStrong");
                    IconPath.Data = Geometry.Parse("M12 2C6.49 2 2 6.49 2 12s4.49 10 10 10 10-4.49 10-10S17.51 2 12 2Zm0 15a1.25 1.25 0 1 1 0-2.5A1.25 1.25 0 0 1 12 17Zm1-4h-2V7h2v6Z");
                    break;

                case DialogType.Success:
                    IconBackground.Background = (Brush)FindResource("Brush.SuccessSoft");
                    IconPath.Fill = (Brush)FindResource("Brush.SuccessStrong");
                    IconPath.Data = Geometry.Parse("M12 2C6.49 2 2 6.49 2 12s4.49 10 10 10 10-4.49 10-10S17.51 2 12 2Zm-1.2 14-4.3-4.3 1.4-1.4 2.9 2.9 5.3-5.3 1.4 1.4L10.8 16Z");
                    break;

                case DialogType.Warning:
                    IconBackground.Background = (Brush)FindResource("Brush.WarningSoft");
                    IconPath.Fill = (Brush)FindResource("Brush.WarningStrong");
                    IconPath.Data = Geometry.Parse("M1 21h22L12 2 1 21Zm12-3h-2v-2h2v2Zm0-4h-2v-4h2v4Z");
                    break;

                case DialogType.Error:
                    IconBackground.Background = (Brush)FindResource("Brush.ErrorSoft");
                    IconPath.Fill = (Brush)FindResource("Brush.ErrorStrong");
                    IconPath.Data = Geometry.Parse("M12 2C6.49 2 2 6.49 2 12s4.49 10 10 10 10-4.49 10-10S17.51 2 12 2Zm4.24 14.24-1.41 1.41L12 13.41l-2.83 2.83-1.41-1.41L10.59 12 7.76 9.17l1.41-1.41L12 10.59l2.83-2.83 1.41 1.41L13.41 12l2.83 2.24Z");
                    break;

                case DialogType.Confirm:
                    IconBackground.Background = (Brush)FindResource("Brush.InfoSoft");
                    IconPath.Fill = (Brush)FindResource("Brush.InfoStrong");
                    IconPath.Data = Geometry.Parse("M12 2C6.49 2 2 6.49 2 12s4.49 10 10 10 10-4.49 10-10S17.51 2 12 2Zm0 15h-.01ZM12 6a4 4 0 0 1 4 4c0 2-2 2.25-2 4h-2c0-2.25 2-2.5 2-4a2 2 0 1 0-4 0H8a4 4 0 0 1 4-4Z");
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
