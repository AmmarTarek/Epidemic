using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class SelfAssessment
    {
        [Key]
        public int AssessmentId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string Symptoms { get; set; }

        public bool IsFlagged { get; set; }
    }

}
