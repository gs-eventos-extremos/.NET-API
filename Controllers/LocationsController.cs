using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeatherEmergencyAPI.DTOs.Location;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IWeatherService _weatherService;

        public LocationsController(ILocationService locationService, IWeatherService weatherService)
        {
            _locationService = locationService;
            _weatherService = weatherService;
        }

        /// <summary>
        /// Lista todas as localizações do usuário
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LocationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserLocations(int userId)
        {
            if (!IsAuthorizedUser(userId))
                return Forbid();

            var locations = await _locationService.GetUserLocationsAsync(userId);
            return Ok(locations);
        }

        /// <summary>
        /// Obtém uma localização específica
        /// </summary>
        [HttpGet("{locationId}")]
        [ProducesResponseType(typeof(LocationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetLocation(int userId, int locationId)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                var location = await _locationService.GetLocationByIdAsync(userId, locationId);
                return Ok(location);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lista localizações favoritas do usuário
        /// </summary>
        [HttpGet("favorites")]
        [ProducesResponseType(typeof(IEnumerable<LocationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetFavoriteLocations(int userId)
        {
            if (!IsAuthorizedUser(userId))
                return Forbid();

            var locations = await _locationService.GetFavoriteLocationsAsync(userId);
            return Ok(locations);
        }

        /// <summary>
        /// Cria uma nova localização com coordenadas
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(LocationResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateLocation(int userId, [FromBody] CreateLocationDto createLocationDto)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                var location = await _locationService.CreateLocationAsync(userId, createLocationDto);
                return CreatedAtAction(
                    nameof(GetLocation),
                    new { userId, locationId = location.Id },
                    location);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Cria uma nova localização usando endereço (sem coordenadas)
        /// </summary>
        [HttpPost("by-address")]
        [ProducesResponseType(typeof(LocationResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateLocationByAddress(
            int userId,
            [FromBody] CreateLocationByAddressDto createLocationDto)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                // Obter coordenadas do endereço
                var (latitude, longitude, formattedAddress) = await _weatherService.GetCoordinatesFromAddressAsync(
                    createLocationDto.City,
                    createLocationDto.State,
                    createLocationDto.Country
                );

                // Criar DTO com coordenadas
                var locationWithCoords = new CreateLocationDto
                {
                    Name = createLocationDto.Name,
                    Latitude = latitude,
                    Longitude = longitude,
                    City = createLocationDto.City,
                    State = createLocationDto.State,
                    Country = createLocationDto.Country,
                    IsFavorite = createLocationDto.IsFavorite
                };

                var location = await _locationService.CreateLocationAsync(userId, locationWithCoords);

                // Adicionar informação do endereço formatado na resposta
                location.Links.Add("formatted_address", formattedAddress);

                return CreatedAtAction(
                    nameof(GetLocation),
                    new { userId, locationId = location.Id },
                    location);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    error = "Endereço não encontrado. Verifique cidade, estado e país.",
                    details = ex.Message
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { error = "Serviço de geocoding temporariamente indisponível" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Atualiza uma localização
        /// </summary>
        [HttpPut("{locationId}")]
        [ProducesResponseType(typeof(LocationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateLocation(
            int userId,
            int locationId,
            [FromBody] UpdateLocationDto updateLocationDto)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                var location = await _locationService.UpdateLocationAsync(userId, locationId, updateLocationDto);
                return Ok(location);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Deleta uma localização
        /// </summary>
        [HttpDelete("{locationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteLocation(int userId, int locationId)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                await _locationService.DeleteLocationAsync(userId, locationId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Valida endereço e retorna coordenadas antes de salvar
        /// </summary>
        [HttpPost("validate-address")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidateAddress([FromBody] CreateLocationByAddressDto addressDto)
        {
            try
            {
                var (latitude, longitude, formattedAddress) = await _weatherService.GetCoordinatesFromAddressAsync(
                    addressDto.City,
                    addressDto.State,
                    addressDto.Country
                );

                return Ok(new
                {
                    isValid = true,
                    latitude,
                    longitude,
                    formattedAddress,
                    originalInput = new
                    {
                        city = addressDto.City,
                        state = addressDto.State,
                        country = addressDto.Country
                    }
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    isValid = false,
                    error = "Endereço não encontrado",
                    suggestion = "Verifique a ortografia da cidade, estado e país"
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { error = "Serviço de validação temporariamente indisponível" });
            }
        }

        /// <summary>
        /// Busca sugestões de localizações baseado em texto parcial
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchLocations(
            int userId,
            [FromQuery] string query)
        {
            if (!IsAuthorizedUser(userId))
                return Forbid();

            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            {
                return BadRequest(new { error = "Query deve ter pelo menos 3 caracteres" });
            }

            // Buscar nas localizações salvas do usuário
            var userLocations = await _locationService.GetUserLocationsAsync(userId);

            var filteredLocations = userLocations
                .Where(l =>
                    l.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (l.City?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.State?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Country?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
                .Take(10)
                .Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.City,
                    l.State,
                    l.Country,
                    l.IsFavorite,
                    DisplayName = $"{l.Name} - {l.City}, {l.State}"
                });

            return Ok(filteredLocations);
        }

        private bool IsAuthorizedUser(int userId)
        {
            var currentUserId = GetCurrentUserId();
            return currentUserId == userId;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!.Value);
        }
    }
}