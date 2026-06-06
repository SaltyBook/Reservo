#region Usings
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using PublicHoliday;
using Reservo.Commands;
using Reservo.Infrastructure;
using Reservo.Models;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
#endregion

namespace Reservo.ViewModels
{
    class StatisticViewModel : BaseViewModel
    {
        private readonly string _employeeHoursPath = Path.Combine(Paths.ResourcesPath, "employeehours.json");

        private ObservableCollection<StatisticData> _statisticData;
        public ObservableCollection<StatisticData> StatisticData
        {
            get { return _statisticData; }
            set
            {
                _statisticData = value;
                OnPropertyChanged();
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

        public ObservableCollection<EmployeeYear> EmployeeYears { get; set; } = new();

        private EmployeeYear _selectedEmployeeYear;

        public EmployeeYear SelectedEmployeeYear
        {
            get => _selectedEmployeeYear;
            set
            {
                _selectedEmployeeYear = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentEmployees));
            }
        }

        public ObservableCollection<EmployeeHours> CurrentEmployees
        {
            get
            {
                return SelectedEmployeeYear?.Employees ?? new ObservableCollection<EmployeeHours>();
            }
        }

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

            AddEmployeeCommand = new RelayCommand(AddEmployee, null);
            RemoveEmployeeCommand = new RelayCommand(RemoveEmployee, null);

            LoadEmployeeHours();
        }

        public void FillStatisticData(ObservableCollection<WorkbookViewModel> workbooks)
        {
            Log.Information("Lade Statistiken");

            foreach (WorkbookViewModel workbook in workbooks)
            {
                var current = new StatisticData { DisplayName = workbook.DisplayName };

                StatisticData.Add(current);

                var entries = workbook.Entries.Where(x => !x.Canceled);

                current.AllReservations = entries.Count();
                current.AllNights = entries.Sum(x => x.StayInfo.NightCount);
                current.AllGuestsNights = entries.Sum(x => x.StayInfo.NightCount * x.GuestInfo.GuestCount);
                current.AllGuests = entries.Sum(x => x.GuestInfo.GuestCount);
                current.AverageGroupSize = current.AllGuests / current.AllReservations;
                current.AverageNightCount = current.AllNights / current.AllReservations;
                current.TotalAmount = (decimal)entries.Sum(x => x.BillingInfo.Total);
                current.GroupCheckCount = entries.Where(x => x.StayInfo.AgeCheck).Count();
                current.AllCanceled = workbook.Entries.Where(x => x.Canceled).Count();

                StatisticData[0].AllReservations += current.AllReservations;
                StatisticData[0].AllNights += current.AllNights;
                StatisticData[0].AllGuestsNights += current.AllGuestsNights;
                StatisticData[0].AllGuests += current.AllGuests;
                StatisticData[0].TotalAmount += current.TotalAmount;
                StatisticData[0].GroupCheckCount += current.GroupCheckCount;
                StatisticData[0].AllCanceled += current.AllCanceled;

                var pieSerie = new PieSeries { StrokeThickness = 1, OutsideLabelFormat = "{1}", InsideLabelFormat = "{2:0}%", InsideLabelPosition = 0.7 };

                pieSerie.Slices.Add(new PieSlice("Belegungen", current.AllReservations)
                {
                    Fill = OxyColor.Parse("#008001")
                });

                pieSerie.Slices.Add(new PieSlice("Stornierungen", current.AllCanceled)
                {
                    Fill = OxyColor.Parse("#FE0000")
                });

                current.PieModel.Legends.Add(new Legend
                {
                    LegendPosition = LegendPosition.RightTop,
                    LegendPlacement = LegendPlacement.Outside,
                    LegendOrientation = LegendOrientation.Vertical,
                    FontSize = 12
                });

                current.PieModel.Series.Add(pieSerie);
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
            if (SelectedEmployeeYear == null)
                return;

            var employee = new EmployeeHours
            {
                Name = "Neuer Mitarbeiter"
            };

            employee.PropertyChanged += Employee_PropertyChanged;

            SelectedEmployeeYear.Employees.Insert(
                SelectedEmployeeYear.Employees.Count - 1,
                employee);

            RecalculateSummary(SelectedEmployeeYear);

            SaveEmployeeHours();
        }

        public void RemoveEmployee(object? obj)
        {
            if (SelectedEmployee == null ||
                SelectedEmployeeYear == null ||
                SelectedEmployee.IsSummaryRow)
                return;

            SelectedEmployee.PropertyChanged -= Employee_PropertyChanged;

            SelectedEmployeeYear.Employees.Remove(SelectedEmployee);

            RecalculateSummary(SelectedEmployeeYear);

            SaveEmployeeHours();
        }

        private void Employee_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (SelectedEmployeeYear != null)
            {
                RecalculateSummary(SelectedEmployeeYear);
            }

            SaveEmployeeHours();
        }

        private void RecalculateSummary(EmployeeYear year)
        {
            var summary = year.Employees.FirstOrDefault(x => x.IsSummaryRow);

            if (summary == null)
                return;

            var employees = year.Employees.Where(x => !x.IsSummaryRow);

            summary.January = employees.Sum(x => x.January);
            summary.February = employees.Sum(x => x.February);
            summary.March = employees.Sum(x => x.March);
            summary.April = employees.Sum(x => x.April);
            summary.May = employees.Sum(x => x.May);
            summary.June = employees.Sum(x => x.June);
            summary.July = employees.Sum(x => x.July);
            summary.August = employees.Sum(x => x.August);
            summary.September = employees.Sum(x => x.September);
            summary.October = employees.Sum(x => x.October);
            summary.November = employees.Sum(x => x.November);
            summary.December = employees.Sum(x => x.December);
        }

        private void SaveEmployeeHours()
        {
            try
            {
                var json = JsonSerializer.Serialize(EmployeeYears,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(_employeeHoursPath, json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Speichern der Mitarbeiterstunden");
            }
        }

        private void LoadEmployeeHours()
        {
            try
            {
                if (!File.Exists(_employeeHoursPath))
                {
                    CreateYear(DateTime.Now.Year);

                    SelectedEmployeeYear = EmployeeYears.First(x => x.Year == DateTime.Now.Year);

                    SaveEmployeeHours();
                    return;
                }

                var json = File.ReadAllText(_employeeHoursPath);

                var years = JsonSerializer.Deserialize<ObservableCollection<EmployeeYear>>(json);

                EmployeeYears.Clear();

                if (years != null)
                {
                    foreach (var year in years)
                    {
                        EnsureSummaryRow(year);

                        foreach (var employee in year.Employees)
                        {
                            if (!employee.IsSummaryRow)
                            {
                                employee.PropertyChanged += Employee_PropertyChanged;
                            }
                        }

                        EmployeeYears.Add(year);

                        RecalculateSummary(year);
                    }
                }

                EnsureCurrentYearExists();

                SelectedEmployeeYear = EmployeeYears.OrderByDescending(x => x.Year).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Laden der Mitarbeiterstunden");

                CreateYear(DateTime.Now.Year);

                SelectedEmployeeYear = EmployeeYears.First();
            }
        }

        private void EnsureCurrentYearExists()
        {
            var currentYear = DateTime.Now.Year;

            if (!EmployeeYears.Any(x => x.Year == currentYear))
            {
                CreateYear(currentYear);

                SaveEmployeeHours();
            }
        }

        private void CreateYear(int year)
        {
            var employeeYear = new EmployeeYear
            {
                Year = year
            };

            employeeYear.Employees.Add(new EmployeeHours
            {
                Name = "Gesamt",
                IsSummaryRow = true
            });

            EmployeeYears.Add(employeeYear);
        }

        private void EnsureSummaryRow(EmployeeYear year)
        {
            if (!year.Employees.Any(x => x.IsSummaryRow))
            {
                year.Employees.Add(new EmployeeHours
                {
                    Name = "Gesamt",
                    IsSummaryRow = true
                });
            }
        }
    }
}
