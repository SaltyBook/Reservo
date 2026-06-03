using System.ComponentModel;

namespace Reservo.Models
{
    public class GuestInfo : INotifyPropertyChanged
    {
        private string groupName = string.Empty;
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

        private int guestCount;
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

        private string salutation = string.Empty;
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

        private string firstName = string.Empty;
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

        private string lastName = string.Empty;
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

        private string street = string.Empty;
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

        private string location = string.Empty;
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

        private int? lastVisit;
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

        private string mobile = string.Empty;
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
        private string homePhone = string.Empty;
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

        private string eMail = string.Empty;
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

        public GuestInfo(string groupName, int guestCount, string salutation, string firstName, string lastName, string street, string location, int? lastVisit, string mobile, string homePhone, string eMail)
        {
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
        }

        public GuestInfo()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
