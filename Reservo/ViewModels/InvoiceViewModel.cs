#region Usings
using Reservo.Commands;
using Reservo.Models;
using Reservo.Services.Invoice;
using Reservo.Services.Window;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
#endregion

namespace Reservo.ViewModels
{
    public class InvoiceViewModel : BaseViewModel
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IWindowService _windowService;

        private const int DefaultItemCount = 16;

        public ObservableCollection<TableEntry> Items { get; }

        public Entry Entry { get; }

        public ICommand CreateInvoiceCommand { get; }

        public InvoiceViewModel(Entry entry, string year) : this(entry, year, new InvoiceService(), new WindowService()) { }

        public InvoiceViewModel(Entry entry, string year, IInvoiceService invoiceService, IWindowService windowService)
        {
            if (InvoiceItem.allInvoiceItems.Count == 0)
            {
                CreateInvoiceItems();
            }

            Entry = entry;
            _invoiceService = invoiceService;
            _windowService = windowService;

            Items = BuildItems();

            foreach (var item in Items)
            {
                item.PropertyChanged += OnItemChanged;
            }

            CreateInvoiceCommand = new RelayCommand(_ => CreateInvoice(year));

            RecalculateTotal();
        }

        //Creates and initializes the list of invoice table entries for a new invoice.
        private ObservableCollection<TableEntry> BuildItems()
        {
            var list = new ObservableCollection<TableEntry>();

            for (int i = 0; i < DefaultItemCount; i++)
            {
                var template = InvoiceItem.allInvoiceItems[i];

                var item = new TableEntry
                {
                    Quantity = 0,
                    Description = template.Description,
                    Factor = template.Factor
                };

                if (i > 12)
                {
                    item.IsEditableTotal = true;

                    if (i == 15)
                    {
                        item.IsEditableDescription = true;
                    }
                }

                list.Add(item);
            }

            list.Add(new TableEntry
            {
                IsTotalRow = true,
                Description = $"Endsumme (Preisliste {DateTime.Now.Year})"
            });

            return list;
        }

        private void OnItemChanged(object? sender, PropertyChangedEventArgs e)
        {
            RecalculateTotal();
        }

        //Calculates and updates the total value of all invoice items.
        private void RecalculateTotal()
        {
            double sum = Items
                .Where(x => !x.IsTotalRow)
                .Sum(items => items.IsEditableTotal
                    ? items.TotalValue ?? 0
                    : items.Quantity * double.Parse(items.Factor));

            Items.Last().TotalValue = sum;
        }

        //Creates the invoice using InvoiceService and then closes the window
        private void CreateInvoice(string year)
        {
            _invoiceService.CreateInvoice(Entry, Items.ToList(), year);
            _windowService.Close(this);
        }

        //Initializes the default list of invoice items with predefined descriptions and base prices.
        //These items represent the standard pricing structure used for invoice calculation, such as overnight stays, age-based pricing, additional services, and optional extras.
        private void CreateInvoiceItems()
        {
            _ = new InvoiceItem("Übernachtungen (Grundpreis bis 19 Personen)", "235,00");
            _ = new InvoiceItem("Kinder und Jugendliche bis 17 Jahre", "0");
            _ = new InvoiceItem("Junge Erwachsene bis 26 Jahre", "0");
            _ = new InvoiceItem("Erwachsene", "0");
            _ = new InvoiceItem("Übernachtungen Erwachsene ab 27 Jahre", "16,00");
            _ = new InvoiceItem("Übernachtungen Erwachsene bis 26 Jahre", "15,00");
            _ = new InvoiceItem("Übernachtungen Kinder und Jugendliche bis 17 Jahre", "11,00");
            _ = new InvoiceItem("Tagesgäste pro Tag", "7,00");
            _ = new InvoiceItem("Tage Heizkosten vom 1.10. – 30.4 und bei Bedarf", "40,00");
            _ = new InvoiceItem("Bettwäsche (Leihgebühren) ", "10,00");
            _ = new InvoiceItem("Maschinen Wäsche waschen", "4,00");
            _ = new InvoiceItem("Leihgebühr Faltzelt", "25,00");
            _ = new InvoiceItem("Kaminholz (kleine Kiste)", "5,00");
            _ = new InvoiceItem("Endreinigung (90 – 170 Euro) ", "0");
            _ = new InvoiceItem("Getränke laut beigefügter Rechnung (+20 € Lieferung) ", "0");
            _ = new InvoiceItem("Zusatz", "0");
        }
    }
}
