using Reservo.Classes;
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Dialog;
using Reservo.Services.Document;
using Reservo.Services.File;
using Reservo.Utils;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace Reservo.ViewModels
{
    class TenantViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IFileService _filesService;
        private readonly IDocumentService _documentsSerive;

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

        public TenantViewModel() : this(new DialogService(), new FileService(), new DocumentService()) { }

        public TenantViewModel(IDialogService dialog, IFileService files, IDocumentService documents)
        {
            Log.Information("MainViewModel initialisiert");

            _dialogService = dialog;
            _filesService = files;
            _documentsSerive = documents;
        }

        //Loads all Excel files from the database directory
        public async Task LoadWorkbooks()
        {
            Log.Information("Lade Excel-Dateien aus {Dir}", Paths.DatabasePath);

            var workbookFiles = Directory.GetFiles(Paths.DatabasePath, "*.xlsx").OrderByDescending(file => file).ToList();

            Workbooks.Clear();

            foreach (var file in workbookFiles)
            {
                Log.Information("Lade Workbook {File}", file);

                var result = await Task.Run(() => XLSX.LoadXLSX(file));

                var workbookViewModel = new WorkbookViewModel(file, _filesService, _documentsSerive, _dialogService)
                {
                    Entries = new ObservableCollection<Entry>(result.Entries)
                };

                foreach (var entry in workbookViewModel.Entries)
                {
                    entry.PropertyChanged += Entry_PropertyChanged;
                }

                Workbooks.Add(workbookViewModel);
            }

            SelectedWorkbook = Workbooks.FirstOrDefault();

            CheckAllEntryDates();
        }

        //Saves all loaded workbooks
        public void SaveWorkbooks()
        {
            Log.Information("Speichere Excel-Dateien in {Dir}", Paths.DatabasePath);

            if (Workbooks.Count == 0)
            {
                return;
            }

            foreach (var workbook in Workbooks)
            {
                Log.Information("Speichere Workbook {File}", workbook.FilePath);
                XLSX.SaveXLSX(workbook.FilePath, workbook.Entries);
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
