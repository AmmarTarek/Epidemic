namespace HealthApi.DTO
{
    public class VaccineRecordAllDto
    {
        public int VaccineId { get; set; }
        public string PatientName { get; set; }
        public string VaccineTypeName { get; set; }
        public int DoseNumber { get; set; }
        public string PatientGender { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }
        public string AreaName { get; set; }
        public string Status { get; set; }
        public DateTime? VaccinationDate { get; set; }
        public DateTime DateOfAssignment {  get; set; }
        public string ClinicName { get; set; }
        public string ClinicContactInfo { get; set; }
        public string VaccineDetails { get; set; }
        public int userId { get; set; }
        
    }
}
