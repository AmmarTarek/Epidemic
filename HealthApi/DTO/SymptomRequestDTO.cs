namespace HealthApi.DTO
{
    public class SymptomRequestDTO
    {
        public bool Fever { get; set; }
        public bool Cough { get; set; }
        public bool BreathDifficulty { get; set; }
        public bool LossTasteSmell { get; set; }
        public bool Fatigue { get; set; }
        public bool MuscleAches { get; set; }
        public bool SoreThroat { get; set; }
        public bool RunnyNose { get; set; }
        public bool Nausea { get; set; }
        public bool ContactPositive { get; set; }
    }
}
