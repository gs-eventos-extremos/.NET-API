namespace WeatherEmergencyAPI.DTOs
{
    public class RateLimitStatusDto
    {
        public string ClientId { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public int Limit { get; set; }
        public int Remaining { get; set; }
        public DateTime ResetTime { get; set; }
        public string Period { get; set; } = string.Empty;
    }
}