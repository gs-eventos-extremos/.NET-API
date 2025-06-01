using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherEmergencyAPI.DTOs.User;
using WeatherEmergencyAPI.Models;
using WeatherEmergencyAPI.Repositories.Interfaces;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMessageBusService _messageBusService;

        public AuthService(
            IUserRepository userRepository,
            IMapper mapper,
            IConfiguration configuration,
            IMessageBusService messageBusService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _messageBusService = messageBusService;
        }

        public async Task<UserResponseDto> RegisterAsync(CreateUserDto createUserDto)
        {
            // Verificar se email já existe
            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            {
                throw new InvalidOperationException("Email já cadastrado");
            }

            // Criar novo usuário
            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            var createdUser = await _userRepository.AddAsync(user);

            // Publicar mensagem no RabbitMQ
            await _messageBusService.PublishUserRegistered(createdUser.Id, createdUser.Email, createdUser.Name);

            var userResponse = _mapper.Map<UserResponseDto>(createdUser);

            // Adicionar HATEOAS links
            userResponse.Links = new Dictionary<string, object>
            {
                { "self", new { href = $"/api/users/{userResponse.Id}", method = "GET" } },
                { "update_password", new { href = "/api/auth/update-password", method = "PUT" } },
                { "delete", new { href = $"/api/users/{userResponse.Id}", method = "DELETE" } },
                { "login", new { href = "/api/auth/login", method = "POST" } }
            };

            return userResponse;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginUserDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            var token = GenerateJwtToken(user.Id, user.Email);
            var userResponse = _mapper.Map<UserResponseDto>(user);

            // Adicionar HATEOAS links
            userResponse.Links = new Dictionary<string, object>
            {
                { "self", new { href = $"/api/users/{userResponse.Id}", method = "GET" } },
                { "update_password", new { href = "/api/auth/update-password", method = "PUT" } },
                { "delete", new { href = $"/api/users/{userResponse.Id}", method = "DELETE" } },
                { "locations", new { href = $"/api/users/{userResponse.Id}/locations", method = "GET" } },
                { "emergency_contacts", new { href = $"/api/users/{userResponse.Id}/emergency-contacts", method = "GET" } }
            };

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JwtSettings:ExpirationInHours"])),
                User = userResponse
            };
        }

        public async Task<bool> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto)
        {
            var user = await _userRepository.GetByEmailAsync(updatePasswordDto.Email);

            if (user == null || !user.IsActive)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            // Soft delete
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public string GenerateJwtToken(int userId, string email)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]!);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim("UserId", userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JwtSettings:ExpirationInHours"])),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}