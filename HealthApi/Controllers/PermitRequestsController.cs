//using HealthApi.DTO;
//using HealthApi.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace HealthApi.Controllers
//{
//    [ApiController]
//    public class PermitRequestsController : ControllerBase
//    {
//        private readonly AppDbContext context;

//        public PermitRequestsController(AppDbContext context)
//        {
//            this.context = context;
//        }

//        [HttpGet("api/permitRequests/{id}")]
//        public IActionResult GetMyPermitRequests(int id)
//        {
//            try
//            {
//                var requests = context.PermitRequests
//                    .Where(r => r.UserId == id)
//                    .OrderByDescending(r => r.RequestedDate)
//                    .ToList();

//                if (!requests.Any())
//                {
//                    return NotFound("No permit requests found for this user.");
//                }

//                // Optionally project to DTO if needed
//                var result = requests.Select(r => new
//                {
//                    r.PermitId,
//                    r.Purpose,
//                    r.Status,
//                    r.RequestedDate
//                });

//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        [HttpPost("api/permitRequests/filter")]
//        public IActionResult FilterPermitRequests([FromBody] PermitRequestFilterDTO filter)
//        {
//            try
//            {
//                var query = context.PermitRequests
//                    .Include(pr => pr.User) // assuming there's a navigation property
//                    .AsQueryable();

//                // Filter by status
//                if (!string.IsNullOrWhiteSpace(filter.Status))
//                {
//                    query = query.Where(pr => pr.Status.ToLower() == filter.Status.ToLower());
//                }

//                // Filter by area
//                //if (!string.IsNullOrWhiteSpace(filter.AreaName))
//                //{
//                //    query = query.Where(pr => pr.User.AreaName.Contains(filter.AreaName));
//                //}

//                // Filter by user name
//                if (!string.IsNullOrEmpty(filter.UserName))
//                {
//                    query = query.Where(u =>
//                        (u.User.FirstName + " " + u.User.LastName).Contains(filter.UserName));
//                }

//                // Filter by gender
//                if (!string.IsNullOrWhiteSpace(filter.Gender))
//                {
//                    query = query.Where(pr => pr.User.Sex == filter.Gender);
//                }

//                // Filter by age range
//                if (filter.MinAge.HasValue || filter.MaxAge.HasValue)
//                {
//                    var today = DateTime.Today;

//                    if (filter.MinAge.HasValue)
//                    {
//                        var maxDob = today.AddYears(-filter.MinAge.Value);
//                        query = query.Where(pr => pr.User.DateOfBirth <= maxDob);
//                    }

//                    if (filter.MaxAge.HasValue)
//                    {
//                        var minDob = today.AddYears(-filter.MaxAge.Value);
//                        query = query.Where(pr => pr.User.DateOfBirth >= minDob);
//                    }
//                }

//                // IsFlagged filtering
//                if (!string.IsNullOrWhiteSpace(filter.IsFlagged))
//                {
//                    if (bool.TryParse(filter.IsFlagged, out var isFlaggedValue))
//                    {
//                        query = query.Where(u => u.User.IsFlagged == isFlaggedValue);
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

//                    query = query.Where(u => matchingEPassIds.Contains(u.User.EPassStatusId));
//                }

//                query = query.OrderByDescending(pr => pr.RequestedDate);

//                var permitRequests = query.ToList();

//                if (!permitRequests.Any())
//                    return NotFound("No matching permit requests found.");

//                var result = permitRequests.Select(pr => new PermitRequestDTO
//                {
//                    PermitId = pr.PermitId,
//                    UserId = pr.UserId,
//                    FullName = $"{pr.User.FirstName} {pr.User.LastName}",
//                    Age = (int)((DateTime.Today - pr.User.DateOfBirth).TotalDays / 365.25),
//                    //AreaName = pr.User.AreaName,
//                    AreaName = "Cairo",
//                    Gender = pr.User.Sex,
//                    EPass = (bool)(context.EPasses.FirstOrDefault(ep => ep.EPassID == pr.User.EPassStatusId)?.Status ?? false),
//                    IsFlagged = (bool)pr.User.IsFlagged,
//                    Purpose = pr.Purpose,
//                    Status = pr.Status
//                }).ToList();

//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        [HttpPut("api/permitRequests/updateStatus/{id}")]
//        public IActionResult UpdatePermitStatus(int id, [FromBody] PermitStatusUpdateDTO dto)
//        {
//            var permit = context.PermitRequests.FirstOrDefault(p => p.PermitId == id);
//            if (permit == null)
//                return NotFound("Permit request not found.");

//            permit.Status = dto.Status;
//            context.SaveChanges();

//            return Ok("Status updated successfully.");
//        }

//        public class PermitStatusUpdateDTO
//        {
//            public string Status { get; set; }
//        }

//        public class PermitRequestFilterDTO
//        {
//            public string? Status { get; set; }
//            public string? UserName { get; set; }
//            public string? AreaName { get; set; }
//            public string? Gender { get; set; }
//            public int? MinAge { get; set; }
//            public int? MaxAge { get; set; }
//            public string? EPass { get; set; } // Assuming EPass is a boolean, you might want to change this to bool?
//            public string? IsFlagged { get; set; }
//        }
//        public class PermitRequestDTO
//        {
//            public int PermitId { get; set; }
//            public int UserId { get; set; }
//            public string FullName { get; set; }
//            public int Age { get; set; }
//            public string AreaName { get; set; }
//            public string Gender { get; set; }
//            public bool EPass { get; set; }
//            public bool IsFlagged { get; set; }
//            public string Purpose { get; set; }
//            public string Status { get; set; }
//        }



//    }
//}


using HealthApi.DTO;
using HealthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermitRequestsController : ControllerBase
    {
        private readonly AppDbContext context;

        public PermitRequestsController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("myRequests")]
        public IActionResult GetMyPermitRequests()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("Invalid or missing user ID.");
                }

                var requests = context.PermitRequests
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.RequestedDate)
                    .ToList();

                if (!requests.Any())
                    return NotFound("No permit requests found for this user.");

                var result = requests.Select(r => new
                {
                    r.PermitId,
                    r.Purpose,
                    r.Status,
                    r.RequestedDate
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("create")]
        [Authorize] // Requires token
        public IActionResult CreatePermitRequest([FromBody] PermitRequestCreateDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid or missing user ID in token.");
            }

            if (string.IsNullOrWhiteSpace(dto.Purpose))
            {
                return BadRequest("Purpose is required.");
            }

            var latestRequest = context.PermitRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestedDate)
                .FirstOrDefault();

            if (latestRequest != null) 
            {
                if (latestRequest.Status == "Pending") 
                {
                    return BadRequest("Please wait for older request response");
                }
            }

            var newRequest = new PermitRequest
            {
                UserId = userId,
                Purpose = dto.Purpose,
                RequestedDate = DateTime.UtcNow,
                Status = "Pending"
            };

            context.PermitRequests.Add(newRequest);
            context.SaveChanges();

            return Ok(new
            {
                Message = "Permit request created successfully.",
                PermitId = newRequest.PermitId
            });
        }


        // 🔍 Filter permit requests (Admin)
        [HttpPost("filter")]
        public IActionResult FilterPermitRequests([FromBody] PermitRequestFilterDTO filter)
        {
            try
            {
                var query = context.PermitRequests
                    .Include(pr => pr.User)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.Status))
                    query = query.Where(pr => pr.Status.ToLower() == filter.Status.ToLower());

                if (!string.IsNullOrWhiteSpace(filter.UserName))
                    query = query.Where(pr => (pr.User.FirstName + " " + pr.User.LastName).Contains(filter.UserName));

                if (!string.IsNullOrWhiteSpace(filter.Gender))
                    query = query.Where(pr => pr.User.Sex == filter.Gender);

                if (filter.MinAge.HasValue || filter.MaxAge.HasValue)
                {
                    var today = DateTime.Today;
                    if (filter.MinAge.HasValue)
                    {
                        var maxDob = today.AddYears(-filter.MinAge.Value);
                        query = query.Where(pr => pr.User.DateOfBirth <= maxDob);
                    }

                    if (filter.MaxAge.HasValue)
                    {
                        var minDob = today.AddYears(-filter.MaxAge.Value);
                        query = query.Where(pr => pr.User.DateOfBirth >= minDob);
                    }
                }

                if (!string.IsNullOrWhiteSpace(filter.IsFlagged) &&
                    bool.TryParse(filter.IsFlagged, out var isFlagged))
                {
                    query = query.Where(pr => pr.User.IsFlagged == isFlagged);
                }

                if (!string.IsNullOrEmpty(filter.EPass))
                {
                    bool allowed = filter.EPass == "Allowed";
                    var allowedIds = context.EPasses
                        .Where(ep => ep.Status == allowed)
                        .Select(ep => ep.EPassID)
                        .ToList();

                    query = query.Where(pr => allowedIds.Contains(pr.User.EPassStatusId));
                }

                query = query.OrderByDescending(pr => pr.RequestedDate);

                var permitRequests = query.ToList();

                if (!permitRequests.Any())
                    return NotFound("No matching permit requests found.");

                var result = permitRequests.Select(pr => new PermitRequestDTO
                {
                    PermitId = pr.PermitId,
                    UserId = pr.UserId,
                    FullName = $"{pr.User.FirstName} {pr.User.LastName}",
                    Age = (int)((DateTime.Today - pr.User.DateOfBirth).TotalDays / 365.25),
                    AreaName = "Cairo", // Replace with actual data if needed
                    Gender = pr.User.Sex,
                    EPass = context.EPasses.FirstOrDefault(ep => ep.EPassID == pr.User.EPassStatusId)?.Status ?? false,
                    IsFlagged = pr.User.IsFlagged ?? false,
                    Purpose = pr.Purpose,
                    Status = pr.Status
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 🔁 Update permit request status (Admin)
        [HttpPut("updateStatus/{id}")]
        public IActionResult UpdatePermitStatus(int id, [FromBody] PermitStatusUpdateDTO dto)
        {
            var permit = context.PermitRequests.FirstOrDefault(p => p.PermitId == id);
            if (permit == null)
                return NotFound("Permit request not found.");

            permit.Status = dto.Status;

            var user = context.Users.FirstOrDefault(u => u.UserId == permit.UserId);
            if (permit.Status == "Accepted")
            {
                user.EPassStatusId = 2;
            }
            else 
            {
                user.EPassStatusId = 4;
            }
            context.Users.Update(user);

            context.SaveChanges();

            return Ok("Status updated successfully.");
        }

        // 📦 DTOs

        public class PermitStatusUpdateDTO
        {
            public string Status { get; set; }
        }

        public class PermitRequestFilterDTO
        {
            public string? Status { get; set; }
            public string? UserName { get; set; }
            public string? AreaName { get; set; }
            public string? Gender { get; set; }
            public int? MinAge { get; set; }
            public int? MaxAge { get; set; }
            public string? EPass { get; set; } // "Allowed" or "Denied"
            public string? IsFlagged { get; set; } // "true"/"false"
        }

        public class PermitRequestDTO
        {
            public int PermitId { get; set; }
            public int UserId { get; set; }
            public string FullName { get; set; }
            public int Age { get; set; }
            public string AreaName { get; set; }
            public string Gender { get; set; }
            public bool EPass { get; set; }
            public bool IsFlagged { get; set; }
            public string Purpose { get; set; }
            public string Status { get; set; }
        }

        public class PermitRequestCreateDTO
        {
            public string Purpose { get; set; }
        }

    }
}
