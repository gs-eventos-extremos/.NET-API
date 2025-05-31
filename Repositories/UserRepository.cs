using Microsoft.EntityFrameworkCore;
using WeatherEmergencyAPI.Data;
using WeatherEmergencyAPI.Models;
using WeatherEmergencyAPI.Repositories.Interfaces;

namespace WeatherEmergencyAPI.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .Include(u => u.SavedLocations)
                .Include(u => u.EmergencyContacts)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }
    }
}