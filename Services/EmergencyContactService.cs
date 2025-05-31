using AutoMapper;
using WeatherEmergencyAPI.DTOs.EmergencyContact;
using WeatherEmergencyAPI.Models;
using WeatherEmergencyAPI.Repositories.Interfaces;
using WeatherEmergencyAPI.Services.Interfaces;
using WeatherEmergencyAPI.Utils;

namespace WeatherEmergencyAPI.Services
{
    public class EmergencyContactService : IEmergencyContactService
    {
        private readonly IEmergencyContactRepository _contactRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public EmergencyContactService(
            IEmergencyContactRepository contactRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _contactRepository = contactRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmergencyContactResponseDto>> GetUserContactsAsync(int userId)
        {
            var contacts = await _contactRepository.GetUserContactsAsync(userId);
            var contactDtos = _mapper.Map<IEnumerable<EmergencyContactResponseDto>>(contacts);

            foreach (var contact in contactDtos)
            {
                contact.Links = GenerateContactLinks(userId, contact.Id);
            }

            return contactDtos;
        }

        public async Task<EmergencyContactResponseDto> GetContactByIdAsync(int userId, int contactId)
        {
            var contact = await _contactRepository.GetUserContactByIdAsync(userId, contactId);

            if (contact == null)
            {
                throw new KeyNotFoundException("Contato não encontrado");
            }

            var contactDto = _mapper.Map<EmergencyContactResponseDto>(contact);
            contactDto.Links = GenerateContactLinks(userId, contactId);

            return contactDto;
        }

        public async Task<EmergencyContactResponseDto> CreateContactAsync(int userId, CreateEmergencyContactDto createContactDto)
        {
            // Verificar se usuário existe
            var userExists = await _userRepository.ExistsAsync(u => u.Id == userId && u.IsActive);
            if (!userExists)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            // Se marcado como primário, desmarcar outros
            if (createContactDto.IsPrimary)
            {
                var currentPrimary = await _contactRepository.GetUserPrimaryContactAsync(userId);
                if (currentPrimary != null)
                {
                    currentPrimary.IsPrimary = false;
                    await _contactRepository.UpdateAsync(currentPrimary);
                }
            }

            var contact = _mapper.Map<EmergencyContact>(createContactDto);
            contact.UserId = userId;
            contact.CountryCode = CountryService.GetCountryCode(createContactDto.Country);
            contact.CreatedAt = DateTime.UtcNow;

            var createdContact = await _contactRepository.AddAsync(contact);
            var contactDto = _mapper.Map<EmergencyContactResponseDto>(createdContact);
            contactDto.Links = GenerateContactLinks(userId, createdContact.Id);

            return contactDto;
        }

        public async Task<EmergencyContactResponseDto> UpdateContactAsync(int userId, int contactId, CreateEmergencyContactDto updateContactDto)
        {
            var contact = await _contactRepository.GetUserContactByIdAsync(userId, contactId);

            if (contact == null)
            {
                throw new KeyNotFoundException("Contato não encontrado");
            }

            // Se marcado como primário, desmarcar outros
            if (updateContactDto.IsPrimary && !contact.IsPrimary)
            {
                var currentPrimary = await _contactRepository.GetUserPrimaryContactAsync(userId);
                if (currentPrimary != null && currentPrimary.Id != contactId)
                {
                    currentPrimary.IsPrimary = false;
                    await _contactRepository.UpdateAsync(currentPrimary);
                }
            }

            _mapper.Map(updateContactDto, contact);
            contact.CountryCode = CountryService.GetCountryCode(updateContactDto.Country);
            contact.UpdatedAt = DateTime.UtcNow;

            await _contactRepository.UpdateAsync(contact);

            var contactDto = _mapper.Map<EmergencyContactResponseDto>(contact);
            contactDto.Links = GenerateContactLinks(userId, contactId);

            return contactDto;
        }

        public async Task<bool> DeleteContactAsync(int userId, int contactId)
        {
            var contact = await _contactRepository.GetUserContactByIdAsync(userId, contactId);

            if (contact == null)
            {
                throw new KeyNotFoundException("Contato não encontrado");
            }

            await _contactRepository.DeleteAsync(contact);
            return true;
        }

        private Dictionary<string, object> GenerateContactLinks(int userId, int contactId)
        {
            return new Dictionary<string, object>
            {
                { "self", new { href = $"/api/users/{userId}/emergency-contacts/{contactId}", method = "GET" } },
                { "update", new { href = $"/api/users/{userId}/emergency-contacts/{contactId}", method = "PUT" } },
                { "delete", new { href = $"/api/users/{userId}/emergency-contacts/{contactId}", method = "DELETE" } },
                { "user", new { href = $"/api/users/{userId}", method = "GET" } },
                { "all_contacts", new { href = $"/api/users/{userId}/emergency-contacts", method = "GET" } }
            };
        }
    }
}