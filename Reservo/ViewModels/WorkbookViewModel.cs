#region Usings
using Reservo.Commands;
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Credentials;
using Reservo.Services.Dialog;
using Reservo.Services.Document;
using Reservo.Services.Email;
using Reservo.Services.FileService;
using Reservo.Services.PathService;
using Serilog;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
#endregion

namespace Reservo.ViewModels
{
    public class WorkbookViewModel : BaseViewModel
    {
        private readonly IFileService _fileService;
        private readonly IDocumentService _documentService;
        private readonly IDialogService _dialogService;
        private readonly IEmailService _emailService;
        private readonly IPathService _pathService;

        public string FilePath { get; }
        public string DisplayName { get; }
        public short Year { get; }

        public bool IsUpdated { get; set; } = true;

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

        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                SetProperty(ref _visibility, value);
            }
        }

        #region Commands
        public ICommand AddEntryCommand { get; }
        public ICommand DeleteEntryCommand { get; }
        public ICommand CreateReservationCommand { get; }
        public ICommand CreateInvoiceCommand { get; }
        public ICommand CreateReservationEmailCommand { get; }
        public ICommand CreateInvoiceEmailCommand { get; }
        public ICommand CreateCalendarEntryCommand { get; }
        public ICommand OpenNoteCommand { get; }
        #endregion

        public WorkbookViewModel(string filePath) : this(filePath, new FileService(), new DocumentService(), new DialogService(), new EmailService(), new PathService()) { }

        public WorkbookViewModel(string filePath, IFileService files, IDocumentService documents, IDialogService dialog, IEmailService email, IPathService path)
        {
            Log.Information("WorkbookViewModel initialisiert");

            _fileService = files;
            _documentService = documents;
            _dialogService = dialog;
            _emailService = email;
            _pathService = path;

            FilePath = filePath;
            DisplayName = Path.GetFileNameWithoutExtension(filePath);

            try
            {
                Year = Convert.ToInt16(DisplayName.Substring(DisplayName.LastIndexOf('-') + 1));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Konvertieren des Jahres in der Datenbank: {Name}", DisplayName);
                _dialogService.ShowError("Fehler", "Jahr konnte aus Datenbank nicht konvertiert werden!");
                Year = 0;
            }

            AddEntryCommand = new RelayCommand(_ => AddEntry());
            DeleteEntryCommand = new RelayCommand(_ => DeleteEntry(), _ => SelectedEntry is not null);
            CreateReservationCommand = new RelayCommand(_ => CreateReservation(), _ => SelectedEntry is not null);
            CreateInvoiceCommand = new RelayCommand(_ => CreateInvoice(), _ => SelectedEntry is not null);
            CreateReservationEmailCommand = new RelayCommand(_ => CreateReservationEmail(), CanCreateEmail);
            CreateInvoiceEmailCommand = new RelayCommand(_ => CreateInvoiceEmail(), CanCreateEmail);
            CreateCalendarEntryCommand = new RelayCommand(_ => CreateCalendarEntry(), CanCreateCalendarEntry);
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

            var entry = new Entry(nextId);
            entry.Year = this.Year;

            Entries.Add(entry);
            SelectedEntry = entry;

            IsUpdated = true;
        }

        //Deletes the currently selected entry
        public void DeleteEntry()
        {
            if (SelectedEntry is null)
            {
                return;
            }

            Log.Information("Eintrag löschen (Id {Id})", SelectedEntry.Id);

            Entries.Remove(SelectedEntry);
            SelectedEntry = null;

            IsUpdated = true;
        }

        //Creates a reservation document if necessary and then opens it
        private void CreateReservation()
        {
            if (SelectedEntry is null)
            {
                return;
            }

            Log.Information("Reservierung erstellen (Id {Id})", SelectedEntry.Id);

            try
            {
                var entry = SelectedEntry;
                var documentPath = _pathService.GetReservationPath(entry);

                if (!_fileService.Exists(documentPath))
                {
                    Log.Debug("Reservierung existiert nicht, wird erstellt");
                    _documentService.CreateReservation(entry, _pathService);
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
            if (SelectedEntry is null)
            {
                return;
            }

            Log.Information("Rechnung erstellen (Id {Id})", SelectedEntry.Id);

            try
            {
                var entry = SelectedEntry;
                var documentPath = _pathService.GetInvoicePath(entry);

                if (!_fileService.Exists(documentPath))
                {
                    Log.Debug("Rechnung existiert nicht, wird erstellt");
                    var window = _documentService.CreateInvoice(entry);
                    window.ShowDialog();
                }

                if (_fileService.Exists(documentPath))
                {
                    _fileService.OpenFile(documentPath);
                }
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

            if (String.IsNullOrEmpty(entry.GuestInfo.EMail))
            {
                _dialogService.ShowInfo("Fehlende E-Mail", "E-Mail konnte nicht gefunden werden!");
                return;
            }

            var documentPath = _pathService.GetReservationPath(entry);

            if (!_fileService.Exists(documentPath))
            {
                _dialogService.ShowInfo("Fehlende Reservierung", $"Die Reservierung konnte nicht gefunden werden!\r\n{documentPath}");
                return;
            }

            _documentService.CreateReservationMail(entry, _emailService, _dialogService, _pathService);
        }

        //Creates an email for the invoice, provided that an email address is available and the corresponding document exists
        private void CreateInvoiceEmail()
        {
            Log.Information("Erstelle EMail mit Rechnung Klick");

            if (SelectedEntry is null)
            {
                return;
            }

            var entry = SelectedEntry;

            if (String.IsNullOrWhiteSpace(entry.GuestInfo.EMail))
            {
                _dialogService.ShowInfo("Fehlende E-Mail", "E-Mail konnte nicht gefunden werden!");
                return;
            }

            var documentPath = _pathService.GetInvoicePath(entry);

            if (!_fileService.Exists(documentPath))
            {
                _dialogService.ShowInfo("Fehlende Rechnung", $"Die Rechnung konnte nicht gefunden werden!\r\n{documentPath}");
                return;
            }

            _documentService.CreateInvoiceMail(entry, _emailService, _dialogService, _pathService);
        }

        private void CreateCalendarEntry()
        {
            Log.Information("Erstelle Kalendereintrag Klick");

            if (SelectedEntry is null)
            {
                return;
            }

            string calendarId = InternCredentials.CalendarID;

            string name = $"{SelectedEntry.GuestInfo.GroupName} - {SelectedEntry.GuestInfo.LastName}";

            int guestCount = SelectedEntry.GuestInfo.GuestCount;

            string start = SelectedEntry.StayInfo.Arrival.ToString("yyyyMMddTHHmmss"); // YYYYMMDDTHHMMSS
            string end = SelectedEntry.StayInfo.Departure.ToString("yyyyMMddTHHmmss");

            string url =
                "https://calendar.google.com/calendar/render?action=TEMPLATE" +
                "&src=" + Uri.EscapeDataString(calendarId) +
                $"&text=Reservierung {name}" +
                $"&details=Anzahl der Teilnehmer: {guestCount}" +
                $"&dates={start}/{end}";

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
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

        private bool CanCreateCalendarEntry(object? obj)
        {
            if (SelectedEntry is null || String.IsNullOrEmpty(InternCredentials.CalendarID))
            {
                return false;
            }

            return true;
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
