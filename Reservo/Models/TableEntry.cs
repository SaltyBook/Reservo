using System.ComponentModel;
using System.Globalization;

namespace Reservo.Models
{
    public class TableEntry : INotifyPropertyChanged
    {
        private int _quantity;
        private string _description;
        private string _factor;
        private bool _isTotalRow;
        public bool IsEditableDescription { get; set; }
        public bool IsEditableTotal { get; set; }

        public bool IsTotalRow
        {
            get => _isTotalRow;
            set
            {
                _isTotalRow = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(Result));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Factor
        {
            get => _factor;
            set
            {
                _factor = value;
                OnPropertyChanged(nameof(Factor));
                OnPropertyChanged(nameof(Result));
            }
        }

        public string Result
        {
            get
            {
                var culture = new CultureInfo("de-DE");

                if (IsTotalRow || IsEditableTotal)
                    return TotalValue?.ToString("N2", culture) + " €";

                if (decimal.TryParse(Factor, NumberStyles.Any, culture, out decimal multiplier))
                    return (Quantity * multiplier).ToString("N2", culture) + " €";

                return "Fehler";
            }
            set
            {
                if (IsEditableTotal)
                {
                    var culture = new CultureInfo("de-DE");

                    var cleaned = value?.Replace("€", "").Trim();

                    if (decimal.TryParse(cleaned, NumberStyles.Any, culture, out decimal parsed))
                    {
                        TotalValue = parsed;
                    }
                }
            }
        }

        private decimal? _totalValue = 0;
        public decimal? TotalValue
        {
            get => _totalValue;
            set
            {
                if (_totalValue != value)
                {
                    _totalValue = value;
                    OnPropertyChanged(nameof(Result));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
