using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Point = NetTopologySuite.Geometries.Point;

namespace HealthApi.Models
{
    public class AnonymousCheckIn
    {
        [Key]
        public int CheckInId { get; set; }
        public Point Location { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }


}
