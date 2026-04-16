using Reservo.Models;
using Reservo.ViewModels;

namespace Reservo.Helpers
{
    public class CopyToParameter
    {
        public Entry Entry { get; set; }
        public WorkbookViewModel FromWorkbook { get; set; }
        public bool Delete { get; set; }

        public CopyToParameter(Entry entry, WorkbookViewModel fromWorkbook, bool delete)
        {
            Entry = entry;
            FromWorkbook = fromWorkbook;
            Delete = delete;
        }
    }
}
