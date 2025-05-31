using WeatherEmergencyAPI.DTOs.Weather;

namespace WeatherEmergencyAPI.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<CurrentWeatherDto> GetCurrentWeatherAsync(double latitude, double longitude);
        Task<List<WeatherForecastDto>> GetWeatherForecastAsync(double latitude, double longitude);
        Task<List<WeatherAlertDto>> GetWeatherAlertsAsync(double latitude, double longitude);
        Task<WeatherResponseDto> GetCompleteWeatherInfoAsync(double latitude, double longitude);
        Task<CurrentWeatherDto> GetWeatherByLocationIdAsync(int locationId, int userId);
    }
}