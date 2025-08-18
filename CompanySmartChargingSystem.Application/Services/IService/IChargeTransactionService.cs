using CompanySmartChargingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Application.Services.IService
{
    public interface IChargeTransactionService
    {
        public Task<IEnumerable<ChargeTransaction>> GetAllCharges();
        public Task ImportCharges(IEnumerable<ChargeTransaction> allCharges);
    }
}
