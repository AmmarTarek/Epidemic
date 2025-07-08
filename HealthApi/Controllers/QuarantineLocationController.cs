using HealthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    public class QuarantineLocationController : Controller
    {
        private readonly AppDbContext context;

        public QuarantineLocationController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("api/QuarantineLocation/GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var locations = await context.QuarantineLocations.ToListAsync();

            var result = locations.Select(q => new
            {
                q.LocationName,
                latitude = q.Y, // Y = Latitude
                longitude = q.X // X = Longitude
            });

            return Ok(result);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of Earth in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in km
        }

        private double ToRadians(double deg) => deg * (Math.PI / 180);



    [HttpGet("api/QuarantineLocation/closestQuarantine")]
    [Authorize]
    public IActionResult GetClosestFacilityFromToken()
    {
        // 1. Get user ID from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("Invalid token: user ID missing.");

        int userId = int.Parse(userIdClaim.Value);

        // 2. Get user and their location
        var user = context.Users
            .Include(u => u.UserLocation)
            .FirstOrDefault(u => u.UserId == userId);

        if (user == null || user.UserLocation == null)
            return NotFound("User or user location not found.");

        double userLat = user.UserLocation.Latitude;
        double userLon = user.UserLocation.Longitude;

        // 3. Get all quarantine locations
        var allLocations = context.QuarantineLocations.ToList();
        if (!allLocations.Any())
            return NotFound("No quarantine locations available.");

        // 4. Find the closest one
        var closest = allLocations
            .Select(loc => new
            {
                Location = loc,
                Distance = CalculateDistance(userLat, userLon, loc.Y, loc.X)
            })
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        // 5. Return the closest location
        return Ok(new
        {
            closest.Location.LocationId,
            closest.Location.LocationName,
            closest.Location.Type,
            closest.Location.X,
            closest.Location.Y,
            userLat,
            userLon,
            DistanceInKm = Math.Round(closest.Distance, 2)
        });
    }



}
}
