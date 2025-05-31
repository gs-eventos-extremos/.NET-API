namespace WeatherEmergencyAPI.DTOs.Location
{
    public class LocationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime CreatedAt { get; set; }

        // HATEOAS Links
        public Dictionary<string, object> Links { get; set; } = new();
    }
}