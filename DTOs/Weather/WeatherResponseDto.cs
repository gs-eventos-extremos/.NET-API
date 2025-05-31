namespace WeatherEmergencyAPI.DTOs.Weather
{
    public class WeatherResponseDto
    {
        public CurrentWeatherDto Current { get; set; } = null!;
        public List<WeatherForecastDto> Forecast { get; set; } = new();
        public List<WeatherAlertDto> Alerts { get; set; } = new();
        public Dictionary<string, object> Links { get; set; } = new();
    }
}