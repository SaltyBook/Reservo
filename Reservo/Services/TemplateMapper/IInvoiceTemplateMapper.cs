using Reservo.Models;

namespace Reservo.Services.TemplateMapper
{
    public interface IInvoiceTemplateMapper
    {
        Dictionary<string, string> Map(InvoiceData data, short year);
    }
}
