using HealthApi.DTO;
using HealthApi.Models;
using HealthApi.Repository.Repos_A;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace HealthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _hasher;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
            _hasher = new PasswordHasher<User>();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _repo.UserExistsAsync(dto.Email))
                return BadRequest("Email already exists.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Sex = dto.Sex,
                Job = dto.Job,
                PhoneNumber = dto.PhoneNumber,
                CreatedAtTime = DateTime.UtcNow
            };

            // Hash the password
            user.Password = _hasher.HashPassword(user, dto.Password);

            var createdUser = await _repo.RegisterAsync(user);
            return Ok(new { createdUser.UserId, createdUser.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _repo.GetUserByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials");

            var result = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid credentials");

            var token = JwtHelper.GenerateJwtToken(user, _config);
            return Ok(new { token });
        }
    }
}
