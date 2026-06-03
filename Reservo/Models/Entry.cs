using Reservo.Infrastructure;
using System.ComponentModel;

namespace Reservo.Models
{
    public class Entry : INotifyPropertyChanged
    {
        private int id;

        private bool offer;
        private bool canceled;
        private string note = string.Empty;

        public string Initials => $"{FirstName.FirstOrDefault()}{LastName.FirstOrDefault()}".ToUpper();

        public string FullName => $"{FirstName} {LastName}".Trim();

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

        public GuestInfo GuestInfo { get; } = new();
        public StayInfo StayInfo { get; } = new();
        public BillingInfo BillingInfo { get; } = new();

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
            this.offer = offer;
            this.canceled = canceled;
            this.note = note;

            //GuestInfo
            this.groupName = groupName;
            this.guestCount = guestCount;
            this.salutation = salutation;
            this.firstName = firstName;
            this.lastName = lastName;
            this.street = street;
            this.location = location;
            this.lastVisit = lastVisit;
            this.mobile = mobile;
            this.homePhone = homePhone;
            this.eMail = eMail;

            //StayInfo
            this.arrival = arrival;
            this.departure = departure;
            this.nightCount = nightCount;
            this.tent = tent;
            this.ageCheck = ageCheck;
            this.infoSheet = infoSheet;
            this.calendarEntry = calendarEntry;
            this.reserved = reserved;
            this.contactVia = contactVia;

            //BillingInfo
            this.invoiceNumber = invoiceNumber;
            this.total = total;
            this.drinks = drinks;
        }

        public Entry(int id, bool offer, bool canceled, string note, GuestInfo guestInfo, StayInfo stayInfo, BillingInfo billingInfo)
        {
            this.id = id;
            this.offer = offer;
            this.canceled = canceled;
            this.note = note;
            GuestInfo = guestInfo;
            StayInfo = stayInfo;
            BillingInfo = billingInfo;
        }

        public Entry(int id, GuestInfo guestInfo, StayInfo stayInfo)
        {
            this.id = id;
            GuestInfo = guestInfo;
            StayInfo = stayInfo;
        }

        public Entry(int id)
        {
            this.id = id;
            StayInfo = new StayInfo(DateTime.Now.Date, DateTime.Now.Date.AddDays(2), DateTime.Now.Date);
            CalcNightCount();
        }

        public Entry()
        {
            arrival = DateTime.Now.Date;
            departure = DateTime.Now.Date.AddDays(2);
            reserved = DateTime.Now.Date;
            CalcNightCount();
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
                Offer = this.Offer,
                Canceled = this.Canceled,
                Note = this.Note,

                //GuestInfo
                GroupName = this.GroupName,
                GuestCount = this.GuestCount,
                Salutation = this.Salutation,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Street = this.Street,
                Location = this.Location,
                LastVisit = this.LastVisit,
                Mobile = this.Mobile,
                HomePhone = this.HomePhone,
                EMail = this.EMail,

                //StayInfo
                Arrival = this.Arrival,
                Departure = this.Departure,
                NightCount = this.NightCount,
                Tent = this.Tent,
                infoSheet = this.InfoSheet,
                CalendarEntry = this.CalendarEntry,
                Reserved = this.Reserved,
                ContactVia = this.ContactVia,

                //BillingInfo
                InvoiceNumber = this.InvoiceNumber,
                Total = this.Total,
                AgeCheck = this.AgeCheck,
                Drinks = this.Drinks
            };
        }

        public Entry FullClone(int nextId, string test)
        {
            return new Entry
            (
                Id = nextId,
                Offer = this.Offer,
                Canceled = this.Canceled,
                Note = this.Note,
                new GuestInfo(this.GroupName, this.GuestCount, this.Salutation, this.FirstName, this.LastName, this.Street, this.Location, this.LastVisit, this.Mobile, this.HomePhone, this.EMail),
                new StayInfo(this.Arrival, this.Departure, this.NightCount, this.Tent, this.AgeCheck, this.InfoSheet, this.CalendarEntry, this.Reserved, this.ContactVia),
                new BillingInfo(this.InvoiceNumber, this.Total, this.Drinks)
            );
        }

        public Entry PartialClone(int nextId)
        {
            return new Entry
            {
                Id = nextId,

                //GuestInfo
                GroupName = this.GroupName,
                GuestCount = this.GuestCount,
                Salutation = this.Salutation,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Street = this.Street,
                Location = this.Location,
                Mobile = this.Mobile,
                HomePhone = this.HomePhone,
                EMail = this.EMail,

                //StayInfo
                Arrival = this.Arrival,
                Departure = this.Departure,
                NightCount = this.NightCount,
                ContactVia = this.ContactVia
            };
        }

        public Entry PartialClone(int nextId, string test)
        {
            return new Entry
           (
                Id = nextId,
                new GuestInfo(this.GroupName, this.GuestCount, this.Salutation, this.FirstName, this.LastName, this.Street, this.Location, this.Departure.Month, this.Mobile, this.HomePhone, this.EMail),
                new StayInfo(this.Arrival, this.Departure, this.NightCount, this.ContactVia)
           );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
