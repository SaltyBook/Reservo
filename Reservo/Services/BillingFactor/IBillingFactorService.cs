using Reservo.Models;

namespace Reservo.Services.BillingFactor
{
    public interface IBillingFactorService
    {
        BillingFactorModel GetBillingFactors();
        void SaveBillingFactors(BillingFactorModel model);
    }
}
