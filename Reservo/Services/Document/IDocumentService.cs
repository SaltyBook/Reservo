using Reservo.Models;
using Reservo.Services.Dialog;
using Reservo.Services.Email;
using Reservo.Services.PathService;
using Reservo.Views;

namespace Reservo.Services.Document
{
    public interface IDocumentService
    {
        void CreateReservation(Entry entry, IPathService path);
        InvoiceWindow CreateInvoice(Entry entry);
        void CreateReservationMail(Entry entry, IEmailService email, IDialogService dialog, IPathService path);
        void CreateInvoiceMail(Entry entry, IEmailService email, IDialogService dialog, IPathService path);
    }
}
