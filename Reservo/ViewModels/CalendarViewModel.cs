using PublicHoliday;
using Reservo.Commands;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reservo.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        private readonly IDictionary<DateTime, string> _publicHolidays;

        // ── Monat-Navigation ─────────────────────────────────────────
        private DateTime _currentMonth;
        public DateTime CurrentMonth
        {
            get => _currentMonth;
            private set
            {
                SetProperty(ref _currentMonth, value);
                OnPropertyChanged(nameof(MonthTitle));
                Rebuild();
            }
        }

        public string MonthTitle =>
            _currentMonth.ToString("MMMM yyyy");

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }

        // ── Kalendertage ─────────────────────────────────────────────
        public ObservableCollection<CalendarDay> Days { get; } = new();

        // ── Monats-Statistik ─────────────────────────────────────────
        private int _bookingCount;
        public int BookingCount
        {
            get => _bookingCount;
            private set => SetProperty(ref _bookingCount, value);
        }

        private int _occupiedDays;
        public int OccupiedDays
        {
            get => _occupiedDays;
            private set => SetProperty(ref _occupiedDays, value);
        }

        private int _occupancyPercent;
        public int OccupancyPercent
        {
            get => _occupancyPercent;
            private set => SetProperty(ref _occupancyPercent, value);
        }

        private int _freeWeeks;
        public int FreeWeeks
        {
            get => _freeWeeks;
            private set => SetProperty(ref _freeWeeks, value);
        }

        // ── Datenquelle ───────────────────────────────────────────────
        private ObservableCollection<WorkbookViewModel> _workbooks = new();

        public CalendarViewModel()
        {
            var calendar = new GermanPublicHoliday { State = GermanPublicHoliday.States.HE };
            _publicHolidays = calendar.PublicHolidayNames(DateTime.Now.Year)
                .Concat(calendar.PublicHolidayNames(DateTime.Now.Year + 1))
                .ToDictionary(x => x.Key, x => x.Value);

            _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            PreviousMonthCommand = new RelayCommand(_ => CurrentMonth = CurrentMonth.AddMonths(-1));
            NextMonthCommand = new RelayCommand(_ => CurrentMonth = CurrentMonth.AddMonths(1));
        }

        /// <summary>Wird vom MainViewModel aufgerufen wenn Workbooks geladen sind oder IsDirty.</summary>
        public void Refresh(ObservableCollection<WorkbookViewModel> workbooks)
        {
            _workbooks = workbooks;
            Rebuild();
            Log.Information("Kalender aktualisiert für {Month}", MonthTitle);
        }

        // ── Aufbau der Kalendertage ───────────────────────────────────
        private void Rebuild()
        {
            WorkbookViewModel workbook = _workbooks.First(x => x.Year == DateTime.Now.Year);

            Days.Clear();

            var firstDay = _currentMonth;
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            var daysInMonth = lastDay.Day;

            // Vortage (Montag = 0 ... Sonntag = 6)
            int leadingBlanks = ((int)firstDay.DayOfWeek + 6) % 7;

            var allEntries = workbook.Entries
                .Where(e => !e.Canceled)
                .ToList();

            // Farben pro Eintrag zuweisen (zyklisch)
            var colorKeys = new[] { "blue", "teal", "purple", "coral", "green", "amber" };
            var colorMap = new Dictionary<int, string>();
            int colorIdx = 0;
            foreach (var e in allEntries)
            {
                if (!colorMap.ContainsKey(e.Id))
                    colorMap[e.Id] = colorKeys[colorIdx++ % colorKeys.Length];
            }

            // Lückentage vom Vormonat
            var prevMonth = firstDay.AddMonths(-1);
            int prevDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
            for (int i = leadingBlanks - 1; i >= 0; i--)
                Days.Add(new CalendarDay(new DateTime(prevMonth.Year, prevMonth.Month, prevDays - i), isOtherMonth: true));

            // Tage des aktuellen Monats
            var occupiedSet = new HashSet<DateTime>();

            for (int d = 1; d <= daysInMonth; d++)
            {
                var date = new DateTime(firstDay.Year, firstDay.Month, d);

                bool isToday = date == DateTime.Today;
                bool isWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
                bool isHoliday = _publicHolidays.ContainsKey(date);
                string? holidayName = isHoliday ? _publicHolidays[date] : null;

                // Einträge die diesen Tag berühren (Anreise inkl., Abreise exkl.)
                var bars = allEntries
                    .Where(e => e.StayInfo.Arrival.Date <= date && e.StayInfo.Departure.Date > date)
                    .Select(e => new EntryBar
                    {
                        Label = e.GuestInfo.GroupName,
                        ColorKey = colorMap[e.Id],
                        IsStart = e.StayInfo.Arrival.Date == date,
                        IsEnd = e.StayInfo.Departure.Date == date.AddDays(1),
                    })
                    .ToList();

                if (bars.Count > 0)
                    occupiedSet.Add(date);

                Days.Add(new CalendarDay(date)
                {
                    IsToday = isToday,
                    IsWeekend = isWeekend,
                    IsHoliday = isHoliday,
                    HolidayName = holidayName,
                    Bars = new ObservableCollection<EntryBar>(bars),
                });
            }

            // Folgetage bis Zeile voll (Grid braucht Vielfaches von 7)
            int trailing = (7 - Days.Count % 7) % 7;
            var nextMonth = firstDay.AddMonths(1);
            for (int i = 1; i <= trailing; i++)
                Days.Add(new CalendarDay(new DateTime(nextMonth.Year, nextMonth.Month, i), isOtherMonth: true));

            // Statistik
            BookingCount = allEntries.Count(e => e.StayInfo.Arrival.Month == firstDay.Month &&
                                                      e.StayInfo.Arrival.Year == firstDay.Year);
            OccupiedDays = occupiedSet.Count;
            OccupancyPercent = (int)Math.Round(OccupiedDays / (double)daysInMonth * 100);
            FreeWeeks = CountFreeWeeks(firstDay, lastDay, occupiedSet);
        }

        private static int CountFreeWeeks(DateTime first, DateTime last, HashSet<DateTime> occupied)
        {
            int free = 0;
            var monday = first.AddDays(-(((int)first.DayOfWeek + 6) % 7));
            while (monday <= last)
            {
                bool weekFree = Enumerable.Range(0, 7)
                    .Select(i => monday.AddDays(i))
                    .Where(d => d.Month == first.Month)
                    .All(d => !occupied.Contains(d));

                if (weekFree) free++;
                monday = monday.AddDays(7);
            }
            return free;
        }
    }

    // ── Hilfsklassen ─────────────────────────────────────────────────

    public class CalendarDay
    {
        public DateTime Date { get; }
        public int DayNumber => Date.Day;
        public bool IsOtherMonth { get; }
        public bool IsToday { get; init; }
        public bool IsWeekend { get; init; }
        public bool IsHoliday { get; init; }
        public string? HolidayName { get; init; }
        public ObservableCollection<EntryBar> Bars { get; init; } = new();

        public CalendarDay(DateTime date, bool isOtherMonth = false)
        {
            Date = date;
            IsOtherMonth = isOtherMonth;
        }
    }

    public class EntryBar
    {
        public string Label { get; init; } = string.Empty;
        public string ColorKey { get; init; } = "blue";
        public bool IsStart { get; init; }
        public bool IsEnd { get; init; }
    }
}
