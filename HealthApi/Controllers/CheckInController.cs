using HealthApi.Models;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GeometryFactory _geometryFactory;

        public CheckInController(AppDbContext context)
        {
            _context = context;
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CreateCheckIn([FromBody] CheckInDto dto)
        {
            //// Get user ID (you may use claims or pass it explicitly for now)
            //if (!int.TryParse(User.FindFirst("id")?.Value, out int userId))
            //{
            //    return Unauthorized("User ID not found.");
            //}

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Invalid or missing user ID.");
            }

            var point = _geometryFactory.CreatePoint(new Coordinate(dto.X, dto.Y));

            var checkIn = new AnonymousCheckIn
            {
                UserId = userId,
                Location = point,
                Time = dto.Time ?? DateTime.UtcNow
            };

            _context.AnonymousCheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Check-in successful", checkInId = checkIn.CheckInId });
        }

        public class CheckInDto
        {
            public double X { get; set; } // Longitude
            public double Y { get; set; } // Latitude
            public DateTime? Time { get; set; } // Optional, defaults to now
        }

    }

}
