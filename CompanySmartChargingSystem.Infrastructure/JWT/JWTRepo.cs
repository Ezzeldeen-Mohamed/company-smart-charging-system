using Azure;
using CompanySmartChargingSystem.Application.DTOs;
using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Infrastructure.JWT
{
    public class JWTRepo : IJWT
    {
        private readonly JWTConfig jwtConfig;
        private readonly UserManager<User> userManager;
        public JWTRepo(IOptions<JWTConfig> jwtConfig, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.jwtConfig = jwtConfig.Value;
        }

        public RefreshTokenModel CheckAndCreateNewRefreshToken(User user)
        {
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                // If the active token is not expired, return it
                return new RefreshTokenModel
                {
                    isNew = false,
                    Token = activeToken.Token,
                    Expiration = activeToken.Expiration,
                };
            }
            else
            {
                var Token = GenerateRefreshToken();
                
                // If the active token is not expired, return it
                return new RefreshTokenModel
                {
                    isNew = true,
                    Token = Token.Token,
                    Expiration = Token.Expiration,
                };
            }
        }

        public RefreshTokenModel GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);
            return new RefreshTokenModel
            {
                Token = Convert.ToBase64String(randomNumber),
                Expiration = DateTime.UtcNow.AddDays(7),
            };
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

        public async Task<AuthResponse> refreshToken(string refreshToken)
        {
            var authResponse = new AuthResponse();
            var user = userManager.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
            if (user == null)
            {
                authResponse.Message = "Invalid refresh token";

                return authResponse;
            }
            var token = user.RefreshTokens.Single(t => t.Token == refreshToken);
            if (!token.IsActive)
            {
                authResponse.Message = "Inactive refresh token";
                return authResponse;
            }
            // Revoke current refresh token
            token.revokedOn = DateTime.UtcNow;

            // Generate new refresh token and add to user
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);
            // Generate new JWT
            var roles = userManager.GetRolesAsync(user).Result;
            var jwtToken = GenerateToken(user, roles);
            authResponse.Message = "Token refreshed successfully";
            authResponse.Token = jwtToken;
            authResponse.User = new UserInfo
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList()
            };
            authResponse.refreshToken = newRefreshToken.Token;
            authResponse.refreshTokenExpiration = newRefreshToken.Expiration;
            return authResponse;

        }

    }
}
