namespace HealthApi.DTO
{
    public class AllTestRecordsDTO
    {
        public int TestId { get; set; }
        public string TestTypeName { get; set; }
        public string Result { get; set; }
        public string LabName { get; set; }
        public string LabContactInfo { get; set; }
        public string TestDetails { get; set; }
        public DateTime TestDate { get; set; }
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }
        public string AreaName { get; set; }

    }
}
