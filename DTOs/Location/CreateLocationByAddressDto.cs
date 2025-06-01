using System.ComponentModel.DataAnnotations;

namespace WeatherEmergencyAPI.DTOs.Location
{
    public class CreateLocationByAddressDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cidade é obrigatória")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "País é obrigatório")]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        public bool IsFavorite { get; set; } = false;
    }
}