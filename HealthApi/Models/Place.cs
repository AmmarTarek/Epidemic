using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class Place
    {
        [Key]
        public int PlaceId { get; set; }

        public string PlaceName { get; set; }

        public string PlaceType { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
    }

}
