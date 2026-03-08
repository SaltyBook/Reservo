namespace Reservo.Services.Dialog
{
    public interface IDialogService
    {
        void ShowInfo(string title, string message);
        void ShowError(string title, string message);
    }
}
