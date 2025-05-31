using System.ComponentModel.DataAnnotations;

namespace WeatherEmergencyAPI.DTOs.Weather
{
    public class WeatherRequestDto
    {
        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        public double Longitude { get; set; }
    }
}