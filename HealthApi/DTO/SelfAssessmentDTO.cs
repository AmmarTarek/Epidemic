namespace HealthApi.DTO
{
    public class SelfAssessmentDTO
    {
        public int AssessmentId { get; set; }
        public int UserId { get; set; }
        public string Symptoms { get; set; }
        public bool IsFlagged { get; set; }
        public DateTime Date { get; set; }

        public string UserFullName { get; set; }
    }
}
