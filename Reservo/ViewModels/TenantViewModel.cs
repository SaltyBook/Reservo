#region Usings
using Reservo.Classes;
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Dialog;
using Reservo.Utils;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
#endregion

namespace Reservo.ViewModels
{
    class TenantViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;

        public ObservableCollection<WorkbookViewModel> Workbooks { get; } = new();

        private WorkbookViewModel? _selectedWorkbook;
        public WorkbookViewModel? SelectedWorkbook
        {
            get => _selectedWorkbook;
            set
            {
                if (SetProperty(ref _selectedWorkbook, value))
                {
                    RaiseCountProperties();
                }
            }
        }

        public int EntryCount => SelectedWorkbook?.Entries?.Count ?? 0;
        public int PastEntryCount => SelectedWorkbook?.Entries?.Count(e => e.Departure < DateTime.Today && !e.Canceled) ?? 0;
        public int CanceledEntryCount => SelectedWorkbook?.Entries?.Count(e => e.Canceled) ?? 0;

        public TenantViewModel() : this(new DialogService()) { }

        public TenantViewModel(IDialogService dialog)
        {
            Log.Information("MainViewModel initialisiert");

            _dialogService = dialog;
        }

        //Loads all Excel files from the database directory
        public async Task LoadWorkbooks()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Log.Information("Lade Excel-Dateien aus {Dir}", Paths.DatabasePath);

            var workbookFiles = Directory.GetFiles(Paths.DatabasePath, "*.xlsx").OrderByDescending(file => file).ToList();

            Workbooks.Clear();

            int loadedFiles = 0;
            int totalEntries = 0;
            var warnings = new List<string>();
            var errors = new List<string>();

            foreach (var file in workbookFiles)
            {
                Log.Information("Lade Workbook {File}", file);

                var result = await Task.Run(() => XLSX.LoadXLSX(file));

                if (result.HasErrors)
                {
                    errors.AddRange(result.Errors);
                    continue;
                }

                var workbookViewModel = new WorkbookViewModel(file)
                {
                    Entries = new ObservableCollection<Entry>(result.Entries)
                };

                foreach (var entry in workbookViewModel.Entries)
                {
                    entry.PropertyChanged += Entry_PropertyChanged;
                }

                Workbooks.Add(workbookViewModel);

                loadedFiles++;
                totalEntries += result.Entries.Count;
                warnings.AddRange(result.Warnings);
            }

            SelectedWorkbook = Workbooks.FirstOrDefault();

            CheckAllEntryDates();

            watch.Stop();

            Log.Information("Laden abgeschlossen: {LoadedFiles} Dateien, {TotalEntries} Einträge, {Warnings} Warnungen, {Errors} Fehler, Dauer {ElapsedMs} ms", loadedFiles, totalEntries, warnings.Count, errors.Count, watch.ElapsedMilliseconds);
        }

        //Saves all loaded workbooks
        public void SaveWorkbooks()
        {
            Log.Information("Speichere Excel-Dateien in {Dir}", Paths.DatabasePath);

            if (Workbooks.Count == 0)
            {
                _dialogService.ShowInfo("Speichern", "Es sind keine Tabellen zum Speichern geladen.");
                return;
            }

            foreach (var workbook in Workbooks)
            {
                try
                {
                    Log.Information("Speichere Workbook {File}", workbook.FilePath);
                    XLSX.SaveXLSX(workbook.FilePath, workbook.Entries);
                }
                catch (Exception ex)
                {
                    var fileName = Path.GetFileName(workbook.FilePath);
                    Log.Error(ex, "Fehler beim Speichern von Workbook {File}", workbook.FilePath);
                }
            }
        }

        #region Date
        //Checks all loaded workbooks for date overlaps
        private void CheckAllEntryDates()
        {
            foreach (var workbook in Workbooks)
            {
                var overlaps = DateAnalyse.FindOverlaps(workbook.Entries);

                if (overlaps.Count == 0)
                {
                    continue;
                }

                ShowOverlaps(overlaps, workbook);
            }
        }

        //Checks a modified entry for date overlaps
        private void CheckChangedEntryDate(Entry entry)
        {
            if (SelectedWorkbook is null)
            {
                return;
            }

            var overlaps = DateAnalyse.FindOverlapsForEntry(entry, SelectedWorkbook.Entries);

            if (overlaps.Count == 0)
            {
                return;
            }

            ShowOverlaps(overlaps, SelectedWorkbook);
        }

        // Logs any overlaps found and displays them to the user
        private void ShowOverlaps(List<(Entry, Entry)> overlaps, WorkbookViewModel workbook)
        {
            Log.Warning("Datumsüberschneidungen in Tabelle {DisplayName} gefunden: {Count}", workbook.DisplayName, overlaps.Count);
            foreach (var (firstEntry, secondEntry) in overlaps)
            {
                Log.Warning("Überschneidung: {IdA} ({FromA:d}-{ToA:d}) <-> {IdB} ({FromB:d}-{ToB:d})", firstEntry.Id, firstEntry.Arrival, firstEntry.Departure, secondEntry.Id, secondEntry.Arrival, secondEntry.Departure);

                _dialogService.ShowInfo("Überschneidung", $"{firstEntry.Id} {firstEntry.GroupName} Abreise {firstEntry.Departure:d}\n{secondEntry.Id} {secondEntry.GroupName} Anreise {secondEntry.Arrival:d}");
            }
        }
        #endregion

        //Responds to property changes in individual entries
        private void Entry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not Entry entry)
            {
                return;
            }

            if (e.PropertyName == nameof(Entry.Departure))
            {
                Log.Debug("Abreisedatum geändert (Id {Id})", entry.Id);
                CheckChangedEntryDate(entry);
            }

            if (e.PropertyName == nameof(Entry.Canceled))
            {
                Log.Information("Eintrag storniert geändert (Id {Id}, Canceled={Value})", entry.Id, entry.Canceled);
                RaiseCountProperties();
            }
        }

        //Updates all count properties that depend on the selection
        private void RaiseCountProperties()
        {
            OnPropertyChanged(nameof(EntryCount));
            OnPropertyChanged(nameof(PastEntryCount));
            OnPropertyChanged(nameof(CanceledEntryCount));
        }
    }
}
