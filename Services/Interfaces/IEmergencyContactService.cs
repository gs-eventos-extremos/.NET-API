using WeatherEmergencyAPI.DTOs.EmergencyContact;

namespace WeatherEmergencyAPI.Services.Interfaces
{
    public interface IEmergencyContactService
    {
        Task<IEnumerable<EmergencyContactResponseDto>> GetUserContactsAsync(int userId);
        Task<EmergencyContactResponseDto> GetContactByIdAsync(int userId, int contactId);
        Task<EmergencyContactResponseDto> CreateContactAsync(int userId, CreateEmergencyContactDto createContactDto);
        Task<EmergencyContactResponseDto> UpdateContactAsync(int userId, int contactId, CreateEmergencyContactDto updateContactDto);
        Task<bool> DeleteContactAsync(int userId, int contactId);
    }
}