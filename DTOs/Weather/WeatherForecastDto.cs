namespace WeatherEmergencyAPI.DTOs.Weather
{
    public class WeatherForecastDto
    {
        public DateTime Date { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public double ChanceOfRain { get; set; }
    }
}