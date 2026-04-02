using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Invoice;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Reservo
{
    public class InvoiceService : IInvoiceService
    {
        //Generates an invoice document based on a Word template and the provided entry data.
        //The method copies the invoice template, replaces predefined placeholders with customer, booking, and pricing information,
        //conditionally removes optional rows(e.g.unused additional charges), saves the document, and updates the total amount in the associated data grid.
        public void CreateInvoice(Entry entry, List<TableEntry> entries, string year)
        {
            entry.InvoiceNumber = entry.GetInvoiceCount(year);
            string outputPath = entry.GetInvoicePath(year);
            string templatePath = Path.Combine(Paths.ResourcesPath, "Rechnung-Vorlage.docx");
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
                    ("{{Nummer}}", entry.InvoiceNumber.ToString()),
                    ("{{Jahr}}", string.Format("{0:yy}", DateTime.Now)),
                    ("{{Datum}}", string.Format("{0:d.MM.yyyy}", DateTime.Now)),
                    ("{{Nächte}}", entry.NightCount.ToString()),
                    ("{{Anreise}}", string.Format("{0:d.MM.yyyy}", entry.Arrival)),
                    ("{{Abreise}}", string.Format("{0:d.MM.yyyy}", entry.Departure)),
                    ("{{Preis1}}", entries[0].Result),
                    ("{{Kinder}}", entries[1].Quantity.ToString()),
                    ("{{Junge}}",entries[2].Quantity.ToString()),
                    ("{{Erwachsen}}", entries[3].Quantity.ToString()),
                    ("{{Ab27}}", entries[4].Quantity.ToString()),
                    ("{{Preis2}}", entries[4].Result),
                    ("{{Bis26}}", entries[5].Quantity.ToString()),
                    ("{{Preis3}}", entries[5].Result),
                    ("{{Bis17}}", entries[6].Quantity.ToString()),
                    ("{{Preis4}}", entries[6].Result),
                    ("{{Gäst}}", entries[7].Quantity.ToString()),
                    ("{{Preis5}}", entries[7].Result),
                    ("{{Heizm}}", entries[8].Quantity.ToString()),
                    ("{{Preis6}}", entries[8].Result),
                    ("{{Bett}}", entries[9].Quantity.ToString()),
                    ("{{Preis7}}", entries[9].Result),
                    ("{{Wasch}}", entries[10].Quantity.ToString()),
                    ("{{Preis8}}", entries[10].Result),
                    ("{{Leih}}", entries[11].Quantity.ToString()),
                    ("{{Preis9}}", entries[11].Result),
                    ("{{Holz}}", entries[12].Quantity.ToString()),
                    ("{{Preis10}}", entries[12].Result),
                    ("{{Preis11}}", entries[13].Result),
                    ("{{Preis12}}", entries[14].Result),
                    ("{{Zusatz}}", entries[15].Description),
                    ("{{Preis13}}", entries[15].Result),
                    ("{{Total}}", entries[16].Result)
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
                if (entries[15].Result == "0,00 €" || entries[15].Description == "Zusatz")
                {
                    doc.Tables[0].RemoveRow(doc.Tables[0].Rows.Count - 3);
                }
                doc.Save();
            }
            UpdateDatagrid(entry, entries);
        }

        //Updates the total amount of the given entry based on the calculated invoice result.
        //The method parses the formatted currency string, converts it to a numeric value, and assigns it to the entry’s total field for further processing or display.
        private static void UpdateDatagrid(Entry entry, List<TableEntry> entries)
        {
            entry.Total = Convert.ToDouble(entries[16].Result.Replace("€", "").Replace(".", ""));
        }
    }
}
