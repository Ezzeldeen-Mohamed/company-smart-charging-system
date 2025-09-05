using CompanySmartChargingSystem.Domain.ApplicationDbContext;
using CompanySmartChargingSystem.Infrastructure.Repositories.IRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Infrastructure.Repositories
{
    public class ContractBackgroundService : IContractBackgroundService
    {
        private readonly AppDbContext _context;

        public ContractBackgroundService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CloseInactiveContractsAsync()
        {
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

            var inactiveContracts = await _context.Contracts
                .Include(c => c.ChargeTransactions)
                .Where(c => !c.IsDeleted &&
                            (c.ChargeTransactions == null ||
                             !c.ChargeTransactions.Any() ||
                             c.ChargeTransactions.Max(t => t.Id) != 0 &&
                             c.ChargeTransactions.Max(t => t.Id) > 0 &&
                             c.ChargeTransactions.Max(t => t.Id) > 0 &&
                             c.ChargeTransactions.Max(t => t.Id) > 0) 
                ).ToListAsync();

            foreach (var contract in inactiveContracts)
            {
                contract.IsActive = false; 
            }

            await _context.SaveChangesAsync();
        }
    }
}
