using System.ComponentModel.DataAnnotations;

namespace WeatherEmergencyAPI.DTOs.EmergencyContact
{
    public class CreateEmergencyContactDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número de telefone é obrigatório")]
        [Phone(ErrorMessage = "Número de telefone inválido")]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "País é obrigatório")]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Relationship { get; set; }

        public bool IsPrimary { get; set; } = false;
    }
}