using HealthApi.DTO;
using HealthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;



namespace HealthApi.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext context;
        private readonly ReverseGeocodingService _geoService;

        public UserController(AppDbContext context, ReverseGeocodingService geoService)
        {
            this.context = context;
            _geoService = geoService;
        }


        // Get All Users API (Salem)
        [HttpGet("api/usersDetails")]
        public IActionResult GetAllUsersData()
        {
            var users = new List<User>();
            try
            {
                users = context.Users.Include(t => t.UserLocation).ToList();
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

        // Filter Users API (Salem)
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
                //    usersQuery = usersQuery.Where(u => u.AreaName.Contains(filter.AreaName)); -------------------------------------Remove this in production
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

                // IsFlagged filtering
                if (!string.IsNullOrWhiteSpace(filter.IsFlagged))
                {
                    if (bool.TryParse(filter.IsFlagged, out var isFlaggedValue))
                    {
                        usersQuery = usersQuery.Where(u => u.IsFlagged == isFlaggedValue);
                    }
                }


                var users = usersQuery.ToList();

                if (!users.Any())
                    return NotFound("No matching users found.");

                var userDetailsList = users.Select(user => new UserDetailsDTO
                {
                    UserId = user.UserId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Gender = user.Sex,
                    IsFlagged = user.IsFlagged ?? false,
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

        // Get User Profile API (Salem)
        [HttpGet("api/Profile")]
        //[Authorize] // Requires JWT authentication
        public async Task<IActionResult> GetProfile()
        {
            // Get user ID from the JWT token
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); -------------------------------------Remove this in production
            var userId = 5; 


            if (userId == null)
                return Unauthorized();

            var user = await context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            var profile = new ProfileDto
            {
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email
            };

            return Ok(profile);
        }


        // Change Password API (Salem)
        [HttpPost("api/Profile/ChangePassword")]
        //[Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OldPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword) ||
                string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return BadRequest("All fields are required.");
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return BadRequest("New password and confirmation do not match.");
            }

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); -------------------------------------Remove this in production
            var userId = 5;

            if (userId == null) return Unauthorized();

            var user = await context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            //I need i hashing method here first to compare the hashed passwords 

            user.Password = dto.NewPassword;

            context.Users.Update(user);
            context.SaveChanges();  

            return Ok(new { message = "Password changed successfully" });
        }


        // Saving User Location (Salem)
        //[Authorize]
        [HttpPost("api/Location/SaveUserLocation")]
        public async Task<IActionResult> SaveLocation([FromBody] LocationDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userIdString = "1003"; // For testing, replace with actual user ID retrieval logic ------------------------------------- Remove this in production

            if (!int.TryParse(userIdString, out var userId))
                return Unauthorized();

            // Try to find an existing location for this user
            var existingLocation = await context.UserLocations
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (existingLocation != null)
            {
                // Update existing location
                existingLocation.Latitude = dto.Latitude;
                existingLocation.Longitude = dto.Longitude;
                existingLocation.CreatedAtTime = DateTime.UtcNow;

                context.UserLocations.Update(existingLocation);

                // Optional: also update User table's LocationId if not already set
                var user = await context.Users.FindAsync(userId);
                if (user != null && user.LocationId != existingLocation.LocationId)
                {
                    user.LocationId = existingLocation.LocationId;
                    context.Users.Update(user);
                }
            }
            else
            {
                // No existing location, insert a new one
                var newLocation = new UserLocation
                {
                    UserId = userId,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    CreatedAtTime = DateTime.UtcNow
                };

                context.UserLocations.Add(newLocation);
                await context.SaveChangesAsync(); // Save so we can access LocationId

                // Update user with this new LocationId
                var user = await context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.LocationId = newLocation.LocationId;
                    context.Users.Update(user);
                }
            }

            await context.SaveChangesAsync(); // Final save for either path

            return Ok(new { message = "Location saved successfully." });
        }








    }
} 
