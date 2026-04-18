using Reservo.Models;
using Reservo.Services.Email;
using Reservo.Views;

namespace Reservo.Services.Document
{
    public interface IDocumentService
    {
        void CreateReservation(Entry entry, string year);
        InvoiceWindow CreateInvoice(Entry entry, string year);
        void CreateReservationMail(Entry entry, string year, IEmailService email);
        void CreateInvoiceMail(Entry entry, string year, IEmailService email);
    }
}
