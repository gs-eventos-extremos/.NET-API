using AutoMapper;
using WeatherEmergencyAPI.DTOs.Location;
using WeatherEmergencyAPI.Models;
using WeatherEmergencyAPI.Repositories.Interfaces;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository, IUserRepository userRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationResponseDto>> GetUserLocationsAsync(int userId)
        {
            var locations = await _locationRepository.GetUserLocationsAsync(userId);
            var locationDtos = _mapper.Map<IEnumerable<LocationResponseDto>>(locations);

            foreach (var location in locationDtos)
            {
                location.Links = GenerateLocationLinks(userId, location.Id);
            }

            return locationDtos;
        }

        public async Task<LocationResponseDto> GetLocationByIdAsync(int userId, int locationId)
        {
            var location = await _locationRepository.GetUserLocationByIdAsync(userId, locationId);

            if (location == null)
            {
                throw new KeyNotFoundException("Localização não encontrada");
            }

            var locationDto = _mapper.Map<LocationResponseDto>(location);
            locationDto.Links = GenerateLocationLinks(userId, locationId);

            return locationDto;
        }

        public async Task<LocationResponseDto> CreateLocationAsync(int userId, CreateLocationDto createLocationDto)
        {
            // Verificar se usuário existe
            var userExists = await _userRepository.ExistsAsync(u => u.Id == userId && u.IsActive);
            if (!userExists)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            var location = _mapper.Map<Location>(createLocationDto);
            location.UserId = userId;
            location.CreatedAt = DateTime.UtcNow;

            var createdLocation = await _locationRepository.AddAsync(location);
            var locationDto = _mapper.Map<LocationResponseDto>(createdLocation);
            locationDto.Links = GenerateLocationLinks(userId, createdLocation.Id);

            return locationDto;
        }

        public async Task<LocationResponseDto> UpdateLocationAsync(int userId, int locationId, UpdateLocationDto updateLocationDto)
        {
            var location = await _locationRepository.GetUserLocationByIdAsync(userId, locationId);

            if (location == null)
            {
                throw new KeyNotFoundException("Localização não encontrada");
            }

            _mapper.Map(updateLocationDto, location);
            location.UpdatedAt = DateTime.UtcNow;

            await _locationRepository.UpdateAsync(location);

            var locationDto = _mapper.Map<LocationResponseDto>(location);
            locationDto.Links = GenerateLocationLinks(userId, locationId);

            return locationDto;
        }

        public async Task<bool> DeleteLocationAsync(int userId, int locationId)
        {
            var location = await _locationRepository.GetUserLocationByIdAsync(userId, locationId);

            if (location == null)
            {
                throw new KeyNotFoundException("Localização não encontrada");
            }

            await _locationRepository.DeleteAsync(location);
            return true;
        }

        public async Task<IEnumerable<LocationResponseDto>> GetFavoriteLocationsAsync(int userId)
        {
            var locations = await _locationRepository.GetUserFavoriteLocationsAsync(userId);
            var locationDtos = _mapper.Map<IEnumerable<LocationResponseDto>>(locations);

            foreach (var location in locationDtos)
            {
                location.Links = GenerateLocationLinks(userId, location.Id);
            }

            return locationDtos;
        }

        private Dictionary<string, object> GenerateLocationLinks(int userId, int locationId)
        {
            return new Dictionary<string, object>
            {
                { "self", new { href = $"/api/users/{userId}/locations/{locationId}", method = "GET" } },
                { "update", new { href = $"/api/users/{userId}/locations/{locationId}", method = "PUT" } },
                { "delete", new { href = $"/api/users/{userId}/locations/{locationId}", method = "DELETE" } },
                { "weather", new { href = $"/api/weather/location/{locationId}", method = "GET" } },
                { "user", new { href = $"/api/users/{userId}", method = "GET" } }
            };
        }
    }
}