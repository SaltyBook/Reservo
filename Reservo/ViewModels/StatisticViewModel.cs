using PublicHoliday;
using Reservo.Models;
using Serilog;
using System.Collections.ObjectModel;

namespace Reservo.ViewModels
{
    class StatisticViewModel : BaseViewModel
    {
        private ObservableCollection<StatisticData> _statisticData;
        public ObservableCollection<StatisticData> StatisticData
        {
            get { return _statisticData; }
            set
            {
                _statisticData = value;
            }
        }

        private StatisticData _selectedStatisticData;
        public StatisticData SelectedStatisticData
        {
            get { return _selectedStatisticData; }
            set
            {
                _selectedStatisticData = value;
                OnPropertyChanged();
            }
        }

        private IDictionary<DateTime, string> _publicHolidays;
        public IDictionary<DateTime, string> PublicHolidays
        {
            get { return _publicHolidays; }
            set
            {
                _publicHolidays = value;
                OnPropertyChanged();
            }
        }

        public StatisticViewModel()
        {
            Log.Information("StatisticViewModel initialisiert");

            StatisticData = new ObservableCollection<StatisticData>();
            StatisticData.Add(new StatisticData());
            StatisticData[0].DisplayName = "Alle Jahre";

            var calendar = new GermanPublicHoliday { State = GermanPublicHoliday.States.HE };
            PublicHolidays = calendar.PublicHolidayNames(DateTime.Now.Year);
        }

        public void FillStatisticData(ObservableCollection<WorkbookViewModel> workbooks)
        {
            Log.Information("Lade Statistiken");

            foreach (WorkbookViewModel workbook in workbooks)
            {
                StatisticData.Add(new StatisticData());

                var entries = workbook.Entries.Where(x => !x.Canceled);

                StatisticData[StatisticData.Count - 1].DisplayName = workbook.DisplayName;
                StatisticData[StatisticData.Count - 1].AllReservations = entries.Count();
                StatisticData[StatisticData.Count - 1].AllNights = entries.Sum(x => x.NightCount);
                StatisticData[StatisticData.Count - 1].AllGuestsNights = entries.Sum(x => x.NightCount * x.GuestCount);
                StatisticData[StatisticData.Count - 1].AllGuests = entries.Sum(x => x.GuestCount);
                StatisticData[StatisticData.Count - 1].AverageGroupSize = StatisticData[StatisticData.Count - 1].AllGuests / StatisticData[StatisticData.Count - 1].AllReservations;
                StatisticData[StatisticData.Count - 1].AverageNightCount = StatisticData[StatisticData.Count - 1].AllNights / StatisticData[StatisticData.Count - 1].AllReservations;

                StatisticData[0].AllReservations += StatisticData[StatisticData.Count - 1].AllReservations;
                StatisticData[0].AllNights += StatisticData[StatisticData.Count - 1].AllNights;
                StatisticData[0].AllGuestsNights += StatisticData[StatisticData.Count - 1].AllGuestsNights;
                StatisticData[0].AllGuests += StatisticData[StatisticData.Count - 1].AllGuests;
            }

            StatisticData[0].AverageGroupSize += StatisticData[0].AllGuests / StatisticData[0].AllReservations;
            StatisticData[0].AverageNightCount += StatisticData[0].AllNights / StatisticData[0].AllReservations;

            SelectedStatisticData = StatisticData.First(x => x.DisplayName.EndsWith(DateTime.Now.Year.ToString()));

            Log.Information("Laden für Statistiken abgeschlossen.");
        }
    }
}
