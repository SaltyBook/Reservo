using Reservo.Models;

namespace Reservo.Services.Document
{
    public interface IDocumentService
    {
        void CreateReservation(Entry entry, string year);
        void CreateInvoice(Entry entry, string year);
        void CreateReservationMail(Entry entry, string year);
        void CreateInvoiceMail(Entry entry, string year);
    }
}
