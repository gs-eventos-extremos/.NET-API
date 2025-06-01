namespace WeatherEmergencyAPI.DTOs.ML
{
    public class DisasterPredictionResponseDto
    {
        public bool HasHighRisk { get; set; }
        public float RiskProbability { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public List<string> PotentialHazards { get; set; } = new();
        public DateTime PredictionTime { get; set; }
        public Dictionary<string, object> Links { get; set; } = new();
    }
}