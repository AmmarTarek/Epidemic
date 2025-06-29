using HealthApi.DTO;
using HealthApi.Repository.Repos_A;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _repo.UserExistsAsync(dto.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                DateOfBirth = dto.DateOfBirth,
                Sex = dto.Sex,
                Job = dto.Job,
                PhoneNumber = dto.PhoneNumber,
                RoleId = dto.RoleId,
                CreatedAtTime = DateTime.UtcNow
            };

            var createdUser = await _repo.RegisterAsync(user);
            return Ok(new { createdUser.UserId, createdUser.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _repo.LoginAsync(dto.Email, dto.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = JwtHelper.GenerateJwtToken(user, _config);
            return Ok(new { token });
        }
    }
}
