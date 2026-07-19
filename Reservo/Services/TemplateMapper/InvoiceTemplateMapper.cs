using Reservo.Models;

namespace Reservo.Services.TemplateMapper
{
    public class InvoiceTemplateMapper : IInvoiceTemplateMapper
    {
        public Dictionary<string, string> Map(InvoiceData data, short year)
        {
            return new Dictionary<string, string>
            {
                ["{{Gruppe}}"] = data.Entry.GuestInfo.GroupName,
                ["{{Anrede}}"] = data.Entry.GuestInfo.Salutation,
                ["{{Vorname}}"] = data.Entry.GuestInfo.FirstName,
                ["{{Name}}"] = data.Entry.GuestInfo.LastName,
                ["{{Straße}}"] = data.Entry.GuestInfo.Street,
                ["{{Ort}}"] = data.Entry.GuestInfo.Location,
                ["{{Nummer}}"] = data.Entry.BillingInfo.InvoiceNumber.ToString(),
                ["{{Jahr}}"] = year.ToString(),
                ["{{Datum}}"] = string.Format("{0:d.MM.yyyy}", data.InvoiceDate),
                ["{{Nächte}}"] = data.Entry.StayInfo.NightCount.ToString(),
                ["{{Anreise}}"] = string.Format("{0:d.MM.yyyy}", data.Entry.StayInfo.Arrival),
                ["{{Abreise}}"] = string.Format("{0:d.MM.yyyy}", data.Entry.StayInfo.Departure),
                ["{{Preis1}}"] = data.Price1,
                ["{{Kinder}}"] = data.Children.ToString(),
                ["{{Junge}}"] = data.Youth.ToString(),
                ["{{Erwachsen}}"] = data.Adults.ToString(),
                ["{{Ab27}}"] = data.FromTwentySeven.ToString(),
                ["{{Preis2}}"] = data.Price2,
                ["{{Bis26}}"] = data.UpToTwentySix.ToString(),
                ["{{Preis3}}"] = data.Price3,
                ["{{Bis17}}"] = data.UpToSeventeen.ToString(),
                ["{{Preis4}}"] = data.Price4,
                ["{{Gäst}}"] = data.Guest.ToString(),
                ["{{Preis5}}"] = data.Price5,
                ["{{Heizm}}"] = data.Heater.ToString(),
                ["{{Preis6}}"] = data.Price6,
                ["{{Bett}}"] = data.Bedding.ToString(),
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
