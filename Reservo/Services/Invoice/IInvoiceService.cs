using Reservo.Models;

namespace Reservo.Services.Invoice
{
    public interface IInvoiceService
    {
        void CreateInvoice(Entry entry, List<TableEntry> items, string year);
    }
}
