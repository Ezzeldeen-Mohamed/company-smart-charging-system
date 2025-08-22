using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CompanySmartChargingSystem.Domain.Entities;
using CompanySmartChargingSystem.Infrastructure.JWT;
using CompanySmartChargingSystem.Application.DTOs;
using System.Security.Claims;

namespace company_smart_charging_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJWT _jwtService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJWT jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Generate JWT token with roles
                var token = _jwtService.GenerateToken(user, roles);

                var refreshToken = _jwtService.CheckAndCreateNewRefreshToken(user);

                setRefreshTokenInCookie(refreshToken);

                if (refreshToken.isNew)
                {
                    user.RefreshTokens.Add(refreshToken);

                    await _userManager.UpdateAsync(user);
                }

                

                return Ok(new AuthResponse
                {
                    Token = token,
                    refreshToken = refreshToken.Token,
                    refreshTokenExpiration = refreshToken.Expiration,
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        Roles = roles.ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists" });
                }

                var user = new User
                {
                    UserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Registration failed", errors = result.Errors.Select(e => e.Description) });
                }

                // Assign default role (User)
                await _userManager.AddToRoleAsync(user, "User");

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Generate JWT token with roles
                var token = _jwtService.GenerateToken(user, roles);

                var refreshToken = _jwtService.GenerateRefreshToken();

                setRefreshTokenInCookie(refreshToken);

                user.RefreshTokens.Add(refreshToken);

                await _userManager.UpdateAsync(user);

                return Ok(new AuthResponse
                {
                    Message = "User registered successfully",
                    Token = token,
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        Roles = roles.ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
        [HttpPost("refreshToken")]
        public async Task<IActionResult> refreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var refreshTokenResult = await _jwtService.refreshToken(refreshToken);
            setRefreshTokenInCookie(new RefreshToken { Token = refreshTokenResult.refreshToken, Expiration=refreshTokenResult.refreshTokenExpiration});
            return Ok(refreshTokenResult);
        }
        // helper function
        
        private void setRefreshTokenInCookie(RefreshToken refreshToken)
        {
            if (refreshToken != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = refreshToken.Expiration.ToLocalTime(),
                };
                Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            }
        }
    }
}
