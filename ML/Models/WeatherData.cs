using Microsoft.ML.Data;

namespace WeatherEmergencyAPI.ML.Models
{
    public class WeatherData
    {
        [LoadColumn(0)]
        public float Temperature { get; set; }

        [LoadColumn(1)]
        public float Humidity { get; set; }

        [LoadColumn(2)]
        public float Pressure { get; set; }

        [LoadColumn(3)]
        public float WindSpeed { get; set; }

        [LoadColumn(4)]
        public float Precipitation { get; set; }

        [LoadColumn(5)]
        public bool HasDisaster { get; set; } // Label - o que queremos prever
    }
}