using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
            
        public int? TargetAreaId { get; set; }
        public RiskArea? TargetArea { get; set; }

        public int? TatgetUserId { get; set; }
        public User? TatgetUser { get; set; }
    }

}
