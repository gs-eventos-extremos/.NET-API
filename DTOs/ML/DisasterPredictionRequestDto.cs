using System.ComponentModel.DataAnnotations;

namespace WeatherEmergencyAPI.DTOs.ML
{
    public class DisasterPredictionRequestDto
    {
        [Required]
        [Range(-50, 60, ErrorMessage = "Temperatura deve estar entre -50°C e 60°C")]
        public float Temperature { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Umidade deve estar entre 0% e 100%")]
        public float Humidity { get; set; }

        [Required]
        [Range(950, 1050, ErrorMessage = "Pressão deve estar entre 950 e 1050 hPa")]
        public float Pressure { get; set; }

        [Required]
        [Range(0, 300, ErrorMessage = "Velocidade do vento deve estar entre 0 e 300 km/h")]
        public float WindSpeed { get; set; }

        [Required]
        [Range(0, 500, ErrorMessage = "Precipitação deve estar entre 0 e 500mm")]
        public float Precipitation { get; set; }
    }
}