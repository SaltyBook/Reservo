using Reservo.Documents;
using Reservo.Models;
using Reservo.Services.Invoice;

namespace Reservo
{
    public class InvoiceService : IInvoiceService
    {
        public void CreateInvoice(Entry entry, IEnumerable<TableEntry> items, string year)
        {
            Invoice.CreateInvoice(entry, items.ToList(), year);
        }
    }
}
