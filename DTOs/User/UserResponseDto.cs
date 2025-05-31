namespace WeatherEmergencyAPI.DTOs.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // HATEOAS Links
        public Dictionary<string, object> Links { get; set; } = new();
    }
}