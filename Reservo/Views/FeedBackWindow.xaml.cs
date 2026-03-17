using Reservo.ViewModels;
using System.Windows;

namespace Reservo.Views
{
    /// <summary>
    /// Interaktionslogik für FeedBackWindow.xaml
    /// </summary>
    public partial class FeedBackWindow : Window
    {
        public FeedBackWindow(FeedBackViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
