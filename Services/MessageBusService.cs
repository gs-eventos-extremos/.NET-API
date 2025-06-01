using MassTransit;
using WeatherEmergencyAPI.Messages;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Services
{
    public class MessageBusService : IMessageBusService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MessageBusService> _logger;

        public MessageBusService(IPublishEndpoint publishEndpoint, ILogger<MessageBusService> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishUserRegistered(int userId, string email, string name)
        {
            try
            {
                var message = new UserRegisteredMessage
                {
                    UserId = userId,
                    Email = email,
                    Name = name,
                    RegisteredAt = DateTime.UtcNow
                };

                await _publishEndpoint.Publish(message);
                _logger.LogInformation($"Mensagem UserRegistered publicada para usuário {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem UserRegistered");
            }
        }

        public async Task PublishWeatherAlert(int userId, string alertType, string severity, string description, double latitude, double longitude)
        {
            try
            {
                var message = new WeatherAlertMessage
                {
                    UserId = userId,
                    AlertType = alertType,
                    Severity = severity,
                    Description = description,
                    Latitude = latitude,
                    Longitude = longitude,
                    AlertTime = DateTime.UtcNow
                };

                await _publishEndpoint.Publish(message);
                _logger.LogInformation($"Alerta meteorológico publicado: {alertType} para usuário {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar alerta meteorológico");
            }
        }

        public async Task PublishLogEvent(string level, string message, string source, Dictionary<string, object>? properties = null)
        {
            try
            {
                var logMessage = new LogEventMessage
                {
                    Level = level,
                    Message = message,
                    Source = source,
                    Timestamp = DateTime.UtcNow,
                    Properties = properties ?? new Dictionary<string, object>()
                };

                await _publishEndpoint.Publish(logMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar evento de log");
            }
        }
    }
}