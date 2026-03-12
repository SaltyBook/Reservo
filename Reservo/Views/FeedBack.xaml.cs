using Reservo.ViewModels;
using System.Windows;

namespace Reservo.Views
{
    /// <summary>
    /// Interaktionslogik für FeedBack.xaml
    /// </summary>
    public partial class FeedBack : Window
    {
        public FeedBack(FeedBackViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
