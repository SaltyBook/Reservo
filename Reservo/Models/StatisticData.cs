using OxyPlot;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reservo.Models
{
    public class StatisticData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _displayName = String.Empty;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        private int _allReservations;
        public int AllReservations
        {
            get { return _allReservations; }
            set { _allReservations = value; }
        }

        private int _allNights;
        public int AllNights
        {
            get { return _allNights; }
            set { _allNights = value; }
        }

        private int _allGuestsNights;
        public int AllGuestsNights
        {
            get { return _allGuestsNights; }
            set { _allGuestsNights = value; }
        }

        private int _allGuests;
        public int AllGuests
        {
            get { return _allGuests; }
            set { _allGuests = value; }
        }

        private int _averageGroupSize;
        public int AverageGroupSize
        {
            get { return _averageGroupSize; }
            set { _averageGroupSize = value; }
        }

        private int _averageNightCount;
        public int AverageNightCount
        {
            get { return _averageNightCount; }
            set { _averageNightCount = value; }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set { _totalAmount = value; }
        }

        private int _groupCheckCount;
        public int GroupCheckCount
        {
            get { return _groupCheckCount; }
            set { _groupCheckCount = value; }
        }

        private int _allCanceled;
        public int AllCanceled
        {
            get { return _allCanceled; }
            set { _allCanceled = value; }
        }

        private PlotModel _pieModel = new PlotModel();
        public PlotModel PieModel
        {
            get { return _pieModel; }
            set { _pieModel = value; }
        }
    }
}
