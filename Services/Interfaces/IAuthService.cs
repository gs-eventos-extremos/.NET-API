using WeatherEmergencyAPI.DTOs.User;

namespace WeatherEmergencyAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(CreateUserDto createUserDto);
        Task<LoginResponseDto> LoginAsync(LoginUserDto loginDto);
        Task<bool> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto);
        Task<bool> DeleteAccountAsync(int userId);
        string GenerateJwtToken(int userId, string email);
    }
}