#region Usings
using Microsoft.Office.Interop.Word;
using Reservo.Documents;
using Reservo.Models;
using Reservo.Services.Document;
using Reservo.ViewModels;
using Reservo.Views;
using System.IO;
#endregion

namespace Reservo
{
    public class DocumentService : IDocumentService
    {
        public void CreateReservation(Entry entry, string year)
        {
            Reservation.CreateReservation(entry, year);
        }

        public void CreateInvoice(Entry entry, string year)
        {
            var vm = new InvoiceViewModel(entry, year);
            var window = new InvoiceWindow(vm);
            window.ShowDialog();
        }

        public void CreateReservationMail(Entry entry, string year)
        {
            ExportPdf(entry.GetReservationPath(year));
            Documents.Email.CreateEmail(entry, year, false);
        }

        public void CreateInvoiceMail(Entry entry, string year)
        {
            ExportPdf(entry.GetInvoicePath(year));
            Documents.Email.CreateEmail(entry, year, true);
        }

        private void ExportPdf(string docxPath)
        {
            if (!File.Exists(docxPath))
                return;

            string pdfPath = Path.ChangeExtension(docxPath, ".pdf");

            if (File.Exists(pdfPath))
                File.Delete(pdfPath);

            var app = new Application();
            var doc = app.Documents.Open(docxPath);
            doc.ExportAsFixedFormat(pdfPath, WdExportFormat.wdExportFormatPDF);
            doc.Close();
            app.Quit();
        }
    }
}
