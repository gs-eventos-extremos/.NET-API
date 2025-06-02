using AutoMapper;
using WeatherEmergencyAPI.DTOs.User;
using WeatherEmergencyAPI.Repositories.Interfaces;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            var userDto = _mapper.Map<UserResponseDto>(user);

            // Adicionar HATEOAS links
            userDto.Links = new Dictionary<string, object>
            {
                { "self", new { href = $"/api/users/{userId}", method = "GET" } },
                { "update", new { href = $"/api/users/{userId}", method = "PUT" } },
                { "update_password", new { href = "/api/auth/update-password", method = "PUT" } },
                { "delete", new { href = $"/api/users/{userId}", method = "DELETE" } },
                { "locations", new { href = $"/api/users/{userId}/locations", method = "GET" } },
                { "emergency_contacts", new { href = $"/api/users/{userId}/emergency-contacts", method = "GET" } }
            };

            return userDto;
        }

        public async Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            // Verificar se o novo email já existe (se estiver mudando o email)
            if (!string.IsNullOrEmpty(updateUserDto.Email) &&
                updateUserDto.Email.ToLower() != user.Email.ToLower())
            {
                if (await _userRepository.EmailExistsAsync(updateUserDto.Email))
                {
                    throw new InvalidOperationException("Email já está em uso");
                }
                user.Email = updateUserDto.Email;
            }

            // Atualizar nome se fornecido
            if (!string.IsNullOrEmpty(updateUserDto.Name))
            {
                user.Name = updateUserDto.Name;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return await GetUserByIdAsync(userId);
        }
    }
}