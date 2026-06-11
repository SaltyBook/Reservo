using System.ComponentModel;

namespace Reservo.Models
{
    public class BillingFactorModel :INotifyPropertyChanged
    {
        private double basePrice = 0;
        public double BasePrice
        {
            get => basePrice;
            set
            {
                if (basePrice != value)
                {
                    basePrice = value;
                    OnPropertyChanged(nameof(BasePrice));
                }
            }
        }

        private double fromTwentySevenPrice = 0;
        public double FromTwentySevenPrice
        {
            get => fromTwentySevenPrice;
            set
            {
                if (fromTwentySevenPrice != value)
                {
                    fromTwentySevenPrice = value;
                    OnPropertyChanged(nameof(FromTwentySevenPrice));
                }
            }
        }

        private double upToTwentySixPrice = 0;
        public double UpToTwentySixPrice
        {
            get => upToTwentySixPrice;
            set
            {
                if (upToTwentySixPrice != value)
                {
                    upToTwentySixPrice = value;
                    OnPropertyChanged(nameof(UpToTwentySixPrice));
                }
            }
        }

        private double upToSeventeenPrice = 0;
        public double UpToSeventeenPrice
        {
            get => upToSeventeenPrice;
            set
            {
                if (upToSeventeenPrice != value)
                {
                    upToSeventeenPrice = value;
                    OnPropertyChanged(nameof(UpToSeventeenPrice));
                }
            }
        }

        private double guestPrice = 0;
        public double GuestPrice
        {
            get => guestPrice;
            set
            {
                if (guestPrice != value)
                {
                    guestPrice = value;
                    OnPropertyChanged(nameof(GuestPrice));
                }
            }
        }

        private double heaterPrice = 0;
        public double HeaterPrice
        {
            get => heaterPrice;
            set
            {
                if (heaterPrice != value)
                {
                    heaterPrice = value;
                    OnPropertyChanged(nameof(HeaterPrice));
                }
            }
        }

        private double beddingPrice = 0;
        public double BeddingPrice
        {
            get => beddingPrice;
            set
            {
                if (beddingPrice != value)
                {
                    beddingPrice = value;
                    OnPropertyChanged(nameof(BeddingPrice));
                }
            }
        }

        private double laundryPrice = 0;
        public double LaundryPrice
        {
            get => laundryPrice;
            set
            {
                if (laundryPrice != value)
                {
                    laundryPrice = value;
                    OnPropertyChanged(nameof(LaundryPrice));
                }
            }
        }

        private double tentPrice = 0;
        public double TentPrice
        {
            get => tentPrice;
            set
            {
                if (tentPrice != value)
                {
                    tentPrice = value;
                    OnPropertyChanged(nameof(TentPrice));
                }
            }
        }

        private double woodPrice = 0;
        public double WoodPrice
        {
            get => woodPrice;
            set
            {
                if (woodPrice != value)
                {
                    woodPrice = value;
                    OnPropertyChanged(nameof(WoodPrice));
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
