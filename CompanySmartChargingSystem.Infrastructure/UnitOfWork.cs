using CompanySmartChargingSystem.Application.Services.IService;
using CompanySmartChargingSystem.Domain.ApplicationDbContext;
using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IBaseRepo<Customer> _customers;
        private IBaseRepo<Contract> _contracts;
        private IBaseRepo<Meter> _meters;
        private IBaseRepo<ChargeTransaction> _chargeTransactions;
        private IBaseRepo<User> _users;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IBaseRepo<Customer> Customers => _customers ??= new GenericRepository<Customer>(_context);
        public IBaseRepo<Contract> Contracts => _contracts ??= new GenericRepository<Contract>(_context);
        public IBaseRepo<Meter> Meters => _meters ??= new GenericRepository<Meter>(_context);
        public IBaseRepo<ChargeTransaction> ChargeTransactions => _chargeTransactions ??= new GenericRepository<ChargeTransaction>(_context);
        public IBaseRepo<User> Users => _users ??= new GenericRepository<User>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
} 