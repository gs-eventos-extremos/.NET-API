using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherEmergencyAPI.DTOs.ML;
using WeatherEmergencyAPI.ML;
using WeatherEmergencyAPI.ML.Models;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MLPredictionController : ControllerBase
    {
        private readonly MLModelService _mlModelService;
        private readonly IMessageBusService _messageBusService;
        private readonly ILogger<MLPredictionController> _logger;

        public MLPredictionController(
            MLModelService mlModelService,
            IMessageBusService messageBusService,
            ILogger<MLPredictionController> logger)
        {
            _mlModelService = mlModelService;
            _messageBusService = messageBusService;
            _logger = logger;
        }

        /// <summary>
        /// Prevê o risco de desastres naturais com base em dados meteorológicos
        /// </summary>
        [HttpPost("predict-disaster")]
        [ProducesResponseType(typeof(DisasterPredictionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PredictDisasterRisk([FromBody] DisasterPredictionRequestDto request)
        {
            try
            {
                var prediction = _mlModelService.PredictDisasterRisk(
                    request.Temperature,
                    request.Humidity,
                    request.Pressure,
                    request.WindSpeed,
                    request.Precipitation
                );

                var response = new DisasterPredictionResponseDto
                {
                    HasHighRisk = prediction.HasDisasterPrediction,
                    RiskProbability = prediction.Probability,
                    RiskLevel = GetRiskLevel(prediction.Probability),
                    Recommendation = GetRecommendation(prediction.Probability, request),
                    PotentialHazards = GetPotentialHazards(request),
                    PredictionTime = DateTime.UtcNow
                };

                // HATEOAS links
                response.Links = new Dictionary<string, object>
                {
                    { "self", new { href = "/api/mlprediction/predict-disaster", method = "POST" } },
                    { "current_weather", new { href = "/api/weather/current", method = "POST" } },
                    { "alerts", new { href = "/api/weather/alerts", method = "POST" } }
                };

                // Se há alto risco e usuário autenticado, publicar alerta
                if (prediction.HasDisasterPrediction && User.Identity?.IsAuthenticated == true)
                {
                    var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                    await _messageBusService.PublishWeatherAlert(
                        userId,
                        "Risco de Desastre Natural",
                        response.RiskLevel,
                        response.Recommendation,
                        0, // Latitude seria obtida do contexto real
                        0  // Longitude seria obtida do contexto real
                    );
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao prever risco de desastre");
                return StatusCode(500, new { error = "Erro ao processar predição" });
            }
        }

        /// <summary>
        /// Detecta anomalias em séries temporais de dados meteorológicos
        /// </summary>
        [HttpPost("detect-anomaly")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult DetectAnomaly([FromBody] List<WeatherAnomalyData> historicalData, [FromQuery] float currentValue)
        {
            try
            {
                if (historicalData == null || historicalData.Count < 30)
                {
                    return BadRequest(new { error = "São necessários pelo menos 30 pontos de dados históricos" });
                }

                var anomaly = _mlModelService.DetectAnomaly(historicalData, currentValue);

                var response = new
                {
                    IsAnomaly = anomaly.IsAnomaly,
                    AnomalyScore = anomaly.RawScore,
                    Magnitude = anomaly.Magnitude,
                    CurrentValue = currentValue,
                    Timestamp = DateTime.UtcNow,
                    Recommendation = anomaly.IsAnomaly
                        ? "Valor anômalo detectado. Verifique as condições meteorológicas."
                        : "Valores dentro do padrão normal."
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao detectar anomalia");
                return StatusCode(500, new { error = "Erro ao processar detecção de anomalia" });
            }
        }

        /// <summary>
        /// Obtém informações sobre o modelo ML
        /// </summary>
        [HttpGet("model-info")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetModelInfo()
        {
            return Ok(new
            {
                ModelType = "Binary Classification - Logistic Regression",
                Features = new[] { "Temperature", "Humidity", "Pressure", "WindSpeed", "Precipitation" },
                LastTrainingDate = DateTime.UtcNow.AddDays(-1), // Simulado
                Version = "1.0.0",
                Description = "Modelo de predição de riscos de desastres naturais baseado em dados meteorológicos"
            });
        }

        private string GetRiskLevel(float probability)
        {
            return probability switch
            {
                >= 0.8f => "Crítico",
                >= 0.6f => "Alto",
                >= 0.4f => "Moderado",
                >= 0.2f => "Baixo",
                _ => "Mínimo"
            };
        }

        private string GetRecommendation(float probability, DisasterPredictionRequestDto data)
        {
            if (probability >= 0.8f)
            {
                return "ALERTA CRÍTICO: Procure abrigo imediatamente. Condições extremamente perigosas.";
            }
            else if (probability >= 0.6f)
            {
                if (data.WindSpeed > 80)
                    return "Ventos fortes detectados. Evite áreas abertas e proteja-se.";
                if (data.Precipitation > 100)
                    return "Chuva intensa prevista. Evite áreas de alagamento.";
                return "Condições adversas. Mantenha-se em local seguro.";
            }
            else if (probability >= 0.4f)
            {
                return "Monitore as condições meteorológicas. Possível deterioração do tempo.";
            }

            return "Condições estáveis. Continue monitorando.";
        }

        private List<string> GetPotentialHazards(DisasterPredictionRequestDto data)
        {
            var hazards = new List<string>();

            if (data.Temperature > 40)
                hazards.Add("Calor extremo");
            if (data.Temperature < 5)
                hazards.Add("Frio intenso");
            if (data.Humidity > 90)
                hazards.Add("Alta umidade");
            if (data.WindSpeed > 60)
                hazards.Add("Ventos fortes");
            if (data.Precipitation > 80)
                hazards.Add("Chuva intensa");
            if (data.Pressure < 1000)
                hazards.Add("Baixa pressão atmosférica");

            if (!hazards.Any())
                hazards.Add("Nenhum perigo identificado");

            return hazards;
        }
    }
}