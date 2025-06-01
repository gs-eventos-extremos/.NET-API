using MassTransit;
using WeatherEmergencyAPI.Messages;
using WeatherEmergencyAPI.Repositories.Interfaces;

namespace WeatherEmergencyAPI.Consumers
{
    public class WeatherAlertConsumer : IConsumer<WeatherAlertMessage>
    {
        private readonly ILogger<WeatherAlertConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public WeatherAlertConsumer(
            ILogger<WeatherAlertConsumer> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task Consume(ConsumeContext<WeatherAlertMessage> context)
        {
            var message = context.Message;

            _logger.LogWarning($"Alerta meteorológico recebido: {message.AlertType} - Severidade: {message.Severity}");

            using (var scope = _scopeFactory.CreateScope())
            {
                var emergencyContactRepository = scope.ServiceProvider.GetRequiredService<IEmergencyContactRepository>();

                // Buscar contatos de emergência do usuário
                var contacts = await emergencyContactRepository.GetUserContactsAsync(message.UserId);

                foreach (var contact in contacts.Where(c => c.IsPrimary))
                {
                    // Simular envio de SMS/WhatsApp
                    _logger.LogInformation($"[SIMULADO] SMS enviado para {contact.PhoneNumber}: Alerta {message.AlertType} - {message.Description}");
                }
            }

            // Registrar o alerta processado
            _logger.LogInformation($"Alerta processado: {message.AlertType} para usuário {message.UserId}");
        }
    }
}