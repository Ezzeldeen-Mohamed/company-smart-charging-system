using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Infrastructure.JWT
{
    public class JWTRepo : IJWT
    {
        private readonly JWTConfig jwtConfig;
        public JWTRepo(IOptions<JWTConfig> jwtConfig)
        {
            this.jwtConfig = jwtConfig.Value;
        }
        public string GenerateToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var encodedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
            var increptedKey = new SigningCredentials(encodedKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
                (
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                expires: DateTime.Now.AddMinutes(jwtConfig.ExpiresInMinutes),
                signingCredentials: increptedKey,
                claims: claims
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
