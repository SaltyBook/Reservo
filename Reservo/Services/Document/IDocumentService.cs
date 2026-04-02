using Reservo.Models;
using Reservo.Services.Email;

namespace Reservo.Services.Document
{
    public interface IDocumentService
    {
        void CreateReservation(Entry entry, string year);
        void CreateInvoice(Entry entry, string year);
        void CreateReservationMail(Entry entry, string year, IEmailService email);
        void CreateInvoiceMail(Entry entry, string year, IEmailService email);
    }
}
