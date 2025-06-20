namespace HealthApi.DTO
{
    public class VaccineRecordsDTO
    {
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public string? UserFullName { get; set; } 
        public string? UserSex { get; set; } 
        public int UserAge { get; set; }

        public int VaccineTypeId { get; set; }
        public string? VaccineTypeName { get; set; } 

        public string Status { get; set; } 

        public DateTime DateOfAssignment { get; set; }
        public DateTime? DateOfVaccined { get; set; }

        public int LabId { get; set; }
        public string? LabName { get; set; } 
    }
}
