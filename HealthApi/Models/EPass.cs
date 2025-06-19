using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class EPass
    {
        [Key]
        public int EPassID { get; set; }

        public bool Status { get; set; } // True = 1, False = 2

        public string Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }

}
