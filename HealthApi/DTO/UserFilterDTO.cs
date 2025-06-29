namespace HealthApi.DTO
{
    public class UserFilterDTO
    {
        public string? UserName { get; set; }
        public string? AreaName { get; set; }
        public string? EPass { get; set; }
        public string? Gender { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? IsFlagged { get; set; }
    }
}
