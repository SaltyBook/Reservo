using Reservo.Models;

namespace Reservo.Services.BillingFactor
{
    public class BillingFactorService : IBillingFactorService
    {
        private BillingFactorModel _current = new();

        public BillingFactorModel GetBillingFactors() => _current;

        public void SaveBillingFactors(BillingFactorModel model)
        {
            _current = model;
        }
    }
}
