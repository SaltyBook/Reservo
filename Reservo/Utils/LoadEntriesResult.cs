using Reservo.Models;

namespace Reservo.Utils
{
    public class LoadEntriesResult
    {
        public List<Entry> Entries { get; } = new();
        public List<string> Warnings { get; } = new();
        public List<string> Errors { get; } = new();

        public bool HasErrors => Errors.Count > 0;
        public bool HasWarnings => Warnings.Count > 0;
    }
}
