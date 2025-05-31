using Microsoft.AspNetCore.Mvc;

namespace WeatherEmergencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Verifica se a API está online
        /// </summary>
        /// <returns>Status da API</returns>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                service = "Weather Emergency API",
                version = "1.0.0"
            });
        }

        /// <summary>
        /// Obtém informações sobre a API
        /// </summary>
        [HttpGet("info")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetApiInfo()
        {
            return Ok(new
            {
                api = "Weather Emergency API",
                version = "1.0.0",
                description = "API para gerenciamento de alertas climáticos e contatos de emergência",
                endpoints = new
                {
                    health = "/api/health",
                    auth = new
                    {
                        register = "POST /api/auth/register",
                        login = "POST /api/auth/login",
                        updatePassword = "PUT /api/auth/update-password"
                    },
                    users = new
                    {
                        getUser = "GET /api/users/{userId}",
                        deleteUser = "DELETE /api/users/{userId}"
                    },
                    locations = new
                    {
                        list = "GET /api/users/{userId}/locations",
                        get = "GET /api/users/{userId}/locations/{locationId}",
                        create = "POST /api/users/{userId}/locations",
                        update = "PUT /api/users/{userId}/locations/{locationId}",
                        delete = "DELETE /api/users/{userId}/locations/{locationId}",
                        favorites = "GET /api/users/{userId}/locations/favorites"
                    },
                    emergencyContacts = new
                    {
                        list = "GET /api/users/{userId}/emergency-contacts",
                        get = "GET /api/users/{userId}/emergency-contacts/{contactId}",
                        create = "POST /api/users/{userId}/emergency-contacts",
                        update = "PUT /api/users/{userId}/emergency-contacts/{contactId}",
                        delete = "DELETE /api/users/{userId}/emergency-contacts/{contactId}"
                    }
                }
            });
        }
    }
}