using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reservo.Models
{
    public class EmployeeHours : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<int, decimal> HoursPerMonth { get; set; } = new Dictionary<int, decimal>();

        public decimal January
        {
            get => GetMonthHours(1);
            set => SetMonthHours(1, value);
        }

        public decimal February
        {
            get => GetMonthHours(2);
            set => SetMonthHours(2, value);
        }

        public decimal March
        {
            get => GetMonthHours(3);
            set => SetMonthHours(3, value);
        }

        public decimal April
        {
            get => GetMonthHours(4);
            set => SetMonthHours(4, value);
        }

        public decimal May
        {
            get => GetMonthHours(5);
            set => SetMonthHours(5, value);
        }

        public decimal June
        {
            get => GetMonthHours(6);
            set => SetMonthHours(6, value);
        }

        public decimal July
        {
            get => GetMonthHours(7);
            set => SetMonthHours(7, value);
        }
        public decimal August
        {
            get => GetMonthHours(8);
            set => SetMonthHours(8, value);
        }

        public decimal September
        {
            get => GetMonthHours(9);
            set => SetMonthHours(9, value);
        }

        public decimal October
        {
            get => GetMonthHours(10);
            set => SetMonthHours(10, value);
        }

        public decimal November
        {
            get => GetMonthHours(11);
            set => SetMonthHours(11, value);
        }

        public decimal December
        {
            get => GetMonthHours(12);
            set => SetMonthHours(12, value);
        }

        public decimal TotalHours
        {
            get
            {
                return HoursPerMonth.Values.Sum();
            }
        }

        public decimal GetMonthHours(int month)
        {
            if (HoursPerMonth.ContainsKey(month))
                return HoursPerMonth[month];

            return 0;
        }

        public void SetMonthHours(int month, decimal value)
        {
            if (HoursPerMonth.ContainsKey(month))
                HoursPerMonth[month] = value;
            else
                HoursPerMonth.Add(month, value);

            OnPropertyChanged(nameof(HoursPerMonth));
            OnPropertyChanged(nameof(TotalHours));

            OnPropertyChanged(nameof(January));
            OnPropertyChanged(nameof(February));
            OnPropertyChanged(nameof(March));
            OnPropertyChanged(nameof(April));
            OnPropertyChanged(nameof(May));
            OnPropertyChanged(nameof(June));
            OnPropertyChanged(nameof(July));
            OnPropertyChanged(nameof(August));
            OnPropertyChanged(nameof(September));
            OnPropertyChanged(nameof(October));
            OnPropertyChanged(nameof(November));
            OnPropertyChanged(nameof(December));
        }
    }
}
