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

        private decimal? total;
        public decimal? Total
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

        private decimal? drinks;
        public decimal? Drinks
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

        public BillingInfo(int? invoiceNumber, decimal? total, decimal? drinks)
        {
            this.invoiceNumber = invoiceNumber;
            this.total = total;
            this.drinks = drinks;
        }

        public BillingInfo()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
