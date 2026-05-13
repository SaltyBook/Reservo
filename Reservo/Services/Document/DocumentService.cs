#region Usings
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Dialog;
using Reservo.Services.Document;
using Reservo.Services.Email;
using Reservo.ViewModels;
using Reservo.Views;
using Spire.Doc;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Document = Spire.Doc.Document;
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

        public void CreateReservationMail(Entry entry, string year, IEmailService emailService, IDialogService dialogService)
        {
            if(ExportPdf(entry.GetReservationPath(year), dialogService))
                emailService.CreateEmail(entry, year, false);
        }

        public void CreateInvoiceMail(Entry entry, string year, IEmailService emailService, IDialogService dialogService)
        {
            if(ExportPdf(entry.GetInvoicePath(year), dialogService))
                emailService.CreateEmail(entry, year, true);
        }

        private bool ExportPdf(string docxPath, IDialogService dialogService)
        {
            if (!File.Exists(docxPath))
            {
                dialogService.ShowInfo("Fehlende Datei", $"Der Pfad {docxPath} existiert nicht!");
                return false;
            }

            if (IsFileLocked(new FileInfo(docxPath)))
            {
                dialogService.ShowInfo("Datei bereits verwendet", $"Die Datei {docxPath} wird bereits verwendet!");
                return false;
            }

            string pdfPath = Path.ChangeExtension(docxPath, ".pdf");

            if (File.Exists(pdfPath))
                File.Delete(pdfPath);


            //Load Document
            Document document = new Document();
            document.LoadFromFile(docxPath);

            //Convert Word to PDF
            document.SaveToFile(pdfPath, FileFormat.PDF);

            return true;
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}
