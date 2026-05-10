using DocumentFormat.OpenXml.Spreadsheet;
using Serilog;
using System.Collections.ObjectModel;

namespace Reservo.ViewModels
{
    class StatisticViewModel : BaseViewModel
    {
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

        public StatisticViewModel()
        {
            Log.Information("StatisticViewModel initialisiert");
        }

        public void FillStatisticData(ObservableCollection<WorkbookViewModel> workbooks)
        {
            Log.Information("Lade Statistik 2026");
            WorkbookViewModel workbook = workbooks.FirstOrDefault(x => x.Year == DateTime.Now.Year.ToString());
            var entries = workbook.Entries.Where(x => !x.Canceled);
            AllReservations = entries.Count();
            AllNights = entries.Sum(x => x.NightCount);
            AllGuestsNights = entries.Sum(x => x.NightCount * x.GuestCount);
            AllGuests = entries.Sum(x => x.GuestCount);
            AverageGroupSize = AllGuests / AllReservations;
            AverageNightCount = AllNights / AllReservations;
            Log.Information("Laden für Statistik 2026 abgeschlossen.");
        }
    }
}
