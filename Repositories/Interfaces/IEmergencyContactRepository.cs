using WeatherEmergencyAPI.Models;

namespace WeatherEmergencyAPI.Repositories.Interfaces
{
    public interface IEmergencyContactRepository : IBaseRepository<EmergencyContact>
    {
        Task<IEnumerable<EmergencyContact>> GetUserContactsAsync(int userId);
        Task<EmergencyContact?> GetUserContactByIdAsync(int userId, int contactId);
        Task<EmergencyContact?> GetUserPrimaryContactAsync(int userId);
    }
}