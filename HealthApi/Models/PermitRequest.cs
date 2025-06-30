using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class PermitRequest
    {
        [Key]
        public int PermitId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Purpose { get; set; }

        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Denied"
    }

}
