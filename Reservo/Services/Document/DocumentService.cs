#region Usings
using Microsoft.Office.Interop.Word;
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Document;
using Reservo.Services.Email;
using Reservo.ViewModels;
using Reservo.Views;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;
#endregion

namespace Reservo
{
    public class DocumentService : IDocumentService
    {
        //Creates a reservation confirmation document based on a Word template.
        //The method copies the reservation template to a new file, replaces predefined placeholders with the booking
        //and customer data(such as group name, address, arrival and departure dates), and saves the completed document as a finalized reservation confirmation.
        public void CreateReservation(Entry entry, string year)
        {
            string outputPath = entry.GetReservationPath(year);
            string templatePath = Path.Combine(Paths.ResourcesPath, "Reservierungsbestätigung-Vorlage.docx");
            File.Copy(templatePath, outputPath, true);
            using (var doc = DocX.Load(outputPath))
            {
                var replacements = new (string Placeholder, string Value)[]
                {
                    ("{{Gruppe}}", entry.GroupName),
                    ("{{Anrede}}", entry.Salutation),
                    ("{{Vorname}}", entry.FirstName),
                    ("{{Name}}", entry.LastName),
                    ("{{Straße}}", entry.Street),
                    ("{{Ort}}", entry.Location),
                    ("{{Nummer}}", entry.Id.ToString()),
                    ("{{Jahr}}", string.Format("{0:yy}", DateTime.Now)),
                    ("{{Datum}}", string.Format("{0:dddd, d. MMMM yyyy}", DateTime.Now)),
                    ("{{Anreise}}", string.Format("{0:dddd, d. MMMM yyyy}", entry.Arrival)),
                    ("{{Abreise}}", string.Format("{0:dddd, d. MMMM yyyy}", entry.Departure))
                };
                foreach (var kv in replacements)
                {
                    doc.ReplaceText(new StringReplaceTextOptions
                    {
                        SearchValue = kv.Placeholder,
                        NewValue = kv.Value,
                        EscapeRegEx = true,   // geschweifte Klammern werden sicher maskiert
                        RemoveEmptyParagraph = true,   // leere Absätze nach Ersetzung aufräumen
                    });
                }
                doc.Save();
            }
        }

        public InvoiceWindow CreateInvoice(Entry entry, string year)
        {
            var vm = new InvoiceViewModel(entry, year);
            var window = new InvoiceWindow(vm);
            return window;
        }

        public void CreateReservationMail(Entry entry, string year, IEmailService emailService)
        {
            ExportPdf(entry.GetReservationPath(year));
            emailService.CreateEmail(entry, year, false);
        }

        public void CreateInvoiceMail(Entry entry, string year, IEmailService emailService)
        {
            ExportPdf(entry.GetInvoicePath(year));
            emailService.CreateEmail(entry, year, true);
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
