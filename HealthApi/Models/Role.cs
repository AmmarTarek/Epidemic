using System.ComponentModel.DataAnnotations;
namespace HealthApi.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        public string RoleName { get; set; } = null!; // "Admin", "User", etc.
    }

}
