using Reservo.Infrastructure;
using Reservo.Models;
using System.IO;
using System.Text.RegularExpressions;

namespace Reservo.Services.PathService
{
    public class PathService : IPathService
    {
        //Builds and returns the file path for a reservation confirmation document.
        //The path is based on the current year, a dedicated reservation folder, the entry ID, and the guest’s last name to ensure a consistent and readable file naming scheme.
        public string GetReservationPath(Entry entry, string year)
        {
            return Path.Combine(Paths.ManagementPath, $"{year}-Reservierung", entry.Id.ToString("D2") + "-Reservierung-" + entry.LastName + ".docx");
        }

        //Builds and returns the file path for an invoice document.
        //If an invoice number already exists, it is reused; otherwise, a new sequential invoice number is determined and assigned.
        //The resulting path includes the current year, invoice index, and the guest’s last name.
        public string GetInvoicePath(Entry entry, string year)
        {
            int index = -1;
            if (entry.InvoiceNumber is not null && entry.InvoiceNumber != 0)
            {
                index = (int)entry.InvoiceNumber;
            }
            else
            {
                index = GetInvoiceCount(year);
            }
            return Path.Combine(Paths.ManagementPath, $"{year}-Rechnung", index.ToString("D2") + "-Rechnung-" + entry.LastName + ".docx");
        }

        //Calculates the next available invoice number by counting existing invoice files for the current year.
        //The method uses a regular expression to match valid invoice filenames and returns the next sequential index.
        public int GetInvoiceCount(string year)
        {
            var muster = new Regex(@"^\d{2}-Rechnung.*\.docx$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return Directory.EnumerateFiles(Path.Combine(Paths.ManagementPath, $"{year}-Rechnung")).Select(Path.GetFileName).Count(name => muster.IsMatch(name)) + 1;
        }
    }
}
