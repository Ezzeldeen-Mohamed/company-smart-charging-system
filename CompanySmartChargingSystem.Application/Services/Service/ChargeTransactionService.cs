using CompanySmartChargingSystem.Application.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Application.Services.Service
{
    public class ChargeTransactionService : IChargeTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChargeTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Domain.Entities.ChargeTransaction>> GetAllCharges()
        {
            return await _unitOfWork.ChargeTransactions.GetAllAsync();
        }

        public async Task ImportCharges(IEnumerable<Domain.Entities.ChargeTransaction> allCharges)
        {
            foreach(var entity in allCharges)
            {
                await _unitOfWork.ChargeTransactions.AddAsync(entity);
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
