using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class VaccineType
    {
        [Key]
        public int VaccineId { get; set; }

        public string VaccineName { get; set; }

        public string Description { get; set; }
    }

}
