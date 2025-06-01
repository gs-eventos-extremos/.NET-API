using Microsoft.ML.Data;

namespace WeatherEmergencyAPI.ML.Models
{
    public class WeatherPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool HasDisasterPrediction { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }

        [ColumnName("Score")]
        public float[] Score { get; set; } = Array.Empty<float>();
    }
}