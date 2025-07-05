namespace HealthApi.DTO
{
    public class TestRecordDetailsDTO
    {
        public int TestId { get; set; }
        public string TestTypeName { get; set; }
        public string? TestDetails { get; set; }
        public string Result { get; set; }
        public DateTime DateAdministered { get; set; }
        public string? LabName { get; set; }
        public string? ContactInfo { get; set; }

    }
}
