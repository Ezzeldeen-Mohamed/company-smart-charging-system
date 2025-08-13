using CompanySmartChargingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Infrastructure.JWT
{
    public interface IJWT
    {
        public string GenerateToken(User user, IList<string> roles);
    }
}
