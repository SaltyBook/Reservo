using System.ComponentModel;

namespace Reservo.Models
{
    public class Entry : INotifyPropertyChanged
    {
        private int id;
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

        private bool offer;
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

        private bool canceled;
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

        private string note = string.Empty;
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

        public GuestInfo GuestInfo { get; } = new();
        public StayInfo StayInfo { get; } = new();
        public BillingInfo BillingInfo { get; } = new();

        public string Initials => $"{GuestInfo.FirstName.FirstOrDefault()}{GuestInfo.LastName.FirstOrDefault()}".ToUpper();
        public string FullName => $"{GuestInfo.FirstName} {GuestInfo.LastName}".Trim();
        public short Year { get; set; }

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
            TimeSpan ts = new TimeSpan(12, 0, 0);
            DateTime Today = DateTime.Now.Date;
            Today = Today.Date + ts;
            StayInfo = new StayInfo(Today, Today.AddDays(2), Today);
        }

        public Entry FullClone(int nextId)
        {
            return new Entry
            (
                nextId,
                this.Offer,
                this.Canceled,
                this.Note,
                new GuestInfo(this.GuestInfo.GroupName, this.GuestInfo.GuestCount, this.GuestInfo.Salutation, this.GuestInfo.FirstName, this.GuestInfo.LastName, this.GuestInfo.Street, this.GuestInfo.Location, this.GuestInfo.LastVisit, this.GuestInfo.Mobile, this.GuestInfo.HomePhone, this.GuestInfo.EMail),
                new StayInfo(this.StayInfo.Arrival, this.StayInfo.Departure, this.StayInfo.NightCount, this.StayInfo.Tent, this.StayInfo.AgeCheck, this.StayInfo.InfoSheet, this.StayInfo.CalendarEntry, this.StayInfo.Reserved, this.StayInfo.ContactVia),
                new BillingInfo(this.BillingInfo.InvoiceNumber, this.BillingInfo.Total, this.BillingInfo.Drinks)
            );
        }

        public Entry PartialClone(int nextId)
        {
            return new Entry
           (
                nextId,
                new GuestInfo(this.GuestInfo.GroupName, this.GuestInfo.GuestCount, this.GuestInfo.Salutation, this.GuestInfo.FirstName, this.GuestInfo.LastName, this.GuestInfo.Street, this.GuestInfo.Location, this.StayInfo.Departure.Month, this.GuestInfo.Mobile, this.GuestInfo.HomePhone, this.GuestInfo.EMail),
                new StayInfo(this.StayInfo.Arrival, this.StayInfo.Departure, this.StayInfo.NightCount, this.StayInfo.ContactVia)
           );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
