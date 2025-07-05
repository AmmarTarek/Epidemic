namespace HealthApi.DTO
{
    public class VaccineRecordDTO
    {
        public string VaccineTypeName { get; set; }
        public string Status { get; set; }
        public DateTime DateOfAssignment { get; set; }
        public DateTime? DateOfVaccined { get; set; }
        public string LabName { get; set; }
        public string ContactInfo { get; set; }
        public string StatusMessage { get; set; }
    }
}
