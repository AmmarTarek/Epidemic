using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class UserLocation
    {
        [Key]                                   
        public int LocationId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime CreatedAtTime { get; set; } = DateTime.UtcNow;
    }

}
