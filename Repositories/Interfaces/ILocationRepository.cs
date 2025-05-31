using WeatherEmergencyAPI.Models;

namespace WeatherEmergencyAPI.Repositories.Interfaces
{
    public interface ILocationRepository : IBaseRepository<Location>
    {
        Task<IEnumerable<Location>> GetUserLocationsAsync(int userId);
        Task<IEnumerable<Location>> GetUserFavoriteLocationsAsync(int userId);
        Task<Location?> GetUserLocationByIdAsync(int userId, int locationId);
    }
}