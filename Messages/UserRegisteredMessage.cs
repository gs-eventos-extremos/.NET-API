namespace WeatherEmergencyAPI.Messages
{
    public class UserRegisteredMessage
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }
}