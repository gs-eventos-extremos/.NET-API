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
        private readonly string _modelPath = "ML/TrainedModels/disaster_prediction_model.zip";

        public MLModelService(ILogger<MLModelService> logger)
        {
            _mlContext = new MLContext(seed: 0);
            _logger = logger;

            // Carregar modelo se existir, senão treinar um novo
            LoadOrTrainModel();
        }

        private void LoadOrTrainModel()
        {
            try
            {
                if (File.Exists(_modelPath))
                {
                    _logger.LogInformation("Carregando modelo ML existente...");
                    _disasterPredictionModel = _mlContext.Model.Load(_modelPath, out _);
                    _predictionEngine = _mlContext.Model.CreatePredictionEngine<WeatherData, WeatherPrediction>(_disasterPredictionModel);
                }
                else
                {
                    _logger.LogInformation("Treinando novo modelo ML...");
                    TrainDisasterPredictionModel();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar/treinar modelo ML");
                // Treinar modelo básico em caso de erro
                TrainBasicModel();
            }
        }

        private void TrainDisasterPredictionModel()
        {
            // Gerar dados sintéticos para treinamento
            var trainingData = GenerateTrainingData();
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // Dividir dados em treino e teste
            var splitData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            // Pipeline de transformação e treinamento
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(WeatherData.Temperature),
                    nameof(WeatherData.Humidity),
                    nameof(WeatherData.Pressure),
                    nameof(WeatherData.WindSpeed),
                    nameof(WeatherData.Precipitation))
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(WeatherData.HasDisaster),
                    featureColumnName: "Features"));

            // Treinar o modelo
            _disasterPredictionModel = pipeline.Fit(splitData.TrainSet);

            // Avaliar o modelo
            var predictions = _disasterPredictionModel.Transform(splitData.TestSet);
            var metrics = _mlContext.BinaryClassification.Evaluate(predictions,
                labelColumnName: nameof(WeatherData.HasDisaster));

            _logger.LogInformation($"Modelo treinado - Acurácia: {metrics.Accuracy:P2}");
            _logger.LogInformation($"AUC: {metrics.AreaUnderRocCurve:P2}");
            _logger.LogInformation($"F1 Score: {metrics.F1Score:P2}");

            // Salvar o modelo
            SaveModel();

            // Criar engine de predição
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<WeatherData, WeatherPrediction>(_disasterPredictionModel);
        }

        private void TrainBasicModel()
        {
            // Modelo básico com dados mínimos
            var basicData = new List<WeatherData>
            {
                new WeatherData { Temperature = 35, Humidity = 90, Pressure = 990, WindSpeed = 80, Precipitation = 100, HasDisaster = true },
                new WeatherData { Temperature = 25, Humidity = 60, Pressure = 1013, WindSpeed = 20, Precipitation = 10, HasDisaster = false },
                new WeatherData { Temperature = 40, Humidity = 95, Pressure = 980, WindSpeed = 100, Precipitation = 150, HasDisaster = true },
                new WeatherData { Temperature = 20, Humidity = 50, Pressure = 1020, WindSpeed = 15, Precipitation = 5, HasDisaster = false }
            };

            var dataView = _mlContext.Data.LoadFromEnumerable(basicData);

            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(WeatherData.Temperature),
                    nameof(WeatherData.Humidity),
                    nameof(WeatherData.Pressure),
                    nameof(WeatherData.WindSpeed),
                    nameof(WeatherData.Precipitation))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(WeatherData.HasDisaster),
                    featureColumnName: "Features"));

            _disasterPredictionModel = pipeline.Fit(dataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<WeatherData, WeatherPrediction>(_disasterPredictionModel);
        }

        private List<WeatherData> GenerateTrainingData()
        {
            var random = new Random(42);
            var trainingData = new List<WeatherData>();

            // Gerar 1000 exemplos de treinamento
            for (int i = 0; i < 1000; i++)
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
                if (humidity > 90 && temperature > 35) hasDisaster = true; // Condições de temporal

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
            if (_predictionEngine == null)
            {
                throw new InvalidOperationException("Modelo de predição não está carregado");
            }

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

        public WeatherAnomalyPrediction DetectAnomaly(List<WeatherAnomalyData> historicalData, float currentValue)
        {
            // Converter para formato IDataView
            var dataView = _mlContext.Data.LoadFromEnumerable(historicalData);

            // Pipeline de detecção de anomalias
            const int pvalueHistoryLength = 30;
            const int trainingWindowSize = 90;
            const int seasonalityWindowSize = 30;

            var pipeline = _mlContext.Transforms.DetectIidSpike(
                outputColumnName: nameof(WeatherAnomalyPrediction.Prediction),
                inputColumnName: nameof(WeatherAnomalyData.Value),
                pvalueHistoryLength: pvalueHistoryLength,
                confidence: 95);

            var trainedModel = pipeline.Fit(dataView);
            var transformedData = trainedModel.Transform(dataView);

            // Fazer predição para o valor atual
            var newData = new List<WeatherAnomalyData> { new WeatherAnomalyData { Timestamp = DateTime.UtcNow, Value = currentValue } };
            var newDataView = _mlContext.Data.LoadFromEnumerable(newData);
            var predictions = trainedModel.Transform(newDataView);

            var predictionResults = _mlContext.Data.CreateEnumerable<WeatherAnomalyPrediction>(predictions, reuseRowObject: false).ToList();

            return predictionResults.FirstOrDefault() ?? new WeatherAnomalyPrediction();
        }

        private void SaveModel()
        {
            if (_disasterPredictionModel == null) return;

            try
            {
                var directory = Path.GetDirectoryName(_modelPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                _mlContext.Model.Save(_disasterPredictionModel, null, _modelPath);
                _logger.LogInformation($"Modelo salvo em: {_modelPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar modelo ML");
            }
        }

        public void RetrainModel(List<WeatherData> newData)
        {
            if (newData == null || !newData.Any()) return;

            _logger.LogInformation($"Re-treinando modelo com {newData.Count} novos exemplos");

            var allData = GenerateTrainingData();
            allData.AddRange(newData);

            var dataView = _mlContext.Data.LoadFromEnumerable(allData);
            var splitData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(WeatherData.Temperature),
                    nameof(WeatherData.Humidity),
                    nameof(WeatherData.Pressure),
                    nameof(WeatherData.WindSpeed),
                    nameof(WeatherData.Precipitation))
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(WeatherData.HasDisaster),
                    featureColumnName: "Features"));

            _disasterPredictionModel = pipeline.Fit(splitData.TrainSet);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<WeatherData, WeatherPrediction>(_disasterPredictionModel);

            SaveModel();
        }
    }
}