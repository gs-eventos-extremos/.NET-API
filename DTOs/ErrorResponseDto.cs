namespace WeatherEmergencyAPI.DTOs
{
    public class ErrorResponseDto
    {
        public string Error { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Path { get; set; }
    }
}