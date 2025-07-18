﻿using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace HealthApi.Models

{
    public class RiskArea
    {
        [Key]
        public int AreaId { get; set; }
        public string? AreaName { get; set; }  // e.g., "Downtown", "Uptown"
        public string RiskLevel { get; set; } // "Low", "Medium", "High"
        public string AreaGeometry { get; set; } // Could use GeoJSON or WKT
        public Polygon? Geometry { get; set; }
    }

}
