using Reservo.Infrastructure;
using System.ComponentModel;

namespace Reservo.Models
{
    public class StayInfo : INotifyPropertyChanged
    {
        private DateTime arrival;
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

        private DateTime departure;
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

        private int nightCount;
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

        private bool tent;
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

        private bool ageCheck;
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
       
        private bool infoSheet;
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

        private bool calendarEntry;
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

        private DateTime reserved;
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

        private ContactValues.Contact contactVia;
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

        public StayInfo(DateTime arrival, DateTime departure, int nightCount, bool tent, bool ageCheck, bool infoSheet, bool calendarEntry, DateTime reserved, ContactValues.Contact contactVia)
        {
            this.arrival = arrival;
            this.departure = departure;
            this.nightCount = nightCount;
            this.tent = tent;
            this.ageCheck = ageCheck;
            this.infoSheet = infoSheet;
            this.calendarEntry = calendarEntry;
            this.reserved = reserved;
            this.contactVia = contactVia;
        }

        public StayInfo(DateTime arrival, DateTime departure, int nightCount, ContactValues.Contact contactVia)
        {
            this.arrival = arrival;
            this.departure = departure;
            this.nightCount = nightCount;
            this.contactVia = contactVia;
        }

        public StayInfo(DateTime arrival, DateTime departure, DateTime reserved)
        {
            this.arrival = arrival;
            this.departure = departure;
            this.reserved = reserved;
            CalcNightCount();
        }

        public StayInfo()
        {

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

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
