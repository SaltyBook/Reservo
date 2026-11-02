using ClosedXML.Excel;
using Reservo.Helpers;
using Reservo.Infrastructure;
using Reservo.Models;

namespace ReservoTest
{
    public class XlsxPersistenceTests
    {
        [Fact]
        public void SaveAndLoadXlsx_PreservesReservationFieldsAndCreatesBackup()
        {
            var directory = Path.Combine(Path.GetTempPath(), "ReservoTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            try
            {
                var path = Path.Combine(directory, "Mieterliste-2027.xlsx");
                using (var workbook = new XLWorkbook())
                {
                    var sheet = workbook.Worksheets.Add("Mieterliste");
                    sheet.Cell(1, 1).Value = "Nr";
                    workbook.SaveAs(path);
                }

                var arrival = DateTime.Today.AddMonths(2).Date.AddHours(12);
                var entry = new Entry(
                    42,
                    offer: true,
                    canceled: false,
                    note: "Fensterplatz gewünscht",
                    new GuestInfo("Testgruppe", 8, "Frau", "Ada", "Beispiel", "Teststraße 1", "Berlin", 2024, "0123", "456", "ada@example.test"),
                    new StayInfo(arrival, arrival.AddDays(3), 3, tent: false, ageCheck: true, infoSheet: true, calendarEntry: false,
                        reserved: DateTime.Today, ContactValues.Contact.Gruppenhaus),
                    new BillingInfo(1234, 250.75m, 18.50m));

                XLSX.SaveXLSX(path, new[] { entry });
                var result = XLSX.LoadXLSX(path);

                Assert.False(result.HasErrors);
                var loaded = Assert.Single(result.Entries);
                Assert.Equal(42, loaded.Id);
                Assert.Equal("Testgruppe", loaded.GuestInfo.GroupName);
                Assert.Equal(8, loaded.GuestInfo.GuestCount);
                Assert.Equal(arrival, loaded.StayInfo.Arrival);
                Assert.Equal(arrival.AddDays(3), loaded.StayInfo.Departure);
                Assert.Equal(250.75m, loaded.BillingInfo.Total);
                Assert.Equal("Fensterplatz gewünscht", loaded.Note);
                Assert.Single(Directory.GetFiles(Path.Combine(directory, "Backups"), "Mieterliste-2027_*.xlsx"));
            }
            finally
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, recursive: true);
            }
        }
    }
}
