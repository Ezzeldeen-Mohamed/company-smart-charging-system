using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using CompanySmartChargingSystem.Domain.Entities;
using CompanySmartChargingSystem.Infrastructure.JWT;
using CompanySmartChargingSystem.Application.DTOs;
using System.Security.Claims;
using CompanySmartChargingSystem.Application.Services.IService;
using company_smart_charging_system.Services;
namespace company_smart_charging_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJWT _jwtService;
        private readonly IAuthService authService;
        private readonly ILocalizationService _localizer;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJWT jwtService,
            IAuthService authService,
            ILocalizationService localizer)
        {
            this.authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _localizer = localizer;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = _localizer.GetString("InvalidEmailOrPassword") });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                {
                    return Unauthorized(new { message = _localizer.GetString("InvalidEmailOrPassword") });
                }

                var roles = await _userManager.GetRolesAsync(user);
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
                return StatusCode(500, new { message = _localizer.GetString("LoginError"), error = ex.Message });
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
                    return BadRequest(new { message = _localizer.GetString("UserAlreadyExists") });
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
                    return BadRequest(new
                    {
                        message = _localizer.GetString("RegistrationFailed"),
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                await _userManager.AddToRoleAsync(user, "User");
                var roles = await _userManager.GetRolesAsync(user);

                var token = _jwtService.GenerateToken(user, roles);

                var refreshToken = _jwtService.GenerateRefreshToken();
                setRefreshTokenInCookie(refreshToken);

                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);

                return Ok(new AuthResponse
                {
                    Message = _localizer.GetString("RegistrationSuccess"),
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
                return StatusCode(500, new { message = _localizer.GetString("RegistrationError"), error = ex.Message });
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
                    return Unauthorized(new { message = _localizer.GetString("UserNotAuthenticated") });
                }

                var userInfo = await authService.getCurrentUser(userId);
                if (userInfo == null)
                {
                    return NotFound(new { message = _localizer.GetString("UserNotFound") });
                }

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = _localizer.GetString("GeneralError"), error = ex.Message });
            }
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> refreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var refreshTokenResult = await _jwtService.refreshToken(refreshToken);

            setRefreshTokenInCookie(new RefreshTokenModel
            {
                Token = refreshTokenResult.refreshToken,
                Expiration = refreshTokenResult.refreshTokenExpiration
            });

            return Ok(refreshTokenResult);
        }

        // helper function
        private void setRefreshTokenInCookie(RefreshTokenModel refreshToken)
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
