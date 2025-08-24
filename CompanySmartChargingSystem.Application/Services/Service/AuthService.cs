using CompanySmartChargingSystem.Application.DTOs;
using CompanySmartChargingSystem.Application.Services.IService;
using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Application.Services.Service
{

    public class AuthService(UserManager<User> _userManager, IMemoryCache memoryCache) : IAuthService
    {
        public async Task<UserInfo> getCurrentUser(string userId)
        {
            string cacheKey = $"UserInfo_{userId}";

            if (memoryCache.TryGetValue(cacheKey, out UserInfo cachedUserInfo))
            {
                return cachedUserInfo;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userInfo = new UserInfo
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList()
            };

            memoryCache.Set(cacheKey, userInfo, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });

            Thread.Sleep(5000);

            return userInfo;
        }
    }
}
