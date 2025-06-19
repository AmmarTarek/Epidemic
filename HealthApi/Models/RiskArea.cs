using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class RiskArea
    {
        [Key]
        public int AreaId { get; set; }

        public string RiskLevel { get; set; } // "Low", "Medium", "High"

        public string AreaGeometry { get; set; } // Could use GeoJSON or WKT
    }

}
