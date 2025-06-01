using MassTransit;
using WeatherEmergencyAPI.Messages;

namespace WeatherEmergencyAPI.Consumers
{
    public class UserRegisteredConsumer : IConsumer<UserRegisteredMessage>
    {
        private readonly ILogger<UserRegisteredConsumer> _logger;
        private readonly IConfiguration _configuration;

        public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<UserRegisteredMessage> context)
        {
            var message = context.Message;

            _logger.LogInformation($"Processando registro de usuário: {message.Email}");

            // Simular envio de email de boas-vindas
            if (_configuration.GetValue<bool>("EmailSettings:EnableEmailService"))
            {
                // Aqui você integraria com um serviço real de email
                _logger.LogInformation($"Email de boas-vindas enviado para: {message.Email}");
            }
            else
            {
                _logger.LogInformation($"[SIMULADO] Email de boas-vindas seria enviado para: {message.Email}");
            }

            // Simular outras ações assíncronas
            await Task.Delay(1000); // Simula processamento

            _logger.LogInformation($"Processamento concluído para usuário: {message.UserId}");
        }
    }
}