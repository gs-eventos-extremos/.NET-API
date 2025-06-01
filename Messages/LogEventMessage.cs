namespace WeatherEmergencyAPI.Messages
{
    public class LogEventMessage
    {
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}