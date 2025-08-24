using CompanySmartChargingSystem.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Application.Services.IService
{
    public interface IAuthService
    {
        public Task<UserInfo> getCurrentUser(string userId);
    }
}
