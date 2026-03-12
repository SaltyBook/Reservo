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

        public InvoiceViewModel(Entry entry, IInvoiceService invoiceService, IWindowService windowService, string year)
        {
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
    }
}
