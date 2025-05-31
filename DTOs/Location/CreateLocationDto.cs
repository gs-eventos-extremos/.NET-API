using System.ComponentModel.DataAnnotations;

namespace WeatherEmergencyAPI.DTOs.Location
{
    public class CreateLocationDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Latitude é obrigatória")]
        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Longitude é obrigatória")]
        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        public double Longitude { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public bool IsFavorite { get; set; } = false;
    }
}