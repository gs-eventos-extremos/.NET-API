using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeatherEmergencyAPI.DTOs;

namespace WeatherEmergencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateLimitController : ControllerBase
    {
        private readonly IOptions<IpRateLimitOptions> _ipOptions;
        private readonly IOptions<ClientRateLimitOptions> _clientOptions;

        public RateLimitController(
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
        {
            _ipOptions = ipOptions;
            _clientOptions = clientOptions;
        }

        /// <summary>
        /// Verifica o status do rate limit para o IP atual
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(typeof(RateLimitStatusDto), StatusCodes.Status200OK)]
        public IActionResult GetRateLimitStatus()
        {
            var clientId = Request.Headers["X-ClientId"].FirstOrDefault() ?? "anonymous";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Obter headers de rate limit da resposta
            var limit = Response.Headers["X-Rate-Limit-Limit"].FirstOrDefault() ?? "N/A";
            var remaining = Response.Headers["X-Rate-Limit-Remaining"].FirstOrDefault() ?? "N/A";
            var reset = Response.Headers["X-Rate-Limit-Reset"].FirstOrDefault() ?? "N/A";

            var status = new RateLimitStatusDto
            {
                ClientId = clientId,
                Endpoint = $"{Request.Method} {Request.Path}",
                Limit = int.TryParse(limit, out var l) ? l : 0,
                Remaining = int.TryParse(remaining, out var r) ? r : 0,
                ResetTime = DateTimeOffset.FromUnixTimeSeconds(long.TryParse(reset, out var rs) ? rs : 0).DateTime,
                Period = "1m"
            };

            // Adicionar headers customizados
            Response.Headers.Add("X-RateLimit-ClientId", clientId);
            Response.Headers.Add("X-RateLimit-IP", ip);

            return Ok(status);
        }

        /// <summary>
        /// Obtém as regras de rate limit configuradas
        /// </summary>
        [HttpGet("rules")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetRateLimitRules()
        {
            var rules = new
            {
                IpRateLimiting = new
                {
                    GeneralRules = _ipOptions.Value.GeneralRules,
                    EnableEndpointRateLimiting = _ipOptions.Value.EnableEndpointRateLimiting,
                    HttpStatusCode = _ipOptions.Value.HttpStatusCode,
                    RealIpHeader = _ipOptions.Value.RealIpHeader
                },
                ClientRateLimiting = new
                {
                    GeneralRules = _clientOptions.Value.GeneralRules,
                    EnableEndpointRateLimiting = _clientOptions.Value.EnableEndpointRateLimiting,
                    HttpStatusCode = _clientOptions.Value.HttpStatusCode,
                    ClientIdHeader = _clientOptions.Value.ClientIdHeader
                },
                Info = "As regras específicas estão configuradas no appsettings.json"
            };

            return Ok(rules);
        }
    }
}