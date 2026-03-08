using Reservo.Models;

namespace Reservo.Services.Invoice
{
    public interface IInvoiceService
    {
        void CreateInvoice(Entry entry, IEnumerable<TableEntry> items, string year);
    }
}
