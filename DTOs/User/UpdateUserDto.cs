using System.ComponentModel.DataAnnotations;

namespace WeatherEmergencyAPI.DTOs.User
{
    public class UpdateUserDto
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150)]
        public string? Email { get; set; }
    }
}