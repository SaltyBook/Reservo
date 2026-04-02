using Reservo.Models;

namespace Reservo.Services.Email
{
    public interface IEmailService
    {
        void CreateEmail(Entry entry, string year, bool invoice);
    }
}
