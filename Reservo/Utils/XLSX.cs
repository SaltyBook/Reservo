#region Usings
using ClosedXML.Excel;
using ExcelDataReader;
using Reservo.Infrastructure;
using Reservo.Models;
using Serilog;
using System.IO;
#endregion

namespace Reservo.Utils
{
    public static class XLSX
    {
        // Loads all reservation entries from the yearly Excel file.
        // Returns both successfully loaded entries and detailed error information
        // for files or rows that could not be processed.
        public static LoadEntriesResult LoadXLSX(string path)
        {
            var result = new LoadEntriesResult();

            //var path = Path.Combine(Paths.ManagementPath, "Datenbank", $"Mieterliste-{DateTime.Now.Year}.xlsx");

            Log.Information("Lade Excel-Datei {Path}", path);

            try
            {
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = ExcelReaderFactory.CreateReader(stream);

                bool isHeader = true;
                int row = 0;

                while (reader.Read())
                {
                    row++;

                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    if (!TryParseRow(reader, out var entry))
                    {
                        //Log.Warning("Zeile {Row} konnte nicht gelesen werden", row);
                        continue;
                    }

                    result.Entries.Add(entry);
                }

                Log.Information("Excel geladen: {Count} Einträge, {Errors} Fehler", result.Entries.Count, result.Errors.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Laden der Excel-Datei");
                throw;
            }

            return result;
        }

        // Writes a collection of Entry objects to the Excel worksheet.
        // Creates before creating a Backup
        // Clears existing data rows (except the header), then writes each Entry
        // into consecutive rows starting from row 2. Finally, saves the workbook.
        public static void SaveXLSX(string path, IEnumerable<Entry> entries)
        {
            //var path = Path.Combine(
            //    Paths.ManagementPath,
            //    "Datenbank",
            //    $"Mieterliste-{DateTime.Now.Year}.xlsx"
            //);

            Log.Information("Speichern gestartet ({Count} Einträge)", entries.Count());

            try
            {
                CreateBackup(path);
                CleanupBackups(path, maxBackups: 5);

                using var workbook = new XLWorkbook(path);
                var sheet = workbook.Worksheet(1);

                ClearDataRows(sheet);

                int rowIndex = 2;
                foreach (var entry in entries)
                {
                    WriteRow(sheet, rowIndex++, entry);
                }

                workbook.Save();

                Log.Information("Speichern erfolgreich");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Speichern der Excel-Datei");
                throw;
            }
        }

        //Create Backup
        private static void CreateBackup(string sourceFile)
        {
            if (!File.Exists(sourceFile))
            {
                Log.Warning("Backup übersprungen – Datei existiert nicht: {File}", sourceFile);
                return;
            }

            var backupDir = Path.Combine(
                Path.GetDirectoryName(sourceFile)!,
                "Backups"
            );

            Directory.CreateDirectory(backupDir);

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var fileName = Path.GetFileNameWithoutExtension(sourceFile);
            var extension = Path.GetExtension(sourceFile);

            var backupFile = Path.Combine(
                backupDir,
                $"{fileName}_{timestamp}{extension}"
            );

            File.Copy(sourceFile, backupFile, overwrite: false);

            Log.Information("Backup erstellt: {Backup}", backupFile);
        }

        //Deletes the latest Backup greater than "maxBackup"
        private static void CleanupBackups(string sourceFile, int maxBackups = 5)
        {
            var backupDir = Path.Combine(
                Path.GetDirectoryName(sourceFile)!,
                "Backups"
            );

            if (!Directory.Exists(backupDir))
                return;

            var backups = new DirectoryInfo(backupDir)
                .GetFiles($"{Path.GetFileNameWithoutExtension(sourceFile)}_*")
                .OrderByDescending(f => f.CreationTimeUtc)
                .Skip(maxBackups);

            foreach (var file in backups)
            {
                try
                {
                    file.Delete();
                    Log.Debug("Altes Backup gelöscht: {File}", file.Name);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Backup konnte nicht gelöscht werden: {File}", file.Name);
                }
            }
        }

        // Attempts to create an Entry object from a single Excel row.
        private static bool TryParseRow(IExcelDataReader reader, out Entry entry)
        {
            entry = null!;

            try
            {
                if (reader.IsDBNull(Columns.Id)) return false;
                if (reader.IsDBNull(Columns.Arrival)) return false;
                if (reader.IsDBNull(Columns.Departure)) return false;

                int id = Convert.ToInt32(reader.GetValue(Columns.Id));
                int guestCount = Convert.ToInt32(reader.GetValue(Columns.GuestCount));
                int nightCount = Convert.ToInt32(reader.GetValue(Columns.NightCount));

                DateTime arrival = Convert.ToDateTime(reader.GetValue(Columns.Arrival));
                DateTime departure = Convert.ToDateTime(reader.GetValue(Columns.Departure));

                entry = new Entry(
                    id,
                    reader.GetString(Columns.GroupName),
                    guestCount,
                    reader.GetString(Columns.Salutation),
                    reader.GetString(Columns.FirstName),
                    reader.GetString(Columns.LastName),
                    reader.GetString(Columns.Street),
                    reader.GetString(Columns.Location),
                    arrival,
                    departure,
                    nightCount,
                    GetBool(reader.GetValue(Columns.InfoSheet)),
                    GetBool(reader.GetValue(Columns.CalendarEntry)),
                    GetNullableInt(reader.GetValue(Columns.InvoiceNumber)),
                    GetNullableDouble(reader.GetValue(Columns.Total)),
                    GetBool(reader.GetValue(Columns.AgeCheck)),
                    GetBool(reader.GetValue(Columns.Tent)),
                    GetNullableDouble(reader.GetValue(Columns.Drinks)),
                    GetNullableInt(reader.GetValue(Columns.LastVisit)),
                    Convert.ToDateTime(reader.GetValue(Columns.Reserved)),
                    GetContact(reader.GetValue(Columns.ContactVia)),
                    reader.GetString(Columns.Mobile),
                    reader.GetString(Columns.HomePhone),
                    reader.GetString(Columns.EMail),
                    GetBool(reader.GetValue(Columns.Canceled)),
                    reader.GetString(Columns.Note)
                );

                return true;
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Fehler beim Parsen einer Excel-Zeile");
                return false;
            }
        }


        // Writes all properties of an Entry object into a single Excel row
        private static void WriteRow(IXLWorksheet sheet, int row, Entry e)
        {
            sheet.Cell(row, XL(Columns.Id)).Value = e.Id;
            sheet.Cell(row, XL(Columns.GroupName)).Value = e.GroupName;
            sheet.Cell(row, XL(Columns.GuestCount)).Value = e.GuestCount;
            sheet.Cell(row, XL(Columns.Salutation)).Value = e.Salutation;
            sheet.Cell(row, XL(Columns.FirstName)).Value = e.FirstName;
            sheet.Cell(row, XL(Columns.LastName)).Value = e.LastName;
            sheet.Cell(row, XL(Columns.Street)).Value = e.Street;
            sheet.Cell(row, XL(Columns.Location)).Value = e.Location;
            sheet.Cell(row, XL(Columns.Arrival)).Value = e.Arrival;
            sheet.Cell(row, XL(Columns.Departure)).Value = e.Departure;
            sheet.Cell(row, XL(Columns.NightCount)).Value = e.NightCount;
            sheet.Cell(row, XL(Columns.InfoSheet)).Value = e.InfoSheet;
            sheet.Cell(row, XL(Columns.CalendarEntry)).Value = e.CalendarEntry;
            sheet.Cell(row, XL(Columns.InvoiceNumber)).Value = e.InvoiceNumber;
            sheet.Cell(row, XL(Columns.Total)).Value = e.Total;
            sheet.Cell(row, XL(Columns.AgeCheck)).Value = e.AgeCheck;
            sheet.Cell(row, XL(Columns.Tent)).Value = e.Tent;
            sheet.Cell(row, XL(Columns.Drinks)).Value = e.Drinks;
            sheet.Cell(row, XL(Columns.LastVisit)).Value = e.LastVisit;
            sheet.Cell(row, XL(Columns.Reserved)).Value = e.Reserved;
            sheet.Cell(row, XL(Columns.ContactVia)).Value = e.ContactVia.ToString();
            sheet.Cell(row, XL(Columns.Mobile)).Value = e.Mobile;
            sheet.Cell(row, XL(Columns.HomePhone)).Value = e.HomePhone;
            sheet.Cell(row, XL(Columns.EMail)).Value = e.EMail;
            sheet.Cell(row, XL(Columns.Canceled)).Value = e.Canceled;
            sheet.Cell(row, XL(Columns.Note)).Value = e.Note;
        }

        // Removes all existing data rows from the worksheet (starting at row 2), while keeping the header row intact.
        private static void ClearDataRows(IXLWorksheet sheet)
        {
            var lastRow = sheet.LastRowUsed()?.RowNumber();
            if (lastRow is null || lastRow < 2)
                return;

            sheet.Rows(2, lastRow.Value).Clear(XLClearOptions.Contents);
        }

        private static bool GetBool(object? value)
        {
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        private static int? GetNullableInt(object? value)
        {
            return value == null || value == DBNull.Value ? null : Convert.ToInt32(value);
        }

        private static double? GetNullableDouble(object? value)
        {
            return value == null || value == DBNull.Value ? null : Convert.ToDouble(value);
        }

        private static ContactValues.Contact GetContact(object? value)
        {
            if (value == null || value == DBNull.Value)
                return ContactValues.Contact.Sonstiges;

            return Enum.TryParse<ContactValues.Contact>(value.ToString(), out var result)
                ? result
                : ContactValues.Contact.Sonstiges;
        }

        private static int XL(int columnIndex) => columnIndex + 1;


        // Defines fixed column indices for the Excel worksheet.
        // Centralizes column mapping to avoid magic numbers and ensure consistency between reading and writing Excel data.
        private static class Columns
        {
            public const int Id = 0;
            public const int GroupName = 1;
            public const int GuestCount = 2;
            public const int Salutation = 3;
            public const int FirstName = 4;
            public const int LastName = 5;
            public const int Street = 6;
            public const int Location = 7;
            public const int Arrival = 8;
            public const int Departure = 9;
            public const int NightCount = 10;
            public const int InfoSheet = 11;
            public const int CalendarEntry = 12;
            public const int InvoiceNumber = 13;
            public const int Total = 14;
            public const int AgeCheck = 15;
            public const int Tent = 16;
            public const int Drinks = 17;
            public const int LastVisit = 18;
            public const int Reserved = 19;
            public const int ContactVia = 20;
            public const int Mobile = 21;
            public const int HomePhone = 22;
            public const int EMail = 23;
            public const int Canceled = 24;
            public const int Note = 25;
        }
    }
}
