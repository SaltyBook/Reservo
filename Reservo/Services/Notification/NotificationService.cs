using Notification.Core;
using Notification.Wpf;
using Reservo.Models;
using Reservo.ViewModels;
using Serilog;

namespace Reservo.Services.Notification
{
    public static class NotificationService
    {
        private static readonly NotificationManager _manager = new();

        private const int UpcomingDays = 7;

        /// Checks all workbooks for entries in the next 7 days
        /// and displays a Windows toast notification for each one.
        public static void ShowUpcomingArrivals(WorkbookViewModel workbook)
        {
            var today = DateTime.Today;
            var deadline = today.AddDays(UpcomingDays);

            var upcoming = workbook.Entries
                .Where(e => !e.Canceled
                         && e.StayInfo.Arrival.Date >= today
                         && e.StayInfo.Arrival.Date <= deadline)
                .OrderBy(e => e.StayInfo.Arrival.Date)
                .ToList();

            if (upcoming.Count == 0)
            {
                Log.Information("Keine bevorstehenden Anreisen in den nächsten {Days} Tagen", UpcomingDays);
                return;
            }

            Log.Information("{Count} bevorstehende Anreise(n) gefunden – sende Benachrichtigungen", upcoming.Count);

            foreach (var entry in upcoming)
                ShowNotification(entry, today);
        }

        private static void ShowNotification(Entry entry, DateTime today)
        {
            int daysUntil = (entry.StayInfo.Arrival.Date - today).Days;

            string when = daysUntil == 0 ? "Heute"
                        : daysUntil == 1 ? "Morgen"
                        : $"In {daysUntil} Tagen";

            string nights = entry.StayInfo.NightCount == 1 ? "1 Nacht" : $"{entry.StayInfo.NightCount} Nächte";
            string guests = entry.GuestInfo.GuestCount == 1 ? "1 Gast" : $"{entry.GuestInfo.GuestCount} Gäste";
            string arrival = entry.StayInfo.Arrival.ToString("dddd, dd. MMMM");

            var content = new NotificationContent
            {
                Title = $"Anreise {when} – {entry.GuestInfo.GroupName}",
                Message = $"{entry.GuestInfo.Salutation} {entry.GuestInfo.FirstName} {entry.GuestInfo.LastName}\n" +
                          $"{arrival}  ·  {nights}  ·  {guests}",
                Type = daysUntil == 0 ? NotificationType.Warning
                                         : NotificationType.Information,
            };

            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => _manager.Show(content));

                Log.Debug("Benachrichtigung gesendet für Eintrag {Id} ({GroupName})", entry.Id, entry.GuestInfo.GroupName);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Benachrichtigung konnte nicht angezeigt werden für Eintrag {Id}", entry.Id);
            }
        }
    }
}
