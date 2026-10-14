using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMC.ApiService.DTO;
using NMC.DataAccess;
using NMC.DataAccess.Models;

namespace NMC.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly NmcDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(NmcDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("You must enter both the username and password.");
            }

            bool userExists = await _context.Users.AnyAsync(u => u.Username == dto.Username, cancellationToken);

            if (userExists)
            {
                return BadRequest("The specified username is already taken, sorry.");
            }

            var user = new User
            {
                Username = dto.Username
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            string token = GenerateMockToken(user.Id, user.Username);
            var response = new AuthResponseDto(user.Id, user.Username, token);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username, cancellationToken);

            if (user == null)
            {
                return Unauthorized("Bad luck. Invalid username or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Bad luck. Invalid username or password.");
            }

            string token = GenerateMockToken(user.Id, user.Username);
            var response = new AuthResponseDto(user.Id, user.Username, token);

            return Ok(response);
        }

        [HttpPut("{id:int}/username")]
        public async Task<IActionResult> UpdateUsername(int id, [FromBody] UpdateProfileRequestDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.NewUsername))
            {
                return BadRequest("Username cannot be empty.");
            }

            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            bool userExists = await _context.Users.AnyAsync(u => u.Username == dto.NewUsername && u.Id != id, cancellationToken);
            if (userExists)
            {
                return BadRequest("Username is already taken.");
            }

            user.Username = dto.NewUsername;
            await _context.SaveChangesAsync(cancellationToken);

            string token = GenerateMockToken(user.Id, user.Username);
            return Ok(new AuthResponseDto(user.Id, user.Username, token));
        }

        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequestDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest("New password cannot be empty.");
            }

            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAccount(int id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        #region Support Methods
        private static string GenerateMockToken(int userId, string username)
        {
            return $"mock-jwt-token-user-{userId}-{username}";
        }
        #endregion
    }
}