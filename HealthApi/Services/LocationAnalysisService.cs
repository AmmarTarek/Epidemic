using HealthApi.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApi.Services
{
    public class LocationAnalysisService
    {
        private readonly AppDbContext _context;
        private readonly GeometryFactory _geometryFactory;

        public LocationAnalysisService(AppDbContext context)
        {
            _context = context;

            // Assuming all geometries are in SRID 4326
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        }

        public string? GetContainingAreaName(int userId)
        {
            if (userId == null) return null;
            var userLocation = _context.UserLocations
                                       .FirstOrDefault(u => u.UserId == userId);

            if (userLocation?.Latitude == null || userLocation?.Longitude == null)
            {
                return null;
            }

            var point = _geometryFactory.CreatePoint(new Coordinate(
                (double)userLocation.Longitude,
                (double)userLocation.Latitude
            ));

            var containingArea = _context.RiskAreas
                                         .AsNoTracking()
                                         .FirstOrDefault(area => area.Geometry.Contains(point));

            return containingArea?.AreaName;
        }

        public string? GetContainingAreaRiskState(int userId)
        {
            var userLocation = _context.UserLocations
                                       .FirstOrDefault(u => u.UserId == userId);

            if (userLocation?.Latitude == null || userLocation?.Longitude == null)
            {
                return null;
            }

            var point = _geometryFactory.CreatePoint(new Coordinate(
                (double)userLocation.Longitude,
                (double)userLocation.Latitude
            ));

            var containingArea = _context.RiskAreas
                                         .AsNoTracking()
                                         .FirstOrDefault(area => area.Geometry.Contains(point));

            return containingArea?.RiskLevel;
        }

        public Lab? GetClosestLab(int userId)
        {

            if ( userId == null)
                return null;

            // 2. Get user and their location
            var user = _context.Users
                .Include(u => u.UserLocation)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.UserLocation == null)
                return null;

            double userLat = user.UserLocation.Latitude;
            double userLon = user.UserLocation.Longitude;

            // 3. Get all quarantine locations
            var allLocations = _context.Labs.ToList();
            if (!allLocations.Any())
                return null;

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
            return closest.Location;

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


        public async Task<List<int>> TraceExposedUsers(int infectedUserId, DateTime testDate)
        {
            var exposureRadius = 50.0; // meters
            var timeWindow = TimeSpan.FromHours(1);
            var daysBack = 5;

            var startDate = testDate.AddDays(-daysBack);
            var endDate = testDate;

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            // Step 1: Get infected user's check-ins in last 5 days
            var infectedCheckIns = await _context.AnonymousCheckIns
                .Where(ci => ci.UserId == infectedUserId && ci.Time >= startDate && ci.Time <= endDate)
                .ToListAsync();

            var potentiallyExposedUsers = new HashSet<int>();

            foreach (var checkIn in infectedCheckIns)
            {
                var buffer = checkIn.Location.Buffer(exposureRadius / 111_000); // ~meters to degrees

                var minTime = checkIn.Time - timeWindow;
                var maxTime = checkIn.Time + timeWindow;

                // Step 2: Find other check-ins within buffer and time window
                var nearbyCheckIns = await _context.AnonymousCheckIns
                    .Where(ci =>
                        ci.UserId != infectedUserId &&
                        ci.Time >= minTime &&
                        ci.Time <= maxTime &&
                        ci.Location.IsWithinDistance(checkIn.Location, exposureRadius / 111_000))
                    .Select(ci => ci.UserId)
                    .ToListAsync();

                foreach (var uid in nearbyCheckIns)
                    potentiallyExposedUsers.Add(uid);
            }

            return potentiallyExposedUsers.ToList();
        }

        internal async Task<IEnumerable<object>> TraceExposedUsers(object userId, object testDate)
        {
            throw new NotImplementedException();
        }
    }

}
