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

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
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
        /// Cria uma nova localização
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