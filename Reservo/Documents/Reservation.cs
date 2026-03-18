#region Usings
using Reservo.Infrastructure;
using Reservo.Models;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;
#endregion

namespace Reservo.Documents
{
    public static class Reservation
    {
        //Creates a reservation confirmation document based on a Word template.
        //The method copies the reservation template to a new file, replaces predefined placeholders with the booking
        //and customer data(such as group name, address, arrival and departure dates), and saves the completed document as a finalized reservation confirmation.
        public static void CreateReservation(Entry entry, string year)
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
    }
}
