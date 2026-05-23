using System.ComponentModel;

namespace Reservo.Models
{
    public class BillingInfo : INotifyPropertyChanged
    {
        private int? invoiceNumber;
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

        private double? total;
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

        private double? drinks;
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

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
