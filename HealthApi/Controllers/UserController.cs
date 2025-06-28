using Microsoft.AspNetCore.Mvc;
using HealthApi.Models;
using HealthApi.DTO;



namespace HealthApi.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext context;

        public UserController(AppDbContext context)
        {
            this.context = context;
        }


        [HttpGet("api/usersDetails")]
        public IActionResult GetAllUsersData()
        {
            var users = new List<User>();
            try
            {
                users = context.Users.ToList();
                if (users == null || !users.Any())
                {
                    return NotFound("No users found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            var ListOfUsersDetails = new List<UserDetailsDTO>();
            foreach (var user in users)
            {
                var userDetails = new UserDetailsDTO
                {
                    UserId = user.UserId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Gender = user.Sex,
                    IsFlagged = false, // Assuming a default value; adjust as needed
                    EPass = context.EPasses.FirstOrDefault(ep => ep.EPassID == user.EPassStatusId).Status,
                    Age = (DateTime.UtcNow - user.DateOfBirth).Days / 365, // Calculate age in years

                };
                ListOfUsersDetails.Add(userDetails);
            }

            return Ok(ListOfUsersDetails);
        }
    }
} 
