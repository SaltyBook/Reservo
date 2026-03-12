using ClosedXML.Excel;
using Reservo.Infrastructure;
using System.IO;

namespace Reservo
{
    public class StartUp
    {
        public static void CreateFolderStructure()
        {
            if (!Directory.Exists(Paths.ManagementPath))
            {
                Directory.CreateDirectory(Paths.ManagementPath);
                Directory.CreateDirectory(Path.Combine(Paths.ManagementPath, $"{DateTime.Now.Year}-Rechnung"));
                Directory.CreateDirectory(Path.Combine(Paths.ManagementPath, $"{DateTime.Now.Year}-Reservierung"));
                Directory.CreateDirectory(Paths.DatabasePath);
                CreateDatabase(Path.Combine(Paths.DatabasePath, $"Mieterliste-{DateTime.Now.Year}.xlsx"));
            }
            else
            {
                if (!Directory.Exists(Path.Combine(Paths.ManagementPath, $"{DateTime.Now.Year}-Rechnung")) || !Directory.Exists(Path.Combine(Paths.ManagementPath, $"{DateTime.Now.Year}-Reservierung")))
                {
                    Directory.CreateDirectory(Path.Combine(Paths.ManagementPath, $"{DateTime.Now.Year}-Rechnung"));
                    Directory.CreateDirectory(Path.Combine(Paths.ManagementPath, $"{DateTime.Now.Year}-Reservierung"));
                }
            }
            if (!File.Exists(Path.Combine(Paths.DatabasePath, $"Mieterliste-{DateTime.Now.Year}.xlsx")))
            {
                CreateDatabase(Path.Combine(Paths.DatabasePath, $"Mieterliste-{DateTime.Now.Year}.xlsx"));
            }
        }

        private static void CreateDatabase(string database)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Gäste");
                worksheet.Cell(1, 1).Value = "Nr";
                worksheet.Cell(1, 2).Value = "Gäste";
                worksheet.Cell(1, 3).Value = "Gruppe";
                worksheet.Cell(1, 4).Value = "Anrede";
                worksheet.Cell(1, 5).Value = "Vorname";
                worksheet.Cell(1, 6).Value = "Name";
                worksheet.Cell(1, 7).Value = "Straße";
                worksheet.Cell(1, 8).Value = "Ort";
                worksheet.Cell(1, 9).Value = "Anreise";
                worksheet.Cell(1, 10).Value = "Abreise";
                worksheet.Cell(1, 11).Value = "Nächte";
                worksheet.Cell(1, 12).Value = "Infoblatt zurück?";
                worksheet.Cell(1, 13).Value = "Kalender-eintrag?";
                worksheet.Cell(1, 14).Value = "Rechnungsnummer";
                worksheet.Cell(1, 15).Value = "Summe";
                worksheet.Cell(1, 16).Value = "über 27";
                worksheet.Cell(1, 17).Value = "Zelt?";
                worksheet.Cell(1, 18).Value = "Getränke?";
                worksheet.Cell(1, 19).Value = "letzter Besuch in";
                worksheet.Cell(1, 20).Value = "Reserv. im";
                worksheet.Cell(1, 21).Value = "Kontakt über:";
                worksheet.Cell(1, 22).Value = "Mobil";
                worksheet.Cell(1, 23).Value = "Festnetz";
                worksheet.Cell(1, 24).Value = "Email";
                worksheet.Cell(1, 25).Value = "Storniert";
                worksheet.Cell(1, 26).Value = "Notizen";
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(database);
            }
        }
    }
}
