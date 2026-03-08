#region Usings
using Reservo.Commands;
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Credentials;
using Reservo.Services.Dialog;
using Reservo.Services.Document;
using Reservo.Services.File;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
#endregion

namespace Reservo.ViewModels
{
    public class WorkbookViewModel : BaseViewModel
    {
        private readonly IFileService _files;
        private readonly IDocumentService _documents;
        private readonly IDialogService _dialog;

        public string FilePath { get; }
        public string DisplayName { get; }
        public string Year { get; }

        private ObservableCollection<Entry> _entries = new();
        public ObservableCollection<Entry> Entries
        {
            get => _entries;
            set => SetProperty(ref _entries, value);
        }

        private Entry? _selectedEntry;
        public Entry? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                SetProperty(ref _selectedEntry, value);
            }
        }

        #region Commands
        public ICommand AddEntryCommand { get; }
        public ICommand DeleteEntryCommand { get; }
        public ICommand CreateReservationCommand { get; }
        public ICommand CreateInvoiceCommand { get; }
        public ICommand CreateReservationEmailCommand { get; }
        public ICommand CreateInvoiceEmailCommand { get; }
        public ICommand OpenNoteCommand { get; }
        #endregion

        public WorkbookViewModel(string filePath, IFileService files, IDocumentService documents, IDialogService dialog)
        {
            Log.Information("WorkbookViewModel initialisiert");

            FilePath = filePath;
            DisplayName = Path.GetFileNameWithoutExtension(filePath);
            Year = DisplayName.Substring(DisplayName.LastIndexOf('-') + 1);

            _files = files;
            _documents = documents;
            _dialog = dialog;

            AddEntryCommand = new RelayCommand(_ => AddEntry());
            DeleteEntryCommand = new RelayCommand(_ => DeleteEntry(), _ => SelectedEntry != null);
            CreateReservationCommand = new RelayCommand(_ => CreateReservation(), _ => SelectedEntry != null);
            CreateInvoiceCommand = new RelayCommand(_ => CreateInvoice(), _ => SelectedEntry != null);
            CreateReservationEmailCommand = new RelayCommand(_ => CreateReservationEmail(), CheckEMailCredentials);
            CreateInvoiceEmailCommand = new RelayCommand(_ => CreateInvoiceEmail(), CheckEMailCredentials);
            OpenNoteCommand = new RelayCommand(_ => OpenNote());

            if (!CheckFolderExisting())
                CreateFolder();
        }

        #region Commands
        public void AddEntry()
        {
            var nextId = Entries.Count == 0 ? 1 : Entries.Max(e => e.Id) + 1;

            Log.Information("Eintrag hinzufügen (Id {Id})", nextId);

            var entry = new Entry { Id = nextId };

            Entries.Add(entry);
            SelectedEntry = entry;
        }

        public void DeleteEntry()
        {
            if (SelectedEntry == null)
                return;

            Log.Information("Eintrag löschen (Id {Id})", SelectedEntry.Id);

            Entries.Remove(SelectedEntry);
            SelectedEntry = null;
        }

        private void CreateReservation()
        {
            if (SelectedEntry == null)
                return;

            Log.Information("Reservierung erstellen (Id {Id})", SelectedEntry.Id);

            try
            {
                var entry = SelectedEntry;
                var path = entry.GetReservationPath(Year);

                if (!_files.Exists(path))
                {
                    Log.Debug("Reservierung existiert nicht, wird erstellt");
                    _documents.CreateReservation(entry, Year);
                }

                _files.OpenFile(path);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Erstellen der Reservierung (Id {Id})", SelectedEntry.Id);
                _dialog.ShowError("Fehler", "Reservierung konnte nicht erstellt werden");
            }
        }

        private void CreateInvoice()
        {
            if (SelectedEntry == null)
                return;

            Log.Information("Rechnung erstellen (Id {Id})", SelectedEntry.Id);

            try
            {
                var entry = SelectedEntry;
                var path = SelectedEntry.GetInvoicePath(Year);

                if (!_files.Exists(path))
                {
                    Log.Debug("Rechnung existiert nicht, wird erstellt");
                    _documents.CreateInvoice(entry, Year);
                }

                _files.OpenFile(path);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Erstellen der Rechnung (Id {Id})", SelectedEntry.Id);
                _dialog.ShowError("Fehler", "Rechnung konnte nicht erstellt werden");
            }
        }

        private void CreateReservationEmail()
        {
            Log.Information("Erstelle EMail mit Reservierung Klick");

            if (SelectedEntry == null) return;

            var entry = SelectedEntry;

            if (String.IsNullOrEmpty(entry.EMail))
            {
                _dialog.ShowInfo("Fehlende E-Mail", "E-Mail konnte nicht gefunden werden!");
                return;
            }

            var docxPath = entry.GetReservationPath(Year);
            if (_files.Exists(docxPath))
                _documents.CreateReservationMail(entry, Year);
        }

        private void CreateInvoiceEmail()
        {
            Log.Information("Erstelle EMail mit Rechnung Klick");

            if (SelectedEntry == null) return;

            var entry = SelectedEntry;

            if (String.IsNullOrWhiteSpace(entry.EMail))
            {
                _dialog.ShowInfo("Fehlende E-Mail", "E-Mail konnte nicht gefunden werden!");
                return;
            }

            var docxPath = entry.GetInvoicePath(Year);
            if (_files.Exists(docxPath))
                _documents.CreateInvoiceMail(entry, Year);
        }

        private void OpenNote()
        {
            if (SelectedEntry == null)
                return;
            SelectedEntry.IsNoteOpen = true;
        }
        #endregion

        private bool CheckEMailCredentials(object? _)
        {
            return CredentialsService.creds is not null;
        }

        private bool CheckFolderExisting()
        {
            if (!Directory.Exists(Path.Combine(Paths.ManagementPath, $"{Year}-Rechnung")))
            {
                return false;
            }
            return true;
        }

        private void CreateFolder()
        {
            Directory.CreateDirectory(Path.Combine(Paths.ManagementPath, $"{Year}-Rechnung"));
            Directory.CreateDirectory(Path.Combine(Paths.ManagementPath, $"{Year}-Reservierung"));
        }
    }
}
