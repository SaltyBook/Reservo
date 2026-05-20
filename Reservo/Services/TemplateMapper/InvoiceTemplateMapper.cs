using Reservo.Models;

namespace Reservo.Services.TemplateMapper
{
    public class InvoiceTemplateMapper : IInvoiceTemplateMapper
    {
        public Dictionary<string, string> Map(InvoiceData data, string year)
        {
            return new Dictionary<string, string>
            {
                ["{{Gruppe}}"] = data.Entry.GroupName,
                ["{{Anrede}}"] = data.Entry.Salutation,
                ["{{Vorname}}"] = data.Entry.FirstName,
                ["{{Name}}"] = data.Entry.LastName,
                ["{{Straße}}"] = data.Entry.Street,
                ["{{Ort}}"] = data.Entry.Location,
                ["{{Nummer}}"] = data.Entry.InvoiceNumber.ToString(),
                ["{{Jahr}}"] = year,
                ["{{Datum}}"] = string.Format("{0:d.MM.yyyy}", data.InvoiceDate),
                ["{{Nächte}}"] = data.Entry.NightCount.ToString(),
                ["{{Anreise}}"] = string.Format("{0:d.MM.yyyy}", data.Entry.Arrival),
                ["{{Abreise}}"] = string.Format("{0:d.MM.yyyy}", data.Entry.Departure),
                ["{{Preis1}}"] = data.Price1,
                ["{{Kinder}}"] = data.Children.ToString(),
                ["{{Junge}}"] = data.Youth.ToString(),
                ["{{Erwachsen}}"] = data.Adults.ToString(),
                ["{{Ab27}}"] = data.FromTwentySeven.ToString(),
                ["{{Preis2}}"] = data.Price2,
                ["{{Bis26}}"] = data.UpToSeventeen.ToString(),
                ["{{Preis3}}"] = data.Price3,
                ["{{Bis17}}"] = data.UpToTwentySix.ToString(),
                ["{{Preis4}}"] = data.Price4,
                ["{{Gäst}}"] = data.Guest.ToString(),
                ["{{Preis5}}"] = data.Price5,
                ["{{Heizm}}"] = data.Heater.ToString(),
                ["{{Preis6}}"] = data.Price6,
                ["{{Bett}}"] = data.Bed.ToString(),
                ["{{Preis7}}"] = data.Price7,
                ["{{Wasch}}"] = data.Laundry.ToString(),
                ["{{Preis8}}"] = data.Price8,
                ["{{Leih}}"] = data.Tent.ToString(),
                ["{{Preis9}}"] = data.Price9,
                ["{{Holz}}"] = data.Wood.ToString(),
                ["{{Preis10}}"] = data.Price10,
                ["{{Preis11}}"] = data.Price11,
                ["{{Preis12}}"] = data.Price12,
                ["{{Zusatz}}"] = data.AdditionName,
                ["{{Preis13}}"] = data.Price13,
                ["{{Total}}"] = data.TotalPrice,
            };
        }
    }
}
