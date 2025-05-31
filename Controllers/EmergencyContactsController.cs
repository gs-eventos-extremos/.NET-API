using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeatherEmergencyAPI.DTOs.EmergencyContact;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Controllers
{
    [Route("api/users/{userId}/emergency-contacts")]
    [ApiController]
    [Authorize]
    public class EmergencyContactsController : ControllerBase
    {
        private readonly IEmergencyContactService _contactService;

        public EmergencyContactsController(IEmergencyContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Lista todos os contatos de emergência do usuário
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmergencyContactResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserContacts(int userId)
        {
            if (!IsAuthorizedUser(userId))
                return Forbid();

            var contacts = await _contactService.GetUserContactsAsync(userId);
            return Ok(contacts);
        }

        /// <summary>
        /// Obtém um contato de emergência específico
        /// </summary>
        [HttpGet("{contactId}")]
        [ProducesResponseType(typeof(EmergencyContactResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetContact(int userId, int contactId)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                var contact = await _contactService.GetContactByIdAsync(userId, contactId);
                return Ok(contact);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo contato de emergência
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EmergencyContactResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateContact(
            int userId,
            [FromBody] CreateEmergencyContactDto createContactDto)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                var contact = await _contactService.CreateContactAsync(userId, createContactDto);
                return CreatedAtAction(
                    nameof(GetContact),
                    new { userId, contactId = contact.Id },
                    contact);
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
        /// Atualiza um contato de emergência
        /// </summary>
        [HttpPut("{contactId}")]
        [ProducesResponseType(typeof(EmergencyContactResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateContact(
            int userId,
            int contactId,
            [FromBody] CreateEmergencyContactDto updateContactDto)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                var contact = await _contactService.UpdateContactAsync(userId, contactId, updateContactDto);
                return Ok(contact);
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
        /// Deleta um contato de emergência
        /// </summary>
        [HttpDelete("{contactId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteContact(int userId, int contactId)
        {
            try
            {
                if (!IsAuthorizedUser(userId))
                    return Forbid();

                await _contactService.DeleteContactAsync(userId, contactId);
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