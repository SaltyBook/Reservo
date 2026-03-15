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
        private readonly IFileService _fileService;
        private readonly IDocumentService _documentService;
        private readonly IDialogService _dialogService;

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

        public WorkbookViewModel(string filePath) : this(filePath, new FileService(), new DocumentService(), new DialogService()) { }

        public WorkbookViewModel(string filePath, IFileService files, IDocumentService documents, IDialogService dialog)
        {
            Log.Information("WorkbookViewModel initialisiert");

            FilePath = filePath;
            DisplayName = Path.GetFileNameWithoutExtension(filePath);
            Year = DisplayName.Substring(DisplayName.LastIndexOf('-') + 1);

            _fileService = files;
            _documentService = documents;
            _dialogService = dialog;

            AddEntryCommand = new RelayCommand(_ => AddEntry());
            DeleteEntryCommand = new RelayCommand(_ => DeleteEntry(), _ => SelectedEntry is not null);
            CreateReservationCommand = new RelayCommand(_ => CreateReservation(), _ => SelectedEntry is not null);
            CreateInvoiceCommand = new RelayCommand(_ => CreateInvoice(), _ => SelectedEntry is not null);
            CreateReservationEmailCommand = new RelayCommand(_ => CreateReservationEmail(), CanCreateEmail);
            CreateInvoiceEmailCommand = new RelayCommand(_ => CreateInvoiceEmail(), CanCreateEmail);
            OpenNoteCommand = new RelayCommand(_ => OpenNote(), _ => SelectedEntry is not null);

            if (!CheckFolderExisting())
                CreateFolder();
        }

        #region Commands

        //Adds a new entry with the next available ID and marks it as selected
        public void AddEntry()
        {
            var nextId = Entries.Count == 0 ? 1 : Entries.Max(e => e.Id) + 1;

            Log.Information("Eintrag hinzufügen (Id {Id})", nextId);

            var entry = new Entry { Id = nextId };

            Entries.Add(entry);
            SelectedEntry = entry;
        }

        //Deletes the currently selected entry
        public void DeleteEntry()
        {
            if (SelectedEntry == null)
            {
                return;
            }

            Log.Information("Eintrag löschen (Id {Id})", SelectedEntry.Id);

            Entries.Remove(SelectedEntry);
            SelectedEntry = null;
        }

        //Creates a reservation document if necessary and then opens it
        private void CreateReservation()
        {
            if (SelectedEntry == null)
            {
                return;
            }

            Log.Information("Reservierung erstellen (Id {Id})", SelectedEntry.Id);

            try
            {
                var entry = SelectedEntry;
                var documentPath = entry.GetReservationPath(Year);

                if (!_fileService.Exists(documentPath))
                {
                    Log.Debug("Reservierung existiert nicht, wird erstellt");
                    _documentService.CreateReservation(entry, Year);
                }

                _fileService.OpenFile(documentPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Erstellen der Reservierung (Id {Id})", SelectedEntry.Id);
                _dialogService.ShowError("Fehler", "Reservierung konnte nicht erstellt werden");
            }
        }

        //Creates an invoice document if necessary and then opens it
        private void CreateInvoice()
        {
            if (SelectedEntry == null)
            {
                return;
            }

            Log.Information("Rechnung erstellen (Id {Id})", SelectedEntry.Id);

            try
            {
                var entry = SelectedEntry;
                var documentPath = SelectedEntry.GetInvoicePath(Year);

                if (!_fileService.Exists(documentPath))
                {
                    Log.Debug("Rechnung existiert nicht, wird erstellt");
                    _documentService.CreateInvoice(entry, Year);
                }

                _fileService.OpenFile(documentPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Erstellen der Rechnung (Id {Id})", SelectedEntry.Id);
                _dialogService.ShowError("Fehler", "Rechnung konnte nicht erstellt werden");
            }
        }

        //Creates an email for the reservation, provided that an email address is available and the corresponding document exists
        private void CreateReservationEmail()
        {
            Log.Information("Erstelle EMail mit Reservierung Klick");

            if (SelectedEntry is null)
            {
                return;
            }
            var entry = SelectedEntry;

            if (String.IsNullOrEmpty(entry.EMail))
            {
                _dialogService.ShowInfo("Fehlende E-Mail", "E-Mail konnte nicht gefunden werden!");
                return;
            }

            var documentPath = entry.GetReservationPath(Year);

            if (_fileService.Exists(documentPath))
            {
                _documentService.CreateReservationMail(entry, Year);
            }
        }

        //Creates an email for the invoice, provided that an email address is available and the corresponding document exists
        private void CreateInvoiceEmail()
        {
            Log.Information("Erstelle EMail mit Rechnung Klick");

            if (SelectedEntry == null)
            {
                return;
            }

            var entry = SelectedEntry;

            if (String.IsNullOrWhiteSpace(entry.EMail))
            {
                _dialogService.ShowInfo("Fehlende E-Mail", "E-Mail konnte nicht gefunden werden!");
                return;
            }

            var documentPath = entry.GetInvoicePath(Year);

            if (_fileService.Exists(documentPath))
            {
                _documentService.CreateInvoiceMail(entry, Year);
            }
        }

        //Opens the note view of the currently selected entry
        private void OpenNote()
        {
            if (SelectedEntry is null)
            {
                return;
            }

            SelectedEntry.IsNoteOpen = true;
        }
        #endregion

        //Checks whether email access data is available
        private bool CanCreateEmail(object? _)
        {
            return CredentialsService.creds is not null;
        }

        //Checks whether the invoice folder for the current year exists
        private bool CheckFolderExisting()
        {
            if (!Directory.Exists(Path.Combine(Paths.ManagementPath, $"{Year}-Rechnung")))
            {
                return false;
            }
            return true;
        }

        //Creates the necessary folders for invoicing and reservations for the current year
        private void CreateFolder()
        {
            Directory.CreateDirectory(Path.Combine(Paths.ManagementPath, $"{Year}-Rechnung"));
            Directory.CreateDirectory(Path.Combine(Paths.ManagementPath, $"{Year}-Reservierung"));
        }
    }
}
