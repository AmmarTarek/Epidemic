using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class VaccineRecord
    {
        [Key]
        public int RecordId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int VaccineTypeId { get; set; }
        public VaccineType VaccineType { get; set; }

        public string Status { get; set; } = null!; // "Vaccined", "Pending"

        public DateTime DateOfAssignment { get; set; } = DateTime.UtcNow;
        public DateTime? DateOfVaccined { get; set; }

        public int LabId { get; set; }
        public Lab Lab { get; set; } = null!;
    }

}   
