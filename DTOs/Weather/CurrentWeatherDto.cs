namespace WeatherEmergencyAPI.DTOs.Weather
{
    public class CurrentWeatherDto
    {
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double Humidity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }

        // HATEOAS
        public Dictionary<string, object> Links { get; set; } = new();
    }
}