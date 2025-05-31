using Microsoft.EntityFrameworkCore;
using WeatherEmergencyAPI.Data;
using WeatherEmergencyAPI.Models;
using WeatherEmergencyAPI.Repositories.Interfaces;

namespace WeatherEmergencyAPI.Repositories
{
    public class EmergencyContactRepository : BaseRepository<EmergencyContact>, IEmergencyContactRepository
    {
        public EmergencyContactRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EmergencyContact>> GetUserContactsAsync(int userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<EmergencyContact?> GetUserContactByIdAsync(int userId, int contactId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Id == contactId && c.UserId == userId);
        }

        public async Task<EmergencyContact?> GetUserPrimaryContactAsync(int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsPrimary);
        }
    }
}