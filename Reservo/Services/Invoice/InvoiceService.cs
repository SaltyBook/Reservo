#region Usings
using Reservo.Helpers;
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Invoice;
using Reservo.Services.TemplateMapper;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;
#endregion

namespace Reservo
{
    public class InvoiceService : IInvoiceService
    {
        private readonly InvoiceDataFactory _factory;
        private readonly IInvoiceTemplateMapper _mapper;

        public InvoiceService(InvoiceDataFactory factory, IInvoiceTemplateMapper mapper)
        {
            _factory = factory;
            _mapper = mapper;
        }

        public void CreateInvoice(Entry entry, List<TableEntry> entries, string year)
        {
            entry.InvoiceNumber = entry.GetInvoiceCount(year);

            var data = _factory.Create(entry, entries);

            var replacements = _mapper.Map(data);

            ProcessDocument(data, replacements, year);

            UpdateDatagrid(entry, entries);
        }

        //Generates an invoice document based on a Word template and the provided entry data.
        //The method copies the invoice template, replaces predefined placeholders with customer, booking, and pricing information,
        //conditionally removes optional rows(e.g.unused additional charges), saves the document, and updates the total amount in the associated data grid.
        private void ProcessDocument(InvoiceData data, Dictionary<string, string> replacements, string year)
        {
            string outputPath = data.Entry.GetInvoicePath(year);
            string templatePath = Path.Combine(Paths.ResourcesPath, "Rechnung-Vorlage.docx");
            File.Copy(templatePath, outputPath, true);
            using (var doc = DocX.Load(outputPath))
            {
                foreach (var kv in replacements)
                {
                    doc.ReplaceText(new StringReplaceTextOptions
                    {
                        SearchValue = kv.Key,
                        NewValue = kv.Value,
                        EscapeRegEx = true,   // geschweifte Klammern werden sicher maskiert
                        RemoveEmptyParagraph = true,   // leere Absätze nach Ersetzung aufräumen
                    });
                }
                if (data.Price13 == "0,00 €" || data.AdditionName == "Zusatz")
                {
                    doc.Tables[0].RemoveRow(doc.Tables[0].Rows.Count - 3);
                }
                doc.Save();
            }
        }

        //Updates the total amount of the given entry based on the calculated invoice result.
        //The method parses the formatted currency string, converts it to a numeric value, and assigns it to the entry’s total field for further processing or display.
        private static void UpdateDatagrid(Entry entry, List<TableEntry> entries)
        {
            entry.Total = Convert.ToDouble(entries[16].Result.Replace("€", "").Replace(".", ""));
        }
    }
}
