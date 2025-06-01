using MassTransit;
using WeatherEmergencyAPI.Messages;

namespace WeatherEmergencyAPI.Consumers
{
    public class LogEventConsumer : IConsumer<LogEventMessage>
    {
        private readonly ILogger<LogEventConsumer> _logger;

        public LogEventConsumer(ILogger<LogEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<LogEventMessage> context)
        {
            var message = context.Message;

            // Aqui você poderia enviar logs para ElasticSearch, MongoDB, etc.
            var logMessage = $"[{message.Level}] {message.Source}: {message.Message}";

            // Simular processamento de log
            await Task.Delay(100);

            // Por enquanto, apenas loga no console
            switch (message.Level.ToLower())
            {
                case "error":
                    _logger.LogError(logMessage);
                    break;
                case "warning":
                    _logger.LogWarning(logMessage);
                    break;
                default:
                    _logger.LogInformation(logMessage);
                    break;
            }
        }
    }
}