using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.Controllers;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;
using NUnit.Framework;

namespace ApiTests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private DbContextOptions<NmcDbContext> _dbOptions = null!;

        [SetUp]
        public void SetUp()
        {
            _dbOptions = new DbContextOptionsBuilder<NmcDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        }

        private NmcDbContext CreateContext() => new NmcDbContext(_dbOptions);

        #region Register Tests

        [Test]
        public async Task Register_EmptyUsernameOrPassword_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new AuthController(context);
            var dto = new RegisterRequestDto("", "password123");

            // Act
            var result = await controller.Register(dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.Value, Is.EqualTo("You must enter both the username and password."));
        }

        [Test]
        public async Task Register_ExistingUsername_ReturnsBadRequest()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Users.Add(new User { Username = "existingUser", PasswordHash = "hash" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new RegisterRequestDto("existingUser", "password123");

            // Act
            var result = await controller.Register(dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.Value, Is.EqualTo("The specified username is already taken, sorry."));
        }

        [Test]
        public async Task Register_ValidCredentials_ReturnsOkWithAuthResponse()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new RegisterRequestDto("newUser", "secretPassword");

            // Act
            var result = await controller.Register(dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var response = okResult!.Value as AuthResponseDto;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Username, Is.EqualTo("newUser"));
            Assert.That(response.UserId, Is.GreaterThan(0));
            Assert.That(response.Token, Is.EqualTo($"mock-jwt-token-user-{response.UserId}-newUser"));

            var dbUser = await testContext.Users.FirstOrDefaultAsync(u => u.Username == "newUser");
            Assert.That(dbUser, Is.Not.Null);
            Assert.That(dbUser!.PasswordHash, Is.Not.EqualTo("secretPassword"));
        }

        [TestCase("   ", "password")]
        [TestCase("username", "   ")]
        [TestCase("   ", "   ")]
        public async Task Register_WhitespaceCredentials_ReturnsBadRequest(string username, string password)
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new RegisterRequestDto(username, password);

            // Act
            var result = await controller.Register(dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Is.EqualTo("You must enter both the username and password."));
        }
        #endregion

        #region Login Tests

        [Test]
        public async Task Login_NonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new LoginRequestDto("unknownUser", "password123");

            // Act
            var result = await controller.Login(dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult, Is.Not.Null);
            Assert.That(unauthorizedResult!.Value, Is.EqualTo("Bad luck. Invalid username or password."));
        }

        [Test]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            var hasher = new PasswordHasher<User>();
            var user = new User { Username = "validUser" };
            user.PasswordHash = hasher.HashPassword(user, "correctPassword");

            using (var setupContext = CreateContext())
            {
                setupContext.Users.Add(user);
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new LoginRequestDto("validUser", "wrongPassword");

            // Act
            var result = await controller.Login(dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult, Is.Not.Null);
            Assert.That(unauthorizedResult!.Value, Is.EqualTo("Bad luck. Invalid username or password."));
        }

        [Test]
        public async Task Login_CorrectCredentials_ReturnsOkWithAuthResponse()
        {
            // Arrange
            var hasher = new PasswordHasher<User>();
            var user = new User { Username = "validUser" };
            user.PasswordHash = hasher.HashPassword(user, "correctPassword");

            using (var setupContext = CreateContext())
            {
                setupContext.Users.Add(user);
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new LoginRequestDto("validUser", "correctPassword");

            // Act
            var result = await controller.Login(dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var response = okResult!.Value as AuthResponseDto;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Username, Is.EqualTo("validUser"));
            Assert.That(response.UserId, Is.EqualTo(user.Id));
            Assert.That(response.Token, Is.EqualTo($"mock-jwt-token-user-{user.Id}-validUser"));
        }

        #endregion
    }
}
