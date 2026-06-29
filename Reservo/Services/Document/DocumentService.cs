#region Usings
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Dialog;
using Reservo.Services.Document;
using Reservo.Services.Email;
using Reservo.Services.PathService;
using Reservo.ViewModels;
using Reservo.Views;
using Microsoft.Office.Interop.Word;
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
        public void CreateReservation(Entry entry, IPathService pathService)
        {
            string outputPath = pathService.GetReservationPath(entry);
            string templatePath = Path.Combine(Paths.ResourcesPath, "Reservierungsbestätigung-Vorlage.docx");
            File.Copy(templatePath, outputPath, true);
            using (var doc = DocX.Load(outputPath))
            {
                var replacements = new (string Placeholder, string Value)[]
                {
                    ("{{Gruppe}}", entry.GuestInfo.GroupName),
                    ("{{Anrede}}", entry.GuestInfo.Salutation),
                    ("{{Vorname}}", entry.GuestInfo.FirstName),
                    ("{{Name}}", entry.GuestInfo.LastName),
                    ("{{Straße}}", entry.GuestInfo.Street),
                    ("{{Ort}}", entry.GuestInfo.Location),
                    ("{{Nummer}}", entry.Id.ToString()),
                    ("{{Jahr}}", entry.Year.ToString()),
                    ("{{Datum}}", string.Format("{0:dddd, d. MMMM yyyy}", DateTime.Now)),
                    ("{{Anreise}}", string.Format("{0:dddd, d. MMMM yyyy}", entry.StayInfo.Arrival)),
                    ("{{Abreise}}", string.Format("{0:dddd, d. MMMM yyyy}", entry.StayInfo.Departure))
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

        public InvoiceWindow CreateInvoice(Entry entry)
        {
            var vm = new InvoiceViewModel(entry);
            var window = new InvoiceWindow(vm);
            return window;
        }

        public void CreateReservationMail(Entry entry, IEmailService emailService, IDialogService dialogService, IPathService pathService)
        {
            if(ExportPdf(pathService.GetReservationPath(entry), dialogService))
                emailService.CreateEmail(entry, false, pathService);
        }

        public void CreateInvoiceMail(Entry entry, IEmailService emailService, IDialogService dialogService, IPathService pathService)
        {
            if(ExportPdf(pathService.GetInvoicePath(entry), dialogService))
                emailService.CreateEmail(entry, true, pathService);
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

            var appWord = new Application();
            if (appWord.Documents != null)
            {
                //Load Document
                var wordDocument = appWord.Documents.Open(docxPath);
                if (wordDocument != null)
                {
                    //Convert Word to PDF
                    wordDocument.ExportAsFixedFormat(pdfPath, WdExportFormat.wdExportFormatPDF);
                    wordDocument.Close();
                }
                appWord.Quit();
            }

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
