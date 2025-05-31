using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherEmergencyAPI.DTOs.Weather;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Obtém informações completas do clima
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(WeatherResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCompleteWeather([FromBody] WeatherRequestDto request)
        {
            try
            {
                var weather = await _weatherService.GetCompleteWeatherInfoAsync(request.Latitude, request.Longitude);
                return Ok(weather);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { error = "Serviço de clima temporariamente indisponível" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Erro ao obter informações do clima" });
            }
        }

        /// <summary>
        /// Obtém o clima atual
        /// </summary>
        [HttpPost("current")]
        [ProducesResponseType(typeof(CurrentWeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrentWeather([FromBody] WeatherRequestDto request)
        {
            try
            {
                var weather = await _weatherService.GetCurrentWeatherAsync(request.Latitude, request.Longitude);
                return Ok(weather);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { error = "Serviço de clima temporariamente indisponível" });
            }
        }

        /// <summary>
        /// Obtém a previsão do tempo para os próximos 6 dias
        /// </summary>
        [HttpPost("forecast")]
        [ProducesResponseType(typeof(List<WeatherForecastDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeatherForecast([FromBody] WeatherRequestDto request)
        {
            try
            {
                var forecast = await _weatherService.GetWeatherForecastAsync(request.Latitude, request.Longitude);
                return Ok(forecast);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { error = "Serviço de clima temporariamente indisponível" });
            }
        }

        /// <summary>
        /// Obtém alertas meteorológicos
        /// </summary>
        [HttpPost("alerts")]
        [ProducesResponseType(typeof(List<WeatherAlertDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeatherAlerts([FromBody] WeatherRequestDto request)
        {
            try
            {
                var alerts = await _weatherService.GetWeatherAlertsAsync(request.Latitude, request.Longitude);
                return Ok(alerts);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Erro ao obter alertas meteorológicos" });
            }
        }

        /// <summary>
        /// Obtém o clima para uma localização salva
        /// </summary>
        [HttpGet("location/{locationId}")]
        [Authorize]
        [ProducesResponseType(typeof(CurrentWeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetWeatherByLocation(int locationId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var weather = await _weatherService.GetWeatherByLocationIdAsync(locationId, userId);
                return Ok(weather);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Erro ao obter informações do clima" });
            }
        }
    }
}