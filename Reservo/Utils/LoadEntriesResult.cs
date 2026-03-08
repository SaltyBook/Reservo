using Reservo.Models;

namespace Reservo.Utils
{
    public class LoadEntriesResult
    {
        public List<Entry> Entries { get; } = new();
        public List<RowLoadError> Errors { get; } = new();

        public bool HasErrors()
        {
            return Errors.Count > 0;
        }
    }
}
