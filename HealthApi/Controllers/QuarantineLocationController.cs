using HealthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApi.Controllers
{
    public class QuarantineLocationController : Controller
    {
        private readonly AppDbContext _context;

        public QuarantineLocationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("api/QuarantineLocation/GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var locations = await _context.QuarantineLocations.ToListAsync();

            var result = locations.Select(q => new
            {
                q.LocationName,
                latitude = q.Y, // Y = Latitude
                longitude = q.X // X = Longitude
            });

            return Ok(result);
        }
    }
}
