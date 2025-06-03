using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TimeSeries;
using WeatherEmergencyAPI.ML.Models;

namespace WeatherEmergencyAPI.ML
{
    public class MLModelService
    {
        private readonly MLContext _mlContext;
        private ITransformer? _disasterPredictionModel;
        private PredictionEngine<WeatherData, WeatherPrediction>? _predictionEngine;
        private readonly ILogger<MLModelService> _logger;
        private readonly string _modelPath;
        private bool _isModelReady = false;

        public MLModelService(ILogger<MLModelService> logger, IWebHostEnvironment environment)
        {
            _mlContext = new MLContext(seed: 0);
            _logger = logger;

            // Usar o caminho correto baseado no ambiente
            var modelsFolder = Path.Combine(environment.ContentRootPath, "ML", "TrainedModels");

            // Criar pasta se não existir
            if (!Directory.Exists(modelsFolder))
            {
                try
                {
                    Directory.CreateDirectory(modelsFolder);
                    _logger.LogInformation($"Pasta de modelos criada: {modelsFolder}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar pasta de modelos");
                }
            }

            _modelPath = Path.Combine(modelsFolder, "disaster_prediction_model.zip");

            // Inicializar modelo
            InitializeModel();
        }

        private void InitializeModel()
        {
            try
            {
                _logger.LogInformation("Inicializando modelo ML...");

                // Sempre treinar um novo modelo para evitar problemas
                TrainBasicModel();

                _isModelReady = true;
                _logger.LogInformation("Modelo ML inicializado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inicializar modelo ML");
                _isModelReady = false;
            }
        }

        private void TrainBasicModel()
        {
            try
            {
                // Gerar dados de treinamento
                var trainingData = GenerateTrainingData();
                var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

                // Pipeline simplificado
                var pipeline = _mlContext.Transforms.Concatenate("Features",
                        nameof(WeatherData.Temperature),
                        nameof(WeatherData.Humidity),
                        nameof(WeatherData.Pressure),
                        nameof(WeatherData.WindSpeed),
                        nameof(WeatherData.Precipitation))
                    .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                        labelColumnName: nameof(WeatherData.HasDisaster),
                        featureColumnName: "Features"));

                // Treinar o modelo
                _disasterPredictionModel = pipeline.Fit(dataView);

                // Criar engine de predição
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<WeatherData, WeatherPrediction>(_disasterPredictionModel);

                _logger.LogInformation("Modelo treinado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao treinar modelo");
                throw;
            }
        }

        private List<WeatherData> GenerateTrainingData()
        {
            var random = new Random(42);
            var trainingData = new List<WeatherData>();

            // Gerar apenas 200 exemplos para ser mais rápido
            for (int i = 0; i < 200; i++)
            {
                var temperature = (float)(random.NextDouble() * 45 + 5); // 5-50°C
                var humidity = (float)(random.NextDouble() * 100); // 0-100%
                var pressure = (float)(random.NextDouble() * 40 + 990); // 990-1030 hPa
                var windSpeed = (float)(random.NextDouble() * 120); // 0-120 km/h
                var precipitation = (float)(random.NextDouble() * 200); // 0-200mm

                // Lógica para determinar se há desastre
                bool hasDisaster = false;

                // Condições extremas que podem causar desastres
                if (temperature > 40 && humidity > 80) hasDisaster = true; // Calor extremo
                if (temperature < 10 && precipitation > 100) hasDisaster = true; // Tempestade fria
                if (windSpeed > 80) hasDisaster = true; // Ventos fortes
                if (pressure < 995 && precipitation > 50) hasDisaster = true; // Tempestade
                if (precipitation > 150) hasDisaster = true; // Chuva extrema

                trainingData.Add(new WeatherData
                {
                    Temperature = temperature,
                    Humidity = humidity,
                    Pressure = pressure,
                    WindSpeed = windSpeed,
                    Precipitation = precipitation,
                    HasDisaster = hasDisaster
                });
            }

            return trainingData;
        }

        public WeatherPrediction PredictDisasterRisk(float temperature, float humidity, float pressure, float windSpeed, float precipitation)
        {
            if (!_isModelReady || _predictionEngine == null)
            {
                _logger.LogWarning("Modelo não está pronto, usando predição baseada em regras");

                // Fallback para regras simples se o modelo não estiver pronto
                bool hasDisaster = (temperature > 40 && humidity > 80) ||
                                 (windSpeed > 80) ||
                                 (precipitation > 150) ||
                                 (pressure < 995 && precipitation > 50);

                float probability = 0f;
                if (hasDisaster)
                {
                    probability = 0.85f;
                }
                else if (temperature > 35 || windSpeed > 60 || precipitation > 100)
                {
                    probability = 0.45f;
                }
                else
                {
                    probability = 0.15f;
                }

                return new WeatherPrediction
                {
                    HasDisasterPrediction = hasDisaster,
                    Probability = probability,
                    Score = new[] { probability, 1 - probability }
                };
            }

            try
            {
                var input = new WeatherData
                {
                    Temperature = temperature,
                    Humidity = humidity,
                    Pressure = pressure,
                    WindSpeed = windSpeed,
                    Precipitation = precipitation
                };

                return _predictionEngine.Predict(input);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer predição");

                // Retornar predição padrão em caso de erro
                return new WeatherPrediction
                {
                    HasDisasterPrediction = false,
                    Probability = 0.5f,
                    Score = new[] { 0.5f, 0.5f }
                };
            }
        }

        public WeatherAnomalyPrediction DetectAnomaly(List<WeatherAnomalyData> historicalData, float currentValue)
        {
            try
            {
                if (historicalData == null || historicalData.Count < 10)
                {
                    // Retornar resultado padrão se não houver dados suficientes
                    return new WeatherAnomalyPrediction
                    {
                        Prediction = new[] { 0.0, 0.0, 0.0 }
                    };
                }

                // Detecção simples de anomalia baseada em desvio padrão
                var values = historicalData.Select(d => d.Value).ToList();
                var mean = values.Average();
                var stdDev = Math.Sqrt(values.Select(v => Math.Pow(v - mean, 2)).Average());

                var zScore = Math.Abs((currentValue - mean) / stdDev);
                bool isAnomaly = zScore > 2.5; // Valor está a mais de 2.5 desvios padrão

                return new WeatherAnomalyPrediction
                {
                    Prediction = new[] { isAnomaly ? 1.0 : 0.0, zScore, zScore * 10 }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao detectar anomalia");
                return new WeatherAnomalyPrediction
                {
                    Prediction = new[] { 0.0, 0.0, 0.0 }
                };
            }
        }

        public void RetrainModel(List<WeatherData> newData)
        {
            _logger.LogInformation($"Re-treinamento não implementado nesta versão");
        }
    }
}