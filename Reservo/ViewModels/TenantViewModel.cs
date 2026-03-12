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
using System.Text.Json;
using System.Windows.Controls;

namespace Reservo.ViewModels
{
    class TenantViewModel : BaseViewModel
    {
        private readonly IDialogService _dialog;
        private readonly IFileService _files;
        private readonly IDocumentService _documents;

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

            _dialog = dialog;
            _files = files;
            _documents = documents;
        }

        public async Task LoadEntries()
        {
            var dir = Path.Combine(Paths.ManagementPath, "Datenbank");

            Log.Information("Lade Excel-Dateien aus {Dir}", dir);

            var files = Directory.GetFiles(dir, "*.xlsx").OrderByDescending(f => f).ToList();

            Workbooks.Clear();

            foreach (var file in files)
            {
                Log.Information("Lade Workbook {File}", file);

                var result = await Task.Run(() => XLSX.LoadXLSX(file));

                var vm = new WorkbookViewModel(file, new FileService(), new DocumentService(), new DialogService())
                {
                    Entries = new ObservableCollection<Entry>(result.Entries)
                };

                foreach (var entry in vm.Entries)
                    entry.PropertyChanged += Entry_PropertyChanged;

                Workbooks.Add(vm);
            }

            SelectedWorkbook = Workbooks.FirstOrDefault();

            CheckAllEntryDates();
        }

        public void SaveEntries()
        {
            var dir = Path.Combine(Paths.ManagementPath, "Datenbank");

            Log.Information("Speichere Excel-Dateien in {Dir}", dir);

            if (Workbooks.Count == 0)
                return;

            foreach (var workbook in Workbooks)
            {
                Log.Information("Speichere Workbook {File}", workbook.FilePath);

                XLSX.SaveXLSX(workbook.FilePath, workbook.Entries);
            }
        }

        #region Date
        private void CheckAllEntryDates()
        {
            foreach (var workbook in Workbooks)
            {
                var overlaps = DateAnalyse.FindOverlaps(workbook.Entries);

                if (overlaps.Count == 0)
                    return;

                ShowOverlaps(overlaps, workbook);
            }
        }

        private void CheckChangedEntryDate(Entry entry)
        {
            if (SelectedWorkbook is not null)
            {
                var overlaps = DateAnalyse.FindOverlapsForEntry(entry, SelectedWorkbook.Entries);

                if (overlaps.Count == 0)
                    return;

                ShowOverlaps(overlaps, SelectedWorkbook);
            }
        }

        private void ShowOverlaps(List<(Entry, Entry)> overlaps, WorkbookViewModel vm)
        {
            Log.Warning("Datumsüberschneidungen in Tabelle {DisplayName} gefunden: {Count}", vm.DisplayName, overlaps.Count);
            foreach (var (a, b) in overlaps)
            {
                Log.Warning("Überschneidung: {IdA} ({FromA:d}-{ToA:d}) <-> {IdB} ({FromB:d}-{ToB:d})", a.Id, a.Arrival, a.Departure, b.Id, b.Arrival, b.Departure);

                _dialog.ShowInfo("Überschneidung", $"{a.Id} {a.GroupName} Abreise {a.Departure:d}\n{b.Id} {b.GroupName} Anreise {b.Arrival:d}");
            }
        }
        #endregion

        #region ColumnOrder
        public void SaveColumnOrder(DataGrid grid)
        {
            var order = grid.Columns
                .Select(c => new ColumnOrderInfo
                {
                    Key = c.Header.ToString() ?? "",
                    DisplayIndex = c.DisplayIndex
                })
                .ToList();

            var json = JsonSerializer.Serialize(order);
            File.WriteAllText(Path.Combine(Paths.ResourcesPath, "columnOrder.json"), json);
        }

        public void LoadColumnOrder(DataGrid grid)
        {
            if (!File.Exists(Path.Combine(Paths.ResourcesPath, "columnOrder.json")))
                return;

            var json = File.ReadAllText(Path.Combine(Paths.ResourcesPath, "columnOrder.json"));
            var order = JsonSerializer.Deserialize<List<ColumnOrderInfo>>(json);

            if (order == null) return;

            var sorted = order.OrderBy(o => o.DisplayIndex).ToList();

            int index = 0;

            foreach (var info in sorted)
            {
                var column = grid.Columns.FirstOrDefault(c =>
                    c.Header.ToString() == info.Key);

                if (column != null)
                {
                    column.DisplayIndex = index;
                    index++;
                }
            }
        }
        #endregion

        private void Entry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not Entry entry)
                return;

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

        private void RaiseCountProperties()
        {
            OnPropertyChanged(nameof(EntryCount));
            OnPropertyChanged(nameof(PastEntryCount));
            OnPropertyChanged(nameof(CanceledEntryCount));
        }
    }
}
