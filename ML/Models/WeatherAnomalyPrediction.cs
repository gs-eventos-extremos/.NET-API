using Microsoft.ML.Data;

namespace WeatherEmergencyAPI.ML.Models
{
    public class WeatherAnomalyPrediction
    {
        [VectorType(3)]
        public double[] Prediction { get; set; } = Array.Empty<double>();

        public bool IsAnomaly => Prediction[0] > 0.5;
        public double RawScore => Prediction[1];
        public double Magnitude => Prediction[2];
    }
}