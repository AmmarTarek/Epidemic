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

        [HttpPost("api/usersDetails/filter")]
        public IActionResult FilterUsers([FromBody] UserFilterDTO filter)
        {
            try
            {
                var usersQuery = context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(filter.UserName))
                {
                    usersQuery = usersQuery.Where(u =>
                        (u.FirstName + " " + u.LastName).Contains(filter.UserName));
                }

                //if (!string.IsNullOrEmpty(filter.AreaName))
                //{
                //    usersQuery = usersQuery.Where(u => u.AreaName.Contains(filter.AreaName));
                //}

                if (!string.IsNullOrEmpty(filter.Gender))
                {
                    usersQuery = usersQuery.Where(u => u.Sex == filter.Gender);
                }

                if (filter.MinAge.HasValue || filter.MaxAge.HasValue)
                {
                    var now = DateTime.UtcNow;
                    if (filter.MinAge.HasValue)
                    {
                        var maxDob = now.AddYears(-filter.MinAge.Value);
                        usersQuery = usersQuery.Where(u => u.DateOfBirth <= maxDob);
                    }

                    if (filter.MaxAge.HasValue)
                    {
                        var minDob = now.AddYears(-filter.MaxAge.Value);
                        usersQuery = usersQuery.Where(u => u.DateOfBirth >= minDob);
                    }
                }

                // E-Pass filtering
                if (!string.IsNullOrEmpty(filter.EPass))
                {
                    var allowed = filter.EPass == "Allowed";
                    var matchingEPassIds = context.EPasses
                        .Where(ep => ep.Status == allowed)
                        .Select(ep => ep.EPassID)
                        .ToList();

                    usersQuery = usersQuery.Where(u => matchingEPassIds.Contains(u.EPassStatusId));
                }

                var users = usersQuery.ToList();

                if (!users.Any())
                    return NotFound("No matching users found.");

                var userDetailsList = users.Select(user => new UserDetailsDTO
                {
                    UserId = user.UserId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Gender = user.Sex,
                    IsFlagged = false, // default — replace if stored in DB
                    EPass = (bool)(context.EPasses.FirstOrDefault(ep => ep.EPassID == user.EPassStatusId)?.Status),
                    Age = (DateTime.UtcNow - user.DateOfBirth).Days / 365
                }).ToList();

                return Ok(userDetailsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }





    }
} 
