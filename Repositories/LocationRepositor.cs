using Microsoft.EntityFrameworkCore;
using WeatherEmergencyAPI.Data;
using WeatherEmergencyAPI.Models;
using WeatherEmergencyAPI.Repositories.Interfaces;

namespace WeatherEmergencyAPI.Repositories
{
    public class LocationRepository : BaseRepository<Location>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Location>> GetUserLocationsAsync(int userId)
        {
            return await _dbSet
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.IsFavorite)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetUserFavoriteLocationsAsync(int userId)
        {
            return await _dbSet
                .Where(l => l.UserId == userId && l.IsFavorite)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<Location?> GetUserLocationByIdAsync(int userId, int locationId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(l => l.Id == locationId && l.UserId == userId);
        }
    }
}