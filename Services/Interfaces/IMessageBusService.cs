namespace WeatherEmergencyAPI.Services.Interfaces
{
    public interface IMessageBusService
    {
        Task PublishUserRegistered(int userId, string email, string name);
        Task PublishWeatherAlert(int userId, string alertType, string severity, string description, double latitude, double longitude);
        Task PublishLogEvent(string level, string message, string source, Dictionary<string, object>? properties = null);
    }
}