using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class QuarantineLocation
    {
        [Key]
        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public string Type { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
    }

}
