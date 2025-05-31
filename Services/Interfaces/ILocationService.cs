using WeatherEmergencyAPI.DTOs.Location;

namespace WeatherEmergencyAPI.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationResponseDto>> GetUserLocationsAsync(int userId);
        Task<LocationResponseDto> GetLocationByIdAsync(int userId, int locationId);
        Task<LocationResponseDto> CreateLocationAsync(int userId, CreateLocationDto createLocationDto);
        Task<LocationResponseDto> UpdateLocationAsync(int userId, int locationId, UpdateLocationDto updateLocationDto);
        Task<bool> DeleteLocationAsync(int userId, int locationId);
        Task<IEnumerable<LocationResponseDto>> GetFavoriteLocationsAsync(int userId);
    }
}