using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Reservo.Models
{
    public class EmployeeHours : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool IsSummaryRow { get; set; }

        private decimal _january;
        public decimal January
        {
            get => _january;
            set => SetField(ref _january, value);
        }

        private decimal _february;
        public decimal February
        {
            get => _february;
            set => SetField(ref _february, value);
        }

        private decimal _march;
        public decimal March
        {
            get => _march;
            set => SetField(ref _march, value);
        }

        private decimal _april;
        public decimal April
        {
            get => _april;
            set => SetField(ref _december, value);
        }

        private decimal _may;
        public decimal May
        {
            get => _may;
            set => SetField(ref _may, value);
        }

        private decimal _june;
        public decimal June
        {
            get => _june;
            set => SetField(ref _june, value);
        }

        private decimal _july;
        public decimal July
        {
            get => _july;
            set => SetField(ref _july, value);
        }

        private decimal _august;
        public decimal August
        {
            get => _august;
            set => SetField(ref _august, value);
        }

        private decimal _september;
        public decimal September
        {
            get => _september;
            set => SetField(ref _september, value);
        }

        private decimal _october;
        public decimal October
        {
            get => _october;
            set => SetField(ref _october, value);
        }

        private decimal _november;
        public decimal November
        {
            get => _november;
            set => SetField(ref _november, value);
        }

        private decimal _december;
        public decimal December
        {
            get => _december;
            set => SetField(ref _december, value);
        }

        public decimal TotalHours =>
            January + February + March + April +
            May + June + July + August +
            September + October + November + December;

        private void SetField(ref decimal field, decimal value, [CallerMemberName] string? propertyName = null)
        {
            if (field == value)
                return;

            field = value;

            OnPropertyChanged(propertyName);
            OnPropertyChanged(nameof(TotalHours));
        }
    }
}
