using System.ComponentModel.DataAnnotations;

namespace WeatherEmergencyAPI.DTOs.User
{
    public class UpdatePasswordDto
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Senha deve conter pelo menos uma letra maiúscula, uma minúscula e um número")]
        public string NewPassword { get; set; } = string.Empty;
    }
}