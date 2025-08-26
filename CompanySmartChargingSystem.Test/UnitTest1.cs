using AutoMapper;
using company_smart_charging_system.Controllers;
using CompanySmartChargingSystem.Application.DTOs;
using CompanySmartChargingSystem.Application.Services.IService;
using CompanySmartChargingSystem.Application.Services.Service;
using CompanySmartChargingSystem.Domain.ApplicationDbContext;
using CompanySmartChargingSystem.Domain.Entities;
using CompanySmartChargingSystem.Infrastructure.JWT;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
namespace CompanySmartChargingSystem.Tests
{
    public class UnitTests
    {
        // Unit Tests for AuthService
        public class AuthServiceTests
        {
            private readonly Mock<UserManager<User>> _userManagerMock;
            private readonly Mock<IMemoryCache> _memoryCacheMock;
            private readonly AuthService _authService;

            public AuthServiceTests()
            {
                _userManagerMock = new Mock<UserManager<User>>(
                    Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
                _memoryCacheMock = new Mock<IMemoryCache>();
                _authService = new AuthService(_userManagerMock.Object, _memoryCacheMock.Object);
            }

            [Fact]
            public async Task GetCurrentUser_UserExistsInCache_ReturnsCachedUserInfo()
            {
                // Arrange
                var userId = "user123";
                var cachedUserInfo = new UserInfo
                {
                    Id = userId,
                    Email = "test@company.com",
                    UserName = "testuser",
                    Roles = new List<string> { "User" }
                };
                object cacheValue = cachedUserInfo;
                _memoryCacheMock.Setup(m => m.TryGetValue($"UserInfo_{userId}", out cacheValue)).Returns(true);

                // Act
                var result = await _authService.getCurrentUser(userId);

                // Assert
                Assert.Equal(cachedUserInfo, result);
                _userManagerMock.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Never);
            }

            [Fact]
            public async Task GetCurrentUser_UserNotInCache_ReturnsUserInfo()
            {
                // Arrange
                var userId = "user123";
                var user = new User { Id = userId, Email = "test@company.com", UserName = "testuser" };
                var roles = new List<string> { "User" };
                object cacheValue = null;
                _memoryCacheMock.Setup(m => m.TryGetValue($"UserInfo_{userId}", out cacheValue)).Returns(false);
                _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
                _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles);

                var cacheEntryMock = new Mock<ICacheEntry>();
                _memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

                // Act
                var result = await _authService.getCurrentUser(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userId, result.Id);
                Assert.Equal(user.Email, result.Email);
                Assert.Equal(user.UserName, result.UserName);
                Assert.Equal(roles, result.Roles);
                _memoryCacheMock.Verify(m => m.CreateEntry($"UserInfo_{userId}"), Times.Once);
            }

            [Fact]
            public async Task GetCurrentUser_UserNotFound_ReturnsNull()
            {
                // Arrange
                var userId = "user123";
                object cacheValue = null;
                _memoryCacheMock.Setup(m => m.TryGetValue($"UserInfo_{userId}", out cacheValue)).Returns(false);
                _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((User)null);

                // Act
                var result = await _authService.getCurrentUser(userId);

                // Assert
                Assert.Null(result);
            }
        }

        // Unit Tests for ChargeTransactionService
        public class ChargeTransactionServiceTests
        {
            private readonly Mock<IUnitOfWork> _unitOfWorkMock;
            private readonly ChargeTransactionService _chargeTransactionService;

            public ChargeTransactionServiceTests()
            {
                _unitOfWorkMock = new Mock<IUnitOfWork>();
                _chargeTransactionService = new ChargeTransactionService(_unitOfWorkMock.Object);
            }

            [Fact]
            public async Task GetAllCharges_ReturnsAllChargeTransactions()
            {
                // Arrange
                var charges = new List<ChargeTransaction>
                {
                    new ChargeTransaction { Id = 1, MeterId = 1, ContractId = 1, NetValue = 100 },
                    new ChargeTransaction { Id = 2, MeterId = 2, ContractId = 2, NetValue = 200 }
                };
                var mockRepo = new Mock<IBaseRepo<ChargeTransaction>>();
                mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(charges);
                _unitOfWorkMock.Setup(u => u.ChargeTransactions).Returns(mockRepo.Object);

                // Act
                var result = await _chargeTransactionService.GetAllCharges();

                // Assert
                Assert.Equal(charges, result);
                Assert.Equal(2, result.Count());
            }

