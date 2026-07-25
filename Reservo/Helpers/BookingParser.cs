using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.ViewModels;
using Serilog;
using System.Globalization;

namespace Reservo.Helpers
{
    public static class BookingParser
    {
        public static bool TryParse(string text, WorkbookViewModel selectedWorkbook, out Entry entry)
        {
            try
            {
                var fields = ParseFields(text);

                GuestInfo guest = ReadGuestInfo(fields);

                StayInfo stay = ReadStayInfo(fields);

                entry = CreateEntry(selectedWorkbook, guest, stay);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                entry = null;
                return false;
            }
            return true;
        }

        private static Dictionary<string, string> ParseFields(string text)
        {
            text = text.Replace("\t", "").Replace("Reisedaten", "").Replace("Absender", "").Trim();
            string[] lines = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < lines.Length - 1; i += 2)
            {
                if (String.Equals(lines[i], "Anreise") && String.Equals(lines[i + 1], "Abreise"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "Abreise") && String.Equals(lines[i + 1], "Teilnehmer"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "Teilnehmer") && String.Equals(lines[i + 1], "davon"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "davon") && String.Equals(lines[i + 1], "Verpflegung"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "Verpflegung") && String.Equals(lines[i + 1], "Organisation"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "Organisation") && String.Equals(lines[i + 1], "Name"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "Name") && String.Equals(lines[i + 1], "Straße"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "Straße") && String.Equals(lines[i + 1], "Ort"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "Ort") && String.Equals(lines[i + 1], "Telefon"))
                {
                    i--;
                    continue;
                }
                if (String.Equals(lines[i], "Telefon") && String.Equals(lines[i + 1], "E-Mail"))
                {
                    i--;
                    continue;
                }
                result[lines[i]] = lines[i + 1];
            }

            return result;
        }

        private static GuestInfo ReadGuestInfo(Dictionary<string, string> fields)
        {
            string organisation = TryGetField(fields, "Organisation");
            int teilnehmer = TryGetInt(fields, "Teilnehmer");
            (string vorName, string nachName) = TryGetName(fields);
            string straße = TryGetField(fields, "Straße");
            string ort = TryGetField(fields, "Ort");
            string telefon = TryGetField(fields, "Telefon");
            string email = TryGetField(fields, "E-Mail");

            return new GuestInfo(organisation, teilnehmer, "", vorName, nachName, straße, ort, null, telefon, "", email);
        }

        private static StayInfo ReadStayInfo(Dictionary<string, string> fields)
        {
            if (!TryGetDate(fields, "Anreise", out var anreise))
                anreise = DateTime.Now;
            if (!TryGetDate(fields, "Abreise", out var abreise))
                anreise = DateTime.Now;

            TimeSpan ts = new TimeSpan(12, 0, 0);

            anreise = anreise.Date + ts;

            abreise = abreise.Date + ts;

            return new StayInfo(anreise, abreise, DateTime.Now, ContactValues.Contact.Gruppenhaus);
        }

        private static Entry CreateEntry(WorkbookViewModel selectedWorkbook, GuestInfo guest, StayInfo stay)
        {
            var nextId = selectedWorkbook.Entries.Count == 0 ? 1 : selectedWorkbook.Entries.Max(e => e.Id) + 1;

            return new Entry(nextId, guest, stay);
        }

        private static string TryGetField(
            IReadOnlyDictionary<string, string> fields,
            string key,
            string defaultValue = "")
        {
            return fields.TryGetValue(key, out var value)
                ? value
                : defaultValue;
        }

        private static bool TryGetDate(
            IReadOnlyDictionary<string, string> fields,
            string key,
            out DateTime date)
        {
            date = default;

            return DateTime.TryParseExact(
                TryGetField(fields, key),
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
        }

        private static int TryGetInt(
            IReadOnlyDictionary<string, string> fields,
            string key,
            int defaultValue = 0)
        {
            return int.TryParse(TryGetField(fields, key), out var value)
                ? value
                : defaultValue;
        }

        private static (string Vorname, string Nachname) TryGetName(
                        IReadOnlyDictionary<string, string> fields)
        {
            var parts = TryGetField(fields, "Name")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return (
                parts.FirstOrDefault() ?? "",
                parts.Length > 1
                    ? string.Join(" ", parts.Skip(1))
                    : ""
            );
        }
    }
}
