namespace Reservo.Models
{
    public class InvoiceItem
    {
        string description;
        string factor;

        public string Description { get => description; set => description = value; }
        public string Factor { get => factor; set => factor = value; }

        public static List<InvoiceItem> allInvoiceItems = new List<InvoiceItem>();

        public InvoiceItem(string description, string factor)
        {
            this.description = description;
            this.factor = factor;
            allInvoiceItems.Add(this);
        }
    }
}
