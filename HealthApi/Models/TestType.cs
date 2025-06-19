using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class TestType
    {
        [Key]
        public int TestId { get; set; }

        public string TestName { get; set; }

        public string Description { get; set; }
    }

}
