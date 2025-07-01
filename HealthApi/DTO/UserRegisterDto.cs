using System.ComponentModel.DataAnnotations;

namespace HealthApi.DTO
{
    public class UserRegisterDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; } = null!;
        public string Job { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }


    public class UserLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

}
