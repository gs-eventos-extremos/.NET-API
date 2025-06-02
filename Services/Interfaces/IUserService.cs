using WeatherEmergencyAPI.DTOs.User;

namespace WeatherEmergencyAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
    }
}