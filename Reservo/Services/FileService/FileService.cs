using Org.BouncyCastle.Bcpg.Sig;
using System.Diagnostics;
using System.IO;

namespace Reservo.Services.FileService
{
    public class FileService : IFileService
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void OpenFile(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }
    }
}
