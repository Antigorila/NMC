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

        #region UpdateUsername Tests

        [Test]
        public async Task UpdateUsername_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new UpdateProfileRequestDto("   ");

            // Act
            var result = await controller.UpdateUsername(1, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Is.EqualTo("Username cannot be empty."));
        }

        [Test]
        public async Task UpdateUsername_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new UpdateProfileRequestDto("NewName");

            // Act
            var result = await controller.UpdateUsername(999, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateUsername_NameAlreadyTakenByAnotherUser_ReturnsBadRequest()
        {
            // Arrange
            using (var setupContext = CreateContext())
            {
                setupContext.Users.Add(new User { Username = "user1" });
                setupContext.Users.Add(new User { Username = "user2" });
                await setupContext.SaveChangesAsync();
            }

            using var testContext = CreateContext();
            var user1 = await testContext.Users.FirstAsync(u => u.Username == "user1");
            var controller = new AuthController(testContext);
            var dto = new UpdateProfileRequestDto("user2");

            // Act
            var result = await controller.UpdateUsername(user1.Id, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Is.EqualTo("Username is already taken."));
        }

        [Test]
        public async Task UpdateUsername_ValidRequest_ReturnsOkAndUpdatesName()
        {
            // Arrange
            int userId;
            using (var setupContext = CreateContext())
            {
                var user = new User { Username = "oldUser" };
                setupContext.Users.Add(user);
                await setupContext.SaveChangesAsync();
                userId = user.Id;
            }

            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new UpdateProfileRequestDto("updatedUser");

            // Act
            var result = await controller.UpdateUsername(userId, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as AuthResponseDto;

            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Username, Is.EqualTo("updatedUser"));

            var updatedUser = await testContext.Users.FindAsync(userId);
            Assert.That(updatedUser!.Username, Is.EqualTo("updatedUser"));
        }

        #endregion

        #region ChangePassword Tests

        [Test]
        public async Task ChangePassword_EmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new ChangePasswordRequestDto("");

            // Act
            var result = await controller.ChangePassword(1, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Is.EqualTo("New password cannot be empty."));
        }

        [Test]
        public async Task ChangePassword_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new ChangePasswordRequestDto("newPass123");

            // Act
            var result = await controller.ChangePassword(999, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task ChangePassword_ValidRequest_UpdatesPasswordHashAndReturnsOk()
        {
            // Arrange
            int userId;
            using (var setupContext = CreateContext())
            {
                var user = new User { Username = "passTestUser", PasswordHash = "oldHash" };
                setupContext.Users.Add(user);
                await setupContext.SaveChangesAsync();
                userId = user.Id;
            }

            using var testContext = CreateContext();
            var controller = new AuthController(testContext);
            var dto = new ChangePasswordRequestDto("brandNewPassword");

            // Act
            var result = await controller.ChangePassword(userId, dto, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());

            var updatedUser = await testContext.Users.FindAsync(userId);
            Assert.That(updatedUser, Is.Not.Null);
            Assert.That(updatedUser!.PasswordHash, Is.Not.EqualTo("oldHash"));

            var hasher = new PasswordHasher<User>();
            var verifyResult = hasher.VerifyHashedPassword(updatedUser, updatedUser.PasswordHash, "brandNewPassword");
            Assert.That(verifyResult, Is.EqualTo(PasswordVerificationResult.Success));
        }

        #endregion

        #region DeleteAccount Tests

        [Test]
        public async Task DeleteAccount_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            using var testContext = CreateContext();
            var controller = new AuthController(testContext);

            // Act
            var result = await controller.DeleteAccount(999, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteAccount_ValidUser_RemovesUserFromDbAndReturnsNoContent()
        {
            // Arrange
            int userId;
            using (var setupContext = CreateContext())
            {
                var user = new User { Username = "userToDelete", PasswordHash = "hash" };
                setupContext.Users.Add(user);
                await setupContext.SaveChangesAsync();
                userId = user.Id;
            }

            using var testContext = CreateContext();
            var controller = new AuthController(testContext);

            // Act
            var result = await controller.DeleteAccount(userId, CancellationToken.None);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());

            var deletedUser = await testContext.Users.FindAsync(userId);
            Assert.That(deletedUser, Is.Null);
        }

        #endregion
    }
}