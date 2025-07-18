﻿//using HealthApi.DTO;
//using HealthApi.Models;
//using HealthApi.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.IdentityModel.Tokens;
//using NetTopologySuite;
//using NetTopologySuite.Geometries;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Text.Json;



//namespace HealthApi.Controllers
//{
//    public class UserController : Controller
//    {
//        private readonly AppDbContext context;
//        private readonly ReverseGeocodingService _geoService;
//        private readonly LocationAnalysisService _locationAnalysisService;

//        public UserController(AppDbContext context, ReverseGeocodingService geoService, LocationAnalysisService locationAnalysisService)
//        {
//            this.context = context;
//            _geoService = geoService;
//            _locationAnalysisService = locationAnalysisService;
//        }


//        // Get All Users API (Salem)
//        [HttpGet("api/usersDetails")]
//        public IActionResult GetAllUsersData()
//        {
//            var users = new List<User>();
//            try
//            {
//                users = context.Users.Include(t => t.UserLocation).ToList();
//                if (users == null || !users.Any())
//                {
//                    return NotFound("No users found.");
//                }
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }

//            var ListOfUsersDetails = new List<UserDetailsDTO>();
//            foreach (var user in users)
//            {
//                var userDetails = new UserDetailsDTO
//                {
//                    UserId = user.UserId,
//                    FullName = $"{user.FirstName} {user.LastName}",
//                    Gender = user.Sex,
//                    IsFlagged = false, // Assuming a default value; adjust as needed
//                    EPass = context.EPasses.FirstOrDefault(ep => ep.EPassID == user.EPassStatusId).Status,
//                    Age = (DateTime.UtcNow - user.DateOfBirth).Days / 365, // Calculate age in years

//                };
//                ListOfUsersDetails.Add(userDetails);
//            }

//            return Ok(ListOfUsersDetails);
//        }

//        // Filter Users API (Salem)
//        [HttpPost("api/usersDetails/filter")]
//        public IActionResult FilterUsers([FromBody] UserFilterDTO filter)
//        {
//            try
//            {
//                var usersQuery = context.Users.AsQueryable();

//                if (!string.IsNullOrEmpty(filter.UserName))
//                {
//                    usersQuery = usersQuery.Where(u =>
//                        (u.FirstName + " " + u.LastName).Contains(filter.UserName));
//                }

//                if (!string.IsNullOrEmpty(filter.AreaName))
//                {
//                    usersQuery = usersQuery.Where(u => u.AreaName.Contains(filter.AreaName)); 
//                }

//                if (!string.IsNullOrEmpty(filter.Gender))
//                {
//                    usersQuery = usersQuery.Where(u => u.Sex == filter.Gender);
//                }

//                if (filter.MinAge.HasValue || filter.MaxAge.HasValue)
//                {
//                    var now = DateTime.UtcNow;
//                    if (filter.MinAge.HasValue)
//                    {
//                        var maxDob = now.AddYears(-filter.MinAge.Value);
//                        usersQuery = usersQuery.Where(u => u.DateOfBirth <= maxDob);
//                    }

//                    if (filter.MaxAge.HasValue)
//                    {
//                        var minDob = now.AddYears(-filter.MaxAge.Value);
//                        usersQuery = usersQuery.Where(u => u.DateOfBirth >= minDob);
//                    }
//                }

//                // E-Pass filtering
//                if (!string.IsNullOrEmpty(filter.EPass))
//                {
//                    var allowed = filter.EPass == "Allowed";
//                    var matchingEPassIds = context.EPasses
//                        .Where(ep => ep.Status == allowed)
//                        .Select(ep => ep.EPassID)
//                        .ToList();

//                    usersQuery = usersQuery.Where(u => matchingEPassIds.Contains(u.EPassStatusId));
//                }

//                // IsFlagged filtering
//                if (!string.IsNullOrWhiteSpace(filter.IsFlagged))
//                {
//                    if (bool.TryParse(filter.IsFlagged, out var isFlaggedValue))
//                    {
//                        usersQuery = usersQuery.Where(u => u.IsFlagged == isFlaggedValue);
//                    }
//                }


//                var users = usersQuery.ToList();

//                if (!users.Any())
//                    return NotFound("No matching users found.");

//                var userDetailsList = users.Select(user => new UserDetailsDTO
//                {
//                    UserId = user.UserId,
//                    FullName = $"{user.FirstName} {user.LastName}",
//                    Gender = user.Sex,
//                    IsFlagged = user.IsFlagged ?? false,
//                    EPass = (bool)(context.EPasses.FirstOrDefault(ep => ep.EPassID == user.EPassStatusId)?.Status),
//                    Age = (DateTime.UtcNow - user.DateOfBirth).Days / 365,
//                    AreaName = _locationAnalysisService.GetContainingAreaName(user.UserId) ?? "Unknown",
//                    Latitude = user.UserLocation?.Latitude,
//                    Longitude = user.UserLocation?.Longitude,

//                }).ToList();

//                return Ok(userDetailsList);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        // Get User Profile API (Salem)
//        [HttpGet("api/Profile")]
//        [Authorize]
//        public async Task<IActionResult> GetProfile()
//        {
//            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (string.IsNullOrEmpty(userIdStr))
//                return Unauthorized();

//            if (!int.TryParse(userIdStr, out int userId))
//                return Unauthorized("Invalid user ID");

//            var user = await context.Users.FindAsync(userId);
//            if (user == null)
//                return NotFound();

//            var areaName = _locationAnalysisService.GetContainingAreaName(userId);

//            var profile = new ProfileDto
//            {
//                FullName = $"{user.FirstName} {user.LastName}",
//                Email = user.Email,
//                AreaName = areaName ?? "Unknown"
//            };

//            return Ok(profile);
//        }



//        // Change Password API (Salem)
//        [HttpPost("api/Profile/ChangePassword")]
//        [Authorize]
//        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
//        {
//            if (string.IsNullOrWhiteSpace(dto.OldPassword) ||
//                string.IsNullOrWhiteSpace(dto.NewPassword) ||
//                string.IsNullOrWhiteSpace(dto.ConfirmPassword))
//            {
//                return BadRequest("All fields are required.");
//            }

//            if (dto.NewPassword != dto.ConfirmPassword)
//            {
//                return BadRequest("New password and confirmation do not match.");
//            }

//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); -------------------------------------Remove this in production
//            //var userId = 5;

//            if (userId == null) return Unauthorized();

//            var user = await context.Users.FindAsync(userId);
//            if (user == null) return NotFound();

//            //I need i hashing method here first to compare the hashed passwords 

//            user.Password = dto.NewPassword;

//            context.Users.Update(user);
//            context.SaveChanges();  

//            return Ok(new { message = "Password changed successfully" });
//        }


//        // Saving User Location (Salem)
//        [Authorize]
//        [HttpPost("api/Location/SaveUserLocation")]
//        public async Task<IActionResult> SaveLocation([FromBody] LocationDto dto)
//        {
//            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            //var userIdString = "1003"; // For testing, replace with actual user ID retrieval logic ------------------------------------- Remove this in production

//            if (!int.TryParse(userIdString, out var userId))
//                return Unauthorized();

//            // Try to find an existing location for this user
//            var existingLocation = await context.UserLocations
//                .FirstOrDefaultAsync(l => l.UserId == userId);

//            if (existingLocation != null)
//            {
//                // Update existing location
//                existingLocation.Latitude = dto.Latitude;
//                existingLocation.Longitude = dto.Longitude;
//                existingLocation.CreatedAtTime = DateTime.UtcNow;

//                context.UserLocations.Update(existingLocation);

//                // Optional: also update User table's LocationId if not already set
//                var user = await context.Users.FindAsync(userId);
//                if (user != null && user.LocationId != existingLocation.LocationId)
//                {
//                    user.LocationId = existingLocation.LocationId;
//                    context.Users.Update(user);

//                }
//            }
//            else
//            {
//                // No existing location, insert a new one
//                var newLocation = new UserLocation
//                {
//                    UserId = userId,
//                    Latitude = dto.Latitude,
//                    Longitude = dto.Longitude,
//                    CreatedAtTime = DateTime.UtcNow
//                };

//                context.UserLocations.Add(newLocation);
//                await context.SaveChangesAsync(); // Save so we can access LocationId

//                // Update user with this new LocationId
//                var user = await context.Users.FindAsync(userId);
//                if (user != null)
//                {
//                    user.LocationId = newLocation.LocationId;
//                    context.Users.Update(user);

//                }
//            }

//            var userDB = await context.Users.FindAsync(userId);
//            userDB.AreaName = _locationAnalysisService.GetContainingAreaName(userId); //--------------
//            context.Users.Update(userDB);

//            await context.SaveChangesAsync(); // Final save for either path

//            return Ok(new { message = "Location saved successfully." });
//        }

//        [HttpGet("api/Location/GetUserLocation")]
//        public async Task<IActionResult> GetUserLocation()
//        {
//            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var userIdString = "1003"; // For testing, replace with actual user ID retrieval logic ------------------------------------- Remove this in production

//            if (!int.TryParse(userIdString, out var userId))
//                return Unauthorized();

//            // Try to find an existing location for this user
//            UserLocation? existingLocation = await context.UserLocations
//                .FirstOrDefaultAsync(l => l.UserId == userId);

//            if (existingLocation != null)
//            {
//                return Ok(new
//                {
//                    latitude = existingLocation.Latitude,
//                    longitude = existingLocation.Longitude
//                });
//            }

//            return NotFound("User location not found");
//        }

//        [HttpGet("api/Location/GetUserAreaState")]
//        public async Task<IActionResult> GetUserAreaState() 
//        {
//            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var userIdString = "1003"; // For testing, replace with actual user ID retrieval logic ------------------------------------- Remove this in production

//            if (!int.TryParse(userIdString, out var userId))
//                return Unauthorized();

//            var areaState = _locationAnalysisService.GetContainingAreaRiskState(userId);
//            if (areaState == null)
//                return NotFound("User location not found or not in a risk area.");

//            var riskState = _locationAnalysisService.GetContainingAreaRiskState(userId);
//            if (riskState == null)
//                return NotFound("Risk state not found for the user's area.");

//            return Ok(new { RiskState = riskState });
//        }

//    }
//} 

