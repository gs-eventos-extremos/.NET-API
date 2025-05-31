using WeatherEmergencyAPI.Models;

namespace WeatherEmergencyAPI.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdWithRelationsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}