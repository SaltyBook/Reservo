using Reservo.Commands;
using Reservo.Services.Window;
using Reservo.Trello;

namespace Reservo.ViewModels
{
    public class FeedBackViewModel : BaseViewModel
    {
        private readonly IWindowService _windowService;

        private string _subject = string.Empty;
        public string Subject
        {
            get => _subject;
            set
            {
                if (SetProperty(ref _subject, value))
                {
                    FeedBackCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set
            {
                if (SetProperty(ref _message, value))
                {
                    FeedBackCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public AsyncRelayCommand FeedBackCommand { get; }

        public FeedBackViewModel(IWindowService windowService)
        {
            _windowService = windowService;

            FeedBackCommand = new AsyncRelayCommand(SendAsync, CanSend);
        }

        //Send the feedback to the external system Trello and then close the window
        private async Task SendAsync()
        {
            TrelloFeedBack.SentCardAsync(Subject, Message);
            _windowService.Close(this);
        }

        //Check whether the subject and message are set
        private bool CanSend()
        {
            return !string.IsNullOrWhiteSpace(Subject)
                && !string.IsNullOrWhiteSpace(Message);
        }
    }
}
