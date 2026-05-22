using Reservo.Models;
using Reservo.Services.Dialog;
using Reservo.Services.Email;
using Reservo.Services.PathService;
using Reservo.Views;

namespace Reservo.Services.Document
{
    public interface IDocumentService
    {
        void CreateReservation(Entry entry, string year, IPathService path);
        InvoiceWindow CreateInvoice(Entry entry, string year);
        void CreateReservationMail(Entry entry, string year, IEmailService email, IDialogService dialog, IPathService path);
        void CreateInvoiceMail(Entry entry, string year, IEmailService email, IDialogService dialog, IPathService path);
    }
}
