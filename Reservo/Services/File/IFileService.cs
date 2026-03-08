namespace Reservo.Services.File
{
    public interface IFileService
    {
        bool Exists(string path);
        void OpenFile(string path);
    }
}