using HealthApi.DTO;
using HealthApi.Models;
using HealthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext context;
        private readonly ReverseGeocodingService _geoService;
        private readonly LocationAnalysisService _locationAnalysisService;

        public UserController(AppDbContext context, ReverseGeocodingService geoService, LocationAnalysisService locationAnalysisService)
        {
            this.context = context;
            _geoService = geoService;
            _locationAnalysisService = locationAnalysisService;
        }

        // -------------------- Get All Users --------------------
        [HttpGet("api/usersDetails")]
        public IActionResult GetAllUsersData()
        {
            try
            {
                var users = context.Users.Include(t => t.UserLocation).ToList();

                if (!users.Any())
                    return NotFound("No users found.");

                var userDetailsList = users.Select(user => new UserDetailsDTO
                {
                    UserId = user.UserId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Gender = user.Sex,
                    IsFlagged = false,
                    EPass = context.EPasses.FirstOrDefault(ep => ep.EPassID == user.EPassStatusId)?.Status ?? false,
                    Age = (DateTime.UtcNow - user.DateOfBirth).Days / 365,
                }).ToList();

                return Ok(userDetailsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // -------------------- Filter Users --------------------
        [HttpPost("api/usersDetails/filter")]
        public IActionResult FilterUsers([FromBody] UserFilterDTO filter)
        {
            try
            {
                var query = context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(filter.UserName))
                    query = query.Where(u => (u.FirstName + " " + u.LastName).Contains(filter.UserName));

                if (!string.IsNullOrEmpty(filter.AreaName))
                    query = query.Where(u => u.AreaName.Contains(filter.AreaName));

                if (!string.IsNullOrEmpty(filter.Gender))
                    query = query.Where(u => u.Sex == filter.Gender);

                if (filter.MinAge.HasValue)
                    query = query.Where(u => u.DateOfBirth <= DateTime.UtcNow.AddYears(-filter.MinAge.Value));

                if (filter.MaxAge.HasValue)
                    query = query.Where(u => u.DateOfBirth >= DateTime.UtcNow.AddYears(-filter.MaxAge.Value));

                if (!string.IsNullOrEmpty(filter.EPass))
                {
                    bool epStatus = filter.EPass == "Allowed";
                    var ePassIds = context.EPasses.Where(ep => ep.Status == epStatus).Select(ep => ep.EPassID).ToList();
                    query = query.Where(u => ePassIds.Contains(u.EPassStatusId));
                }

                if (bool.TryParse(filter.IsFlagged, out bool isFlagged))
                    query = query.Where(u => u.IsFlagged == isFlagged);

                var users = query.Include(u => u.UserLocation).ToList();
                if (!users.Any())
                    return NotFound("No matching users found.");

                var result = users.Select(user => new UserDetailsDTO
                {
                    UserId = user.UserId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Gender = user.Sex,
                    IsFlagged = user.IsFlagged ?? false,
                    EPass = context.EPasses.FirstOrDefault(ep => ep.EPassID == user.EPassStatusId)?.Status ?? false,
                    Age = (DateTime.UtcNow - user.DateOfBirth).Days / 365,
                    AreaName = _locationAnalysisService.GetContainingAreaName(user.UserId) ?? "Unknown",
                    Latitude = user.UserLocation?.Latitude,
                    Longitude = user.UserLocation?.Longitude,
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // -------------------- Get Profile --------------------
        [HttpGet("api/Profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid user ID");

            var user = await context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            var areaName = _locationAnalysisService.GetContainingAreaName(userId);

            var profile = new ProfileDto
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                AreaName = areaName ?? "Unknown"
            };

            return Ok(profile);
        }

        // -------------------- Change Password --------------------
        [HttpPost("api/Profile/ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OldPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword) ||
                string.IsNullOrWhiteSpace(dto.ConfirmPassword))
                return BadRequest("All fields are required.");

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("New password and confirmation do not match.");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var user = await context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // TODO: Verify old password before changing (missing hashing logic)
            user.Password = dto.NewPassword;

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }

        // -------------------- Save Location --------------------
        [HttpPost("api/Location/SaveUserLocation")]
        [Authorize]
        public async Task<IActionResult> SaveLocation([FromBody] LocationDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var location = await context.UserLocations.FirstOrDefaultAsync(l => l.UserId == userId);
            var now = DateTime.UtcNow;

            if (location != null)
            {
                location.Latitude = dto.Latitude;
                location.Longitude = dto.Longitude;
                location.CreatedAtTime = now;
                context.UserLocations.Update(location);
            }
            else
            {
                location = new UserLocation
                {
                    UserId = userId,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    CreatedAtTime = now
                };
                context.UserLocations.Add(location);
                await context.SaveChangesAsync(); // Save to get LocationId
            }

            var user = await context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LocationId = location.LocationId;
                user.AreaName = _locationAnalysisService.GetContainingAreaName(userId);
                context.Users.Update(user);
            }

            await context.SaveChangesAsync();
            return Ok(new { message = "Location saved successfully." });
        }

        // -------------------- Get User Location --------------------
        [HttpGet("api/Location/GetUserLocation")]
        [Authorize]
        public async Task<IActionResult> GetUserLocation()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var location = await context.UserLocations.FirstOrDefaultAsync(l => l.UserId == userId);
            if (location == null)
                return NotFound("User location not found.");

            return Ok(new
            {
                latitude = location.Latitude,
                longitude = location.Longitude
            });
        }

        // -------------------- Get Area Risk State --------------------
        [HttpGet("api/Location/GetUserAreaState")]
        [Authorize]
        public IActionResult GetUserAreaState()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var riskState = _locationAnalysisService.GetContainingAreaRiskState(userId);
            if (riskState == null)
                return NotFound("Risk state not found for the user's area.");

            return Ok(new { RiskState = riskState });
        }
    }
}
