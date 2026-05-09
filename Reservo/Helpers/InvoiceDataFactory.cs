using Reservo.Models;

namespace Reservo.Helpers
{
    public class InvoiceDataFactory
    {
        public InvoiceData Create(Entry entry, List<TableEntry> entries)
        {
            if (entry is null || entries is null || entries.Count < 17)
                throw new ArgumentException("Invalid entries");

            return new InvoiceData
            {
                Entry = entry,
                Price1 = entries[0].Result,
                Children = entries[1].Quantity,
                Youth = entries[2].Quantity,
                Adults = entries[3].Quantity,
                FromTwentySeven = entries[4].Quantity,
                Price2 = entries[4].Result,
                UpToTwentySix = entries[5].Quantity,
                Price3 = entries[5].Result,
                UpToSeventeen = entries[6].Quantity,
                Price4 = entries[6].Result,
                Guest = entries[7].Quantity,
                Price5 = entries[7].Result,
                Heater = entries[8].Quantity,
                Price6 = entries[8].Result,
                Bed = entries[9].Quantity,
                Price7 = entries[9].Result,
                Laundry = entries[10].Quantity,
                Price8 = entries[10].Result,
                Tent = entries[11].Quantity,
                Price9 = entries[11].Result,
                Wood = entries[12].Quantity,
                Price10 = entries[12].Result,
                Price11 = entries[13].Result,
                Price12 = entries[14].Result,
                AdditionName = entries[15].Description,
                Price13 = entries[15].Result,
                TotalPrice = entries[16].Result,
            };
        }
    }
}
