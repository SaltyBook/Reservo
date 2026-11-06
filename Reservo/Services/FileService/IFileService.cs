namespace Reservo.Services.FileService
{
    public interface IFileService
    {
        bool Exists(string path);
        void OpenFile(string path);
        void DeleteFile(string path);
    }
}
