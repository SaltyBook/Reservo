using Reservo.Commands;
using Reservo.Services.Window;
using Reservo.Trello;

namespace Reservo.ViewModels
{
    public class FeedBackViewModel : BaseViewModel
    {
        private readonly IWindowService _windowService;

        private string _subject = "";
        public string Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged();
                FeedBackCommand.RaiseCanExecuteChanged();
            }
        }

        private string _message = "";
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
                FeedBackCommand.RaiseCanExecuteChanged();
            }
        }

        public AsyncRelayCommand FeedBackCommand { get; }

        public FeedBackViewModel(WindowService windowService)
        {
            _windowService = windowService;

            FeedBackCommand = new AsyncRelayCommand(Send, CanSend);
        }

        private async Task Send()
        {
            TrelloFeedBack.SentCard(Subject, Message);
            _windowService.Close(this);
        }

        private bool CanSend()
        {
            return !string.IsNullOrWhiteSpace(Subject)
                && !string.IsNullOrWhiteSpace(Message);
        }
    }
}
