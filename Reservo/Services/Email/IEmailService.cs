using Reservo.Models;
using Reservo.Services.PathService;

namespace Reservo.Services.Email
{
    public interface IEmailService
    {
        void CreateEmail(Entry entry, string year, bool invoice, IPathService path);
    }
}
