using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class AnonymousCheckIn
    {
        [Key]
        public int CheckInId { get; set; }

        public string QRCodeId { get; set; }

        public string Location { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime Time { get; set; } = DateTime.UtcNow;
    }

}
