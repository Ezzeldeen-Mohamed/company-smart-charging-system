using CompanySmartChargingSystem.Application.DTOs;
using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
        public RefreshToken GenerateRefreshToken();
        public RefreshToken CheckAndCreateNewRefreshToken(User user);
        public Task<AuthResponse> refreshToken(string refreshToken);
    }
}
