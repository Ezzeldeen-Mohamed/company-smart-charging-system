using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Domain.Entities
{
    public class User : IdentityUser     
    {
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
