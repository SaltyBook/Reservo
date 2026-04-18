using Reservo.Infrastructure;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace Reservo.Models
{
    public class Entry : INotifyPropertyChanged
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
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string GroupName
        {
            get => groupName;
            set
            {
                if (groupName != value)
                {
                    groupName = value;
                    OnPropertyChanged(nameof(GroupName));
                }
            }
        }

        public int GuestCount
        {
            get => guestCount;
            set
            {
                if (guestCount != value)
                {
                    guestCount = value;
                    OnPropertyChanged(nameof(GuestCount));
                }
            }
        }

        public string Salutation
        {
            get => salutation;
            set
            {
                if (salutation != value)
                {
                    salutation = value;
                    OnPropertyChanged(nameof(Salutation));
                }
            }
        }

        public string FirstName
        {
            get => firstName;
            set
            {
                if (firstName != value)
                {
                    firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        public string LastName
        {
            get => lastName;
            set
            {
                if (lastName != value)
                {
                    lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        public string Street
        {
            get => street;
            set
            {
                if (street != value)
                {
                    street = value;
                    OnPropertyChanged(nameof(Street));
                }
            }
        }

        public string Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        public DateTime Arrival
        {
            get => arrival;
            set
            {
                if (arrival == value)
                    return;

                arrival = value;
                OnPropertyChanged(nameof(Arrival));

                Departure = arrival.AddDays(2);
                CalcNightCount();
            }
        }

        public DateTime Departure
        {
            get => departure;
            set
            {
                if (departure == value)
                    return;

                departure = value;
                OnPropertyChanged(nameof(Departure));

                CalcNightCount();
            }
        }

        public int NightCount
        {
            get => nightCount;
            set
            {
                if (nightCount != value)
                {
                    nightCount = value;
                    OnPropertyChanged(nameof(NightCount));
                }
            }
        }

        public bool InfoSheet
        {
            get => infoSheet;
            set
            {
                if (infoSheet != value)
                {
                    infoSheet = value;
                    OnPropertyChanged(nameof(InfoSheet));
                }
            }
        }

        public bool CalendarEntry
        {
            get => calendarEntry;
            set
            {
                if (calendarEntry != value)
                {
                    calendarEntry = value;
                    OnPropertyChanged(nameof(CalendarEntry));
                }
            }
        }

        public int? InvoiceNumber
        {
            get => invoiceNumber;
            set
            {
                if (invoiceNumber != value)
                {
                    invoiceNumber = value;
                    OnPropertyChanged(nameof(InvoiceNumber));
                }
            }
        }

        public double? Total
        {
            get => total;
            set
            {
                if (total != value)
                {
                    total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public bool AgeCheck
        {
            get => ageCheck;
            set
            {
                if (ageCheck != value)
                {
                    ageCheck = value;
                    OnPropertyChanged(nameof(AgeCheck));
                }
            }
        }

        public bool Tent
        {
            get => tent;
            set
            {
                if (tent != value)
                {
                    tent = value;
                    OnPropertyChanged(nameof(Tent));
                }
            }
        }

        public double? Drinks
        {
            get => drinks;
            set
            {
                if (drinks != value)
                {
                    drinks = value;
                    OnPropertyChanged(nameof(Drinks));
                }
            }
        }

        public int? LastVisit
        {
            get => lastVisit;
            set
            {
                if (lastVisit != value)
                {
                    lastVisit = value;
                    OnPropertyChanged(nameof(LastVisit));
                }
            }
        }

        public DateTime Reserved
        {
            get => reserved;
            set
            {
                if (reserved != value)
                {
                    reserved = value;
                    OnPropertyChanged(nameof(Reserved));
                }
            }
        }

        public ContactValues.Contact ContactVia
        {
            get => contactVia;
            set
            {
                if (contactVia != value)
                {
                    contactVia = value;
                    OnPropertyChanged(nameof(ContactVia));
                }
            }
        }

        public string Mobile
        {
            get => mobile;
            set
            {
                if (mobile != value)
                {
                    mobile = value;
                    OnPropertyChanged(nameof(Mobile));
                }
            }
        }

        public string HomePhone
        {
            get => homePhone;
            set
            {
                if (homePhone != value)
                {
                    homePhone = value;
                    OnPropertyChanged(nameof(HomePhone));
                }
            }
        }

        public string EMail
        {
            get => eMail;
            set
            {
                if (eMail != value)
                {
                    eMail = value;
                    OnPropertyChanged(nameof(EMail));
                }
            }
        }

        public bool Offer
        {
            get => offer;
            set
            {
                if (offer != value)
                {
                    offer = value;
                    OnPropertyChanged(nameof(Offer));
                }
            }
        }

        public bool Canceled
        {
            get => canceled;
            set
            {
                if (canceled != value)
                {
                    canceled = value;
                    OnPropertyChanged(nameof(Canceled));
                }
            }
        }

        public string Note
        {
            get => note;
            set
            {
                if (note != value)
                {
                    note = value;
                    OnPropertyChanged(nameof(Note));
                }
            }
        }

        private bool isNoteOpen;
        public bool IsNoteOpen
        {
            get => isNoteOpen;
            set
            {
                if (isNoteOpen != value)
                {
                    isNoteOpen = value;
                    OnPropertyChanged(nameof(IsNoteOpen));
                }
            }
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
            if (InvoiceNumber is not null && invoiceNumber != 0)
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

        public Entry FullClone(int nextId)
        {
            return new Entry
            {
                Id = nextId,
                GroupName = this.GroupName,
                GuestCount = this.GuestCount,
                Salutation = this.Salutation,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Street = this.Street,
                Location = this.Location,
                Arrival = this.Arrival,
                Departure = this.Departure,
                NightCount = this.NightCount,
                infoSheet = this.InfoSheet,
                CalendarEntry = this.CalendarEntry,
                InvoiceNumber = this.InvoiceNumber,
                Total = this.Total,
                AgeCheck = this.AgeCheck,
                Tent = this.Tent,
                Drinks = this.Drinks,
                LastVisit = this.LastVisit,
                Reserved = this.Reserved,
                ContactVia = this.ContactVia,
                Mobile = this.Mobile,
                HomePhone = this.HomePhone,
                EMail = this.EMail,
                Offer = this.Offer,
                Canceled = this.Canceled,
                Note = this.Note
            };
        }

        public Entry PartialClone(int nextId)
        {
            return new Entry
            {
                Id = nextId,
                GroupName = this.GroupName,
                GuestCount = this.GuestCount,
                Salutation = this.Salutation,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Street = this.Street,
                Location = this.Location,
                Arrival = this.Arrival,
                Departure = this.Departure,
                NightCount = this.NightCount,
                ContactVia = this.ContactVia,
                Mobile = this.Mobile,
                HomePhone = this.HomePhone,
                EMail = this.EMail
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
