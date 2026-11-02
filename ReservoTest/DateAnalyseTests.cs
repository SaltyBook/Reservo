using Reservo.Helpers;
using Reservo.Models;

namespace ReservoTest
{
    public class DateAnalyseTests
    {
        [Fact]
        public void FindOverlaps_FindsNestedBookingsThatAreNotAdjacentInSortOrder()
        {
            var start = DateTime.Today.AddYears(1);
            var entries = new[]
            {
                CreateEntry(1, start, start.AddDays(14)),
                CreateEntry(2, start.AddDays(2), start.AddDays(4)),
                CreateEntry(3, start.AddDays(8), start.AddDays(10)),
            };

            var overlaps = DateAnalyse.FindOverlaps(entries);

            Assert.Equal(2, overlaps.Count);
            Assert.Contains(overlaps, pair => pair.Item1.Id == 1 && pair.Item2.Id == 2);
            Assert.Contains(overlaps, pair => pair.Item1.Id == 1 && pair.Item2.Id == 3);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(3, false)]
        public void FindOverlaps_AppliesTwoDayAdjacencyTolerance(int daysAfterDeparture, bool expectedOverlap)
        {
            var start = DateTime.Today.AddYears(1);
            var first = CreateEntry(1, start, start.AddDays(2));
            var secondArrival = start.AddDays(2 + daysAfterDeparture);
            var second = CreateEntry(2, secondArrival, secondArrival.AddDays(1));

            var overlaps = DateAnalyse.FindOverlaps(new[] { first, second });

            Assert.Equal(expectedOverlap, overlaps.Count == 1);
        }

        [Fact]
        public void FindOverlaps_IgnoresCanceledEntries()
        {
            var start = DateTime.Today.AddYears(1);
            var active = CreateEntry(1, start, start.AddDays(4));
            var canceled = CreateEntry(2, start.AddDays(1), start.AddDays(2));
            canceled.Canceled = true;

            Assert.Empty(DateAnalyse.FindOverlaps(new[] { active, canceled }));
        }

        private static Entry CreateEntry(int id, DateTime arrival, DateTime departure)
        {
            var entry = new Entry(id);
            entry.StayInfo.Arrival = arrival;
            entry.StayInfo.Departure = departure;
            return entry;
        }
    }
}
