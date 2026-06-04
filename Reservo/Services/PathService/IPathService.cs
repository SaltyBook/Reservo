using Reservo.Models;

namespace Reservo.Services.PathService
{
    public interface IPathService
    {
        string GetReservationPath(Entry entry);
        string GetInvoicePath(Entry entry);
        int GetInvoiceCount(short year);
    }
}
