using CompanySmartChargingSystem.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Application.Services.IService
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepo<Customer> Customers { get; }
        IBaseRepo<Contract> Contracts { get; }
        IBaseRepo<Meter> Meters { get; }
        IBaseRepo<ChargeTransaction> ChargeTransactions { get; }
        IBaseRepo<User> Users { get; }
        Task<int> SaveChangesAsync();
    }
} 