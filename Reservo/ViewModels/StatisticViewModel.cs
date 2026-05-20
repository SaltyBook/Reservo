using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using PublicHoliday;
using Reservo.Commands;
using Reservo.Models;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace Reservo.ViewModels
{
    class StatisticViewModel : BaseViewModel
    {
        private readonly string _employeeHoursPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employeehours.json");

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

        private IDictionary<DateTime, string> _publicHolidaysThisYear;
        public IDictionary<DateTime, string> PublicHolidaysThisYear
        {
            get { return _publicHolidaysThisYear; }
            set
            {
                _publicHolidaysThisYear = value;
                OnPropertyChanged();
            }
        }

        private IDictionary<DateTime, string> _publicHolidaysNextYear;
        public IDictionary<DateTime, string> PublicHolidaysNextYear
        {
            get { return _publicHolidaysNextYear; }
            set
            {
                _publicHolidaysNextYear = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EmployeeHours> Employees { get; set; } = new ObservableCollection<EmployeeHours>();

        public EmployeeHours HoursSummary { get; set; } = null;

        private EmployeeHours _selectedEmployee;
        public EmployeeHours SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddEmployeeCommand { get; }
        public ICommand RemoveEmployeeCommand { get; }

        public StatisticViewModel()
        {
            Log.Information("StatisticViewModel initialisiert");

            StatisticData = new ObservableCollection<StatisticData>();
            StatisticData.Add(new StatisticData());
            StatisticData[0].DisplayName = "Alle Jahre";

            var calendar = new GermanPublicHoliday { State = GermanPublicHoliday.States.HE };
            PublicHolidaysThisYear = calendar.PublicHolidayNames(DateTime.Now.Year);
            PublicHolidaysNextYear = calendar.PublicHolidayNames(DateTime.Now.Year + 1);

            Employees.CollectionChanged += Employees_CollectionChanged;

            AddEmployeeCommand = new RelayCommand(AddEmployee, null);
            RemoveEmployeeCommand = new RelayCommand(RemoveEmployee, null);

            LoadEmployeeHours();
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
                StatisticData[StatisticData.Count - 1].TotalAmount = (decimal)entries.Sum(x => x.Total);
                StatisticData[StatisticData.Count - 1].GroupCheckCount = entries.Where(x => x.AgeCheck).Count();
                StatisticData[StatisticData.Count - 1].AllCanceled = workbook.Entries.Where(x => x.Canceled).Count();

                StatisticData[0].AllReservations += StatisticData[StatisticData.Count - 1].AllReservations;
                StatisticData[0].AllNights += StatisticData[StatisticData.Count - 1].AllNights;
                StatisticData[0].AllGuestsNights += StatisticData[StatisticData.Count - 1].AllGuestsNights;
                StatisticData[0].AllGuests += StatisticData[StatisticData.Count - 1].AllGuests;
                StatisticData[0].TotalAmount += StatisticData[StatisticData.Count - 1].TotalAmount;
                StatisticData[0].GroupCheckCount += StatisticData[StatisticData.Count - 1].GroupCheckCount;
                StatisticData[0].AllCanceled += StatisticData[StatisticData.Count - 1].AllCanceled;

                var pieSerie = new PieSeries { StrokeThickness = 1, OutsideLabelFormat = "{1}", InsideLabelFormat = "{2:0}%", InsideLabelPosition = 0.7 };

                pieSerie.Slices.Add(new PieSlice("Belegungen", StatisticData[StatisticData.Count - 1].AllReservations)
                {
                    Fill = OxyColor.Parse("#008001")
                });

                pieSerie.Slices.Add(new PieSlice("Stornierungen", StatisticData[StatisticData.Count - 1].AllCanceled)
                {
                    Fill = OxyColor.Parse("#FE0000")
                });

                StatisticData[StatisticData.Count - 1].PieModel.Legends.Add(new Legend
                {
                    LegendPosition = LegendPosition.RightTop,
                    LegendPlacement = LegendPlacement.Outside,
                    LegendOrientation = LegendOrientation.Vertical,
                    FontSize = 12
                });

                StatisticData[StatisticData.Count - 1].PieModel.Series.Add(pieSerie);
            }

            StatisticData[0].AverageGroupSize += StatisticData[0].AllGuests / StatisticData[0].AllReservations;
            StatisticData[0].AverageNightCount += StatisticData[0].AllNights / StatisticData[0].AllReservations;

            var pieSeries = new PieSeries { StrokeThickness = 1, OutsideLabelFormat = "{1}", InsideLabelFormat = "{2:0}%", InsideLabelPosition = 0.7 };

            pieSeries.Slices.Add(new PieSlice("Belegungen", StatisticData[0].AllReservations)
            {
                Fill = OxyColor.Parse("#008001")
            });

            pieSeries.Slices.Add(new PieSlice("Stornierungen", StatisticData[0].AllCanceled)
            {
                Fill = OxyColor.Parse("#FE0000")
            });

            StatisticData[0].PieModel.Series.Add(pieSeries);

            SelectedStatisticData = StatisticData.First(x => x.DisplayName.EndsWith(DateTime.Now.Year.ToString()));

            Log.Information("Laden für Statistiken abgeschlossen.");
        }

        public void AddEmployee(object? obj)
        {
            Employees.Insert(Employees.Count - 1, new EmployeeHours()
            {
                Name = "Neuer Mitarbeiter"
            });
        }

        public void RemoveEmployee(object? obj)
        {
            if (SelectedEmployee != null)
                Employees.Remove(SelectedEmployee);
        }

        private void Employees_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EmployeeHours employee in e.NewItems)
                {
                    if(!employee.IsSummaryRow)
                        employee.PropertyChanged += Employee_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (EmployeeHours employee in e.OldItems)
                {
                    employee.PropertyChanged -= Employee_PropertyChanged;
                }
            }

            RecalculateSummary();
            SaveEmployeeHours();
        }

        private void Employee_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RecalculateSummary();
            SaveEmployeeHours();
        }

        private void RecalculateSummary()
        {
            HoursSummary = Employees.FirstOrDefault(x => x.IsSummaryRow);
            if (HoursSummary is not null)
            {
                var normalEmployees = Employees.Where(x => !x.IsSummaryRow);

                HoursSummary.January = normalEmployees.Sum(x => x.January);
                HoursSummary.February = normalEmployees.Sum(x => x.February);
                HoursSummary.March = normalEmployees.Sum(x => x.March);
                HoursSummary.April = normalEmployees.Sum(x => x.April);
                HoursSummary.May = normalEmployees.Sum(x => x.May);
                HoursSummary.June = normalEmployees.Sum(x => x.June);
                HoursSummary.July = normalEmployees.Sum(x => x.July);
                HoursSummary.August = normalEmployees.Sum(x => x.August);
                HoursSummary.September = normalEmployees.Sum(x => x.September);
                HoursSummary.October = normalEmployees.Sum(x => x.October);
                HoursSummary.November = normalEmployees.Sum(x => x.November);
                HoursSummary.December = normalEmployees.Sum(x => x.December);
                HoursSummary.TotalHours = normalEmployees.Sum(x => x.TotalHours);

                OnPropertyChanged(nameof(HoursSummary));
            }
        }

        private void UpdateSummary()
        {
            var summary = Employees.FirstOrDefault(x => x.IsSummaryRow);

            if (summary == null)
                return;

            var normalEmployees = Employees.Where(x => !x.IsSummaryRow);

            summary.January = normalEmployees.Sum(x => x.January);
            summary.February = normalEmployees.Sum(x => x.February);
            summary.March = normalEmployees.Sum(x => x.February);
            summary.April = normalEmployees.Sum(x => x.February);
            summary.May = normalEmployees.Sum(x => x.February);
            summary.June = normalEmployees.Sum(x => x.February);
            summary.July = normalEmployees.Sum(x => x.February);
            summary.August = normalEmployees.Sum(x => x.February);
            summary.September = normalEmployees.Sum(x => x.February);
            summary.October = normalEmployees.Sum(x => x.February);
            summary.November = normalEmployees.Sum(x => x.February);
            summary.December = normalEmployees.Sum(x => x.December);

            summary.TotalHours = normalEmployees.Sum(x => x.TotalHours);
        }

        private void SaveEmployeeHours()
        {
            var json = JsonSerializer.Serialize(Employees,
                new JsonSerializerOptions()
                {
                    WriteIndented = true
                });

            File.WriteAllText(_employeeHoursPath, json);
        }

        private void LoadEmployeeHours()
        {
            if (!File.Exists(_employeeHoursPath))
            {
                Employees.Add(new EmployeeHours
                {
                    Name = "Gesamt",
                    IsSummaryRow = true
                });
                return;
            }

            var json = File.ReadAllText(_employeeHoursPath);

            var employees = JsonSerializer.Deserialize<ObservableCollection<EmployeeHours>>(json);

            if (employees == null)
                return;

            Employees.Clear();

            foreach (var employee in employees)
            {
                Employees.Add(employee);
            }

            if (!Employees.Any(x => x.IsSummaryRow))
            {
                Employees.Add(new EmployeeHours
                {
                    Name = "Gesamt",
                    IsSummaryRow = true
                });
            }

            RecalculateSummary();
        }
    }
}
