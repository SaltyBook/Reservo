using Reservo.Models;

namespace Reservo.Services.PathService
{
    public interface IPathService
    {
        string GetReservationPath(Entry entry, string year);
        string GetInvoicePath(Entry entry, string year);
        int GetInvoiceCount(string year);
    }
}