            [Fact]
            public async Task ImportCharges_AddsChargesAndSaves()
            {
                // Arrange
                var charges = new List<ChargeTransaction>
                {
                    new ChargeTransaction { Id = 1, MeterId = 1, ContractId = 1, NetValue = 100 },
                    new ChargeTransaction { Id = 2, MeterId = 2, ContractId = 2, NetValue = 200 }
                };
                var mockRepo = new Mock<IBaseRepo<ChargeTransaction>>();
                _unitOfWorkMock.Setup(u => u.ChargeTransactions).Returns(mockRepo.Object);
                _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(2);

                // Act
                await _chargeTransactionService.ImportCharges(charges);

                // Assert
                mockRepo.Verify(r => r.AddAsync(It.IsAny<ChargeTransaction>()), Times.Exactly(2));
                _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            }
        }

        // Unit Tests for AuthController
        public class AuthControllerTests
        {
            private readonly Mock<UserManager<User>> _userManagerMock;
            private readonly Mock<SignInManager<User>> _signInManagerMock;
            private readonly Mock<IJWT> _jwtServiceMock;
            private readonly Mock<IAuthService> _authServiceMock;
            private readonly AuthController _authController;

            public AuthControllerTests()
            {
                _userManagerMock = new Mock<UserManager<User>>(
                    Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
                _signInManagerMock = new Mock<SignInManager<User>>(
                    _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(),
                    Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
                _jwtServiceMock = new Mock<IJWT>();
                _authServiceMock = new Mock<IAuthService>();
                _authController = new AuthController(_userManagerMock.Object, _signInManagerMock.Object,
                    _jwtServiceMock.Object, _authServiceMock.Object);
            }

            [Fact]
            public async Task Login_ValidCredentials_ReturnsOkWithAuthResponse()
            {
                // Arrange
                var request = new LoginRequest { Email = "test@company.com", Password = "Password123!" };
                var user = new User { Id = "user123", Email = request.Email, UserName = "testuser" };
                var roles = new List<string> { "User" };
                var refreshToken = new RefreshToken { Token = "refresh123", Expiration = DateTime.UtcNow.AddDays(7), isNew = true };

                _userManagerMock.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(user);
                _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, request.Password, false))
                    .ReturnsAsync(SignInResult.Success);
                _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles);
                _jwtServiceMock.Setup(j => j.GenerateToken(user, roles)).Returns("jwt_token");
                _jwtServiceMock.Setup(j => j.CheckAndCreateNewRefreshToken(user)).Returns(refreshToken);
                _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

                // Act
                var result = await _authController.Login(request);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var authResponse = Assert.IsType<AuthResponse>(okResult.Value);
                Assert.Equal("jwt_token", authResponse.Token);
                Assert.Equal(refreshToken.Token, authResponse.refreshToken);
                Assert.Equal(user.Id, authResponse.User.Id);
                Assert.Equal(roles, authResponse.User.Roles);
            }

            [Fact]
            public async Task Login_InvalidCredentials_ReturnsUnauthorized()
            {
                // Arrange
                var request = new LoginRequest { Email = "test@company.com", Password = "WrongPassword" };
                _userManagerMock.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((User)null);

                // Act
                var result = await _authController.Login(request);

                // Assert
                var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
                Assert.Equal("Invalid email or password", (unauthorizedResult.Value as dynamic).message);
            }

