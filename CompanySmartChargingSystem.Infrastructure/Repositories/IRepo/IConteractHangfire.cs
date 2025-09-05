using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Infrastructure.Repositories.IRepo
{
    public interface IContractBackgroundService
    {
        Task CloseInactiveContractsAsync();
    }
}
