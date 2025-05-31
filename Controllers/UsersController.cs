using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeatherEmergencyAPI.DTOs.User;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Obtém informações do usuário atual
        /// </summary>
        [HttpGet("{userId}", Name = "GetUser")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetUser(int userId)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId != userId)
            {
                return Forbid();
            }

            // Implementar busca do usuário quando necessário
            return Ok(new { message = "Endpoint será implementado" });
        }

        /// <summary>
        /// Deleta a conta do usuário (soft delete)
        /// </summary>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccount(int userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (currentUserId != userId)
                {
                    return Forbid();
                }

                await _authService.DeleteAccountAsync(userId);
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

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!.Value);
        }
    }
}