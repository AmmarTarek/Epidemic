using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class Lab
    {
        [Key]
        public int LabId { get; set; }

        public string LabName { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public string ContactInfo { get; set; }
    }

}