            [Fact]
            public async Task Register_ValidRequest_ReturnsOkWithAuthResponse()
            {
                // Arrange
                var request = new RegisterRequest
                {
                    Email = "newuser@company.com",
                    Password = "Password123!",
                    ConfirmPassword = "Password123!"
                };
                var user = new User { Id = "user123", Email = request.Email, UserName = request.Email };
                var roles = new List<string> { "User" };
                var refreshToken = new RefreshToken { Token = "refresh123", Expiration = DateTime.UtcNow.AddDays(7) };

                _userManagerMock.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((User)null);
                _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
                    .ReturnsAsync(IdentityResult.Success);
                _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "User")).ReturnsAsync(IdentityResult.Success);
                _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<User>(), roles)).Returns("jwt_token");
                _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns(refreshToken);
                _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

                // Act
                var result = await _authController.Register(request);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var authResponse = Assert.IsType<AuthResponse>(okResult.Value);
                Assert.Equal("jwt_token", authResponse.Token);
                Assert.Equal("User registered successfully", authResponse.Message);
                Assert.Equal(roles, authResponse.User.Roles);
            }

            [Fact]
            public async Task Register_ExistingUser_ReturnsBadRequest()
            {
                // Arrange
                var request = new RegisterRequest
                {
                    Email = "existing@company.com",
                    Password = "Password123!",
                    ConfirmPassword = "Password123!"
                };
                _userManagerMock.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(new User());

                // Act
                var result = await _authController.Register(request);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("User with this email already exists", (badRequestResult.Value as dynamic).message);
            }
        }

        // Integration Tests
        public class IntegrationTests : IDisposable
        {
            private readonly AppDbContext _context;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly ChargeTransactionController _chargeTransactionController;

            public IntegrationTests()
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
                _context = new AppDbContext(options);

                // Mock IUnitOfWork with real repository implementations
                var customersRepo = new BaseRepo<Customer>(_context);
                var contractsRepo = new BaseRepo<Contract>(_context);
                var metersRepo = new BaseRepo<Meter>(_context);
                var chargeTransactionsRepo = new BaseRepo<ChargeTransaction>(_context);
                var usersRepo = new BaseRepo<User>(_context);

                var unitOfWorkMock = new Mock<IUnitOfWork>();
                unitOfWorkMock.Setup(u => u.Customers).Returns(customersRepo);
                unitOfWorkMock.Setup(u => u.Contracts).Returns(contractsRepo);
                unitOfWorkMock.Setup(u => u.Meters).Returns(metersRepo);
                unitOfWorkMock.Setup(u => u.ChargeTransactions).Returns(chargeTransactionsRepo);
                unitOfWorkMock.Setup(u => u.Users).Returns(usersRepo);
                unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(() => _context.SaveChangesAsync().GetAwaiter().GetResult());
                _unitOfWork = unitOfWorkMock.Object;

                var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
                _mapper = config.CreateMapper();

                _chargeTransactionController = new ChargeTransactionController(_unitOfWork, _mapper);
            }

            [Fact]
            public async Task ChargeTransactionController_GetAll_ReturnsAllChargeTransactions()
            {
                // Arrange
                var charge1 = new ChargeTransaction { Id = 1, MeterId = 1, ContractId = 1, NetValue = 100, CreatedDate = DateTime.UtcNow };
                var charge2 = new ChargeTransaction { Id = 2, MeterId = 2, ContractId = 2, NetValue = 200, CreatedDate = DateTime.UtcNow };
                await _context.ChargeTransactions.AddRangeAsync(charge1, charge2);
                await _context.SaveChangesAsync();

                // Act
                var result = await _chargeTransactionController.GetAll();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var chargeDtos = Assert.IsType<List<ChargeTransactionDto>>(okResult.Value);
                Assert.Equal(2, chargeDtos.Count);
                Assert.Contains(chargeDtos, c => c.Id == 1 && c.NetValue == 100);
                Assert.Contains(chargeDtos, c => c.Id == 2 && c.NetValue == 200);
            }

            [Fact]
            public async Task ChargeTransactionController_Create_AddsNewChargeTransaction()
            {
                // Arrange
                var chargeDto = new ChargeTransactionDto
                {
                    Id = 1,
                    MeterId = 1,
                    ContractId = 1,
                    Amount = 150,
                    Date = DateTime.UtcNow
                };

                // Act
                var result = await _chargeTransactionController.Create(chargeDto);

                // Assert
                var createdResult = Assert.IsType<CreatedAtActionResult>(result.ExecuteResult);
                var createdDto = Assert.IsType<ChargeTransactionDto>(createdResult.Value);
                Assert.Equal(chargeDto.Id, createdDto.Id);
                Assert.Equal(chargeDto.Amount, createdDto.Amount);
                var savedCharge = await _context.ChargeTransactions.FindAsync(chargeDto.Id);
                Assert.NotNull(savedCharge);
                Assert.Equal(chargeDto.Amount, savedCharge.NetValue);
            }

            [Fact]
            public async Task ChargeTransactionController_Update_UpdatesExistingChargeTransaction()
            {
                // Arrange
                var charge = new ChargeTransaction { Id = 1, MeterId = 1, ContractId = 1, NetValue = 100, CreatedDate = DateTime.UtcNow };
                await _context.ChargeTransactions.AddAsync(charge);
                await _context.SaveChangesAsync();
                var chargeDto = new ChargeTransactionDto
                {
                    Id = 1,
                    MeterId = 1,
                    ContractId = 1,
                    Amount = 200,
                    Date = DateTime.UtcNow
                };

                // Act
                var result = await _chargeTransactionController.Update(1, chargeDto);

                // Assert
                Assert.IsType<NoContentResult>(result.ExecuteResult);
                var updatedCharge = await _context.ChargeTransactions.FindAsync(1);
                Assert.Equal(200, updatedCharge.NetValue);
            }

            [Fact]
            public async Task ChargeTransactionController_Delete_RemovesChargeTransaction()
            {
                // Arrange
                var charge = new ChargeTransaction { Id = 1, MeterId = 1, ContractId = 1, NetValue = 100, CreatedDate = DateTime.UtcNow };
                await _context.ChargeTransactions.AddAsync(charge);
                await _context.SaveChangesAsync();

                // Act
                var result = await _chargeTransactionController.Delete(1);

                // Assert
                Assert.IsType<NoContentResult>(result.ExecuteResult);
                var deletedCharge = await _context.ChargeTransactions.FindAsync(1);
                Assert.Null(deletedCharge);
            }

            public void Dispose()
            {
                _context.Database.EnsureDeleted();
                _context.Dispose();
            }
        }

       
    }
}