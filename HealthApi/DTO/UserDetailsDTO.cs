namespace HealthApi.DTO
{
    public class UserDetailsDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }    
        public int Age { get; set; }
        public string AreaName { get; set; }    
        public string Gender { get; set; }
        public bool IsFlagged { get; set; }
        public bool EPass { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
