using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class TestRecord
    {
        [Key]
        public int RecordId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int TestTypeId { get; set; }
        public TestType TestType { get; set; } = null!;

        public string Result { get; set; } = null!;
        public string Details { get; set; } = null!;

        public DateTime DateAdministered { get; set; } = DateTime.UtcNow;

        public int LabId { get; set; }
        public Lab Lab { get; set; } = null!;
    }

}
