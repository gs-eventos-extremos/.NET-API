namespace WeatherEmergencyAPI.DTOs.Weather
{
    public class WeatherAlertDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> AffectedAreas { get; set; } = new();
    }
}