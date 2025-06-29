namespace HealthApi.DTO
{
    public class UserRegisterDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; } = null!;
        public string Job { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int RoleId { get; set; } = 1;
    }


    public class UserLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

}
