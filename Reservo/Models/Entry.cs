using Reservo.Infrastructure;
using Reservo.ViewModels;
using System.IO;
using System.Text.RegularExpressions;

namespace Reservo.Models
{
    public class Entry : BaseViewModel
    {
        private int id;
        private string groupName = string.Empty;
        private int guestCount;
        private string salutation = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string street = string.Empty;
        private string location = string.Empty;
        private DateTime arrival;
        private DateTime departure;
        private int nightCount;
        private bool infoSheet;
        private bool calendarEntry;
        private int? invoiceNumber;
        private double? total;
        private bool ageCheck;
        private bool tent;
        private double? drinks;
        private int? lastVisit;
        private DateTime reserved;
        private ContactValues.Contact contactVia;
        private string mobile = string.Empty;
        private string homePhone = string.Empty;
        private string eMail = string.Empty;
        private bool offer;
        private bool canceled;
        private string note = string.Empty;

        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string GroupName
        {
            get => groupName;
            set => SetProperty(ref groupName, value);
        }

        public int GuestCount
        {
            get => guestCount;
            set => SetProperty(ref guestCount, value);
        }

        public string Salutation
        {
            get => salutation;
            set => SetProperty(ref salutation, value);
        }

        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }

        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }

        public string Street
        {
            get => street;
            set => SetProperty(ref street, value);
        }

        public string Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }

        public DateTime Arrival
        {
            get => arrival;
            set
            {
                if (!SetProperty(ref arrival, value))
                    return;

                Departure = arrival.AddDays(2);
                CalcNightCount();
            }
        }

        public DateTime Departure
        {
            get => departure;
            set
            {
                if (!SetProperty(ref departure, value))
                    return;

                CalcNightCount();
            }
        }

        public int NightCount
        {
            get => nightCount;
            private set => SetProperty(ref nightCount, value);
        }

        public bool InfoSheet
        {
            get => infoSheet;
            set => SetProperty(ref infoSheet, value);
        }

        public bool CalendarEntry
        {
            get => calendarEntry;
            set => SetProperty(ref calendarEntry, value);
        }

        public int? InvoiceNumber
        {
            get => invoiceNumber;
            set => SetProperty(ref invoiceNumber, value);
        }

        public double? Total
        {
            get => total;
            set => SetProperty(ref total, value);
        }

        public bool AgeCheck
        {
            get => ageCheck;
            set => SetProperty(ref ageCheck, value);
        }

        public bool Tent
        {
            get => tent;
            set => SetProperty(ref tent, value);
        }

        public double? Drinks
        {
            get => drinks;
            set => SetProperty(ref drinks, value);
        }

        public int? LastVisit
        {
            get => lastVisit;
            set => SetProperty(ref lastVisit, value);
        }

        public DateTime Reserved
        {
            get => reserved;
            set => SetProperty(ref reserved, value);
        }

        public ContactValues.Contact ContactVia
        {
            get => contactVia;
            set => SetProperty(ref contactVia, value);
        }

        public string Mobile
        {
            get => mobile;
            set => SetProperty(ref mobile, value);
        }

        public string HomePhone
        {
            get => homePhone;
            set => SetProperty(ref homePhone, value);
        }

        public string EMail
        {
            get => eMail;
            set => SetProperty(ref eMail, value);
        }

        public bool Offer
        {
            get => offer;
            set => SetProperty(ref offer, value);
        }

        public bool Canceled
        {
            get => canceled;
            set => SetProperty(ref canceled, value);
        }

        public string Note
        {
            get => note;
            set => SetProperty(ref note, value);
        }

        private bool _isNoteOpen;
        public bool IsNoteOpen
        {
            get => _isNoteOpen;
            set => SetProperty(ref _isNoteOpen, value);
        }

        public Entry(int id, string groupName, int guestCount, string salutation, string firstName, string lastName, string street, string location, DateTime arrival, DateTime departure, int nightCount, bool infoSheet, bool calendarEntry, int? invoiceNumber, double? total, bool ageCheck, bool tent, double? drinks, int? lastVisit, DateTime reserved, ContactValues.Contact contactVia, string mobile, string homePhone, string eMail, bool offer, bool canceled, string note)
        {
            this.id = id;
            this.groupName = groupName;
            this.guestCount = guestCount;
            this.salutation = salutation;
            this.firstName = firstName;
            this.lastName = lastName;
            this.street = street;
            this.location = location;
            this.arrival = arrival;
            this.departure = departure;
            this.nightCount = nightCount;
            this.infoSheet = infoSheet;
            this.calendarEntry = calendarEntry;
            this.invoiceNumber = invoiceNumber;
            this.total = total;
            this.ageCheck = ageCheck;
            this.tent = tent;
            this.drinks = drinks;
            this.lastVisit = lastVisit;
            this.reserved = reserved;
            this.contactVia = contactVia;
            this.mobile = mobile;
            this.homePhone = homePhone;
            this.eMail = eMail;
            this.offer = offer;
            this.canceled = canceled;
            this.note = note;
        }

        public Entry()
        {
            arrival = DateTime.Now.Date;
            departure = DateTime.Now.Date.AddDays(2);
            reserved = DateTime.Now.Date;
            CalcNightCount();
        }

        //Builds and returns the file path for a reservation confirmation document.
        //The path is based on the current year, a dedicated reservation folder, the entry ID, and the guest’s last name to ensure a consistent and readable file naming scheme.
        public string GetReservationPath(string year)
        {
            return Path.Combine(Paths.ManagementPath, $"{year}-Reservierung", Id.ToString("D2") + "-Reservierung-" + LastName + ".docx");
        }

        //Builds and returns the file path for an invoice document.
        //If an invoice number already exists, it is reused; otherwise, a new sequential invoice number is determined and assigned.
        //The resulting path includes the current year, invoice index, and the guest’s last name.
        public string GetInvoicePath(string year)
        {
            int index = -1;
            if (InvoiceNumber != null && invoiceNumber != 0)
            {
                index = (int)InvoiceNumber;
            }
            else
            {
                index = GetInvoiceCount(year);
            }
            return Path.Combine(Paths.ManagementPath, $"{year}-Rechnung", index.ToString("D2") + "-Rechnung-" + LastName + ".docx");
        }

        //Calculates the next available invoice number by counting existing invoice files for the current year.
        //The method uses a regular expression to match valid invoice filenames and returns the next sequential index.
        public int GetInvoiceCount(string year)
        {
            var muster = new Regex(@"^\d{2}-Rechnung.*\.docx$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return Directory.EnumerateFiles(Path.Combine(Paths.ManagementPath, $"{year}-Rechnung")).Select(Path.GetFileName).Count(name => muster.IsMatch(name)) + 1;
        }

        //Calculates the number of overnight stays based on arrival and departure dates.
        //If the departure date is later than the arrival date, the difference in days is assigned as the night count. Otherwise, the value is set to zero.
        private void CalcNightCount()
        {
            if (Departure > Arrival)
                NightCount = (Departure - Arrival).Days;
            else
                NightCount = 0;
        }
    }
}
