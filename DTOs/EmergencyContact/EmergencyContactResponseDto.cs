namespace WeatherEmergencyAPI.DTOs.EmergencyContact
{
    public class EmergencyContactResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string? Relationship { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }

        // HATEOAS Links
        public Dictionary<string, object> Links { get; set; } = new();
    }
}