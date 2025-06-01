namespace WeatherEmergencyAPI.Messages
{
    public class WeatherAlertMessage
    {
        public int UserId { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime AlertTime { get; set; }
    }
}