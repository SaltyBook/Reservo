using System.IO;

namespace Reservo.Infrastructure
{
    public static class Paths
    {
        public static readonly string ManagementPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Verwaltung");
        public static readonly string ResourcesPath = Path.Combine(ManagementPath, "Resources");
        public static readonly string DatabasePath = Path.Combine(ManagementPath, "Datenbank");
    }
}
