using HealthApi.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System;
using System.Linq;

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
    }
}
