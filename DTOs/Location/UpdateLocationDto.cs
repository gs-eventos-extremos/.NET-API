using System.ComponentModel.DataAnnotations;

namespace WeatherEmergencyAPI.DTOs.Location
{
    public class UpdateLocationDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        public double? Longitude { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public bool? IsFavorite { get; set; }
    }
}