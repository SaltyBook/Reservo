using Reservo.Models;

namespace Reservo.Helpers
{
    public static class DateAnalyse
    {
        // Returns a list of tuples representing overlapping or adjacent entries from the given collection.
        // The entries are first sorted by Arrival date. Then each consecutive pair is checked
        // for overlaps or adjacency (with a default tolerance of 2 days).
        public static List<(Entry, Entry)> FindOverlaps(IEnumerable<Entry> entries)
        {
            var overlaps = new List<(Entry, Entry)>();
            var sorted = entries.Where(x => x.Canceled == false).OrderBy(e => e.Arrival).ToList();

            for (int i = 0; i < sorted.Count - 1; i++)
            {
                var current = sorted[i];
                var next = sorted[i + 1];

                if (IsOverlappingOrAdjacent(current, next))
                {
                    overlaps.Add((current, next));
                }
            }

            return overlaps;
        }

        // Returns a list of tuples representing all entries from the collection that overlap or are adjacent
        // to the specified entry. The specified entry itself is excluded from comparison.
        // Uses a default tolerance of 2 days for adjacency checks.
        public static List<(Entry, Entry)> FindOverlapsForEntry(Entry entry, IEnumerable<Entry> entries)
        {
            var overlaps = new List<(Entry, Entry)>();
            var sorted = entries.Where(x => x.Canceled == false).OrderBy(e => e.Arrival).ToList();

            foreach (var other in sorted)
            {
                if (other == entry)
                    continue;

                if (IsOverlappingOrAdjacent(other, entry))
                {
                    overlaps.Add((other, entry));
                }
            }

            return overlaps;
        }

        // Determines if two entries overlap or are adjacent within a given tolerance in days (default 2).
        // Returns true if the arrival of one entry is before or equal to the departure of the other (with tolerance),
        // and the departure of one is after or equal to the arrival of the other (with tolerance).
        private static bool IsOverlappingOrAdjacent(Entry a, Entry b, int toleranceDays = 2)
        {
            return a.Arrival <= b.Departure.AddDays(toleranceDays)
                && a.Departure >= b.Arrival.AddDays(-toleranceDays);
        }
    }
}
