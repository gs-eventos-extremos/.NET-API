using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Globalization;
using WeatherEmergencyAPI.DTOs.Weather;
using WeatherEmergencyAPI.Repositories.Interfaces;
using WeatherEmergencyAPI.Services.Interfaces;

namespace WeatherEmergencyAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILocationRepository _locationRepository;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public WeatherService(
            HttpClient httpClient,
            IConfiguration configuration,
            IMemoryCache cache,
            ILocationRepository locationRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;
            _locationRepository = locationRepository;
            _apiKey = _configuration["WeatherApi:OpenWeatherMapKey"] ?? throw new ArgumentNullException("OpenWeatherMap API Key não configurada");
            _baseUrl = _configuration["WeatherApi:OpenWeatherMapUrl"] ?? "https://api.openweathermap.org/data/2.5";
        }

        public async Task<CurrentWeatherDto> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            var cacheKey = $"weather_current_{latitude}_{longitude}";

            if (_cache.TryGetValue(cacheKey, out CurrentWeatherDto? cachedWeather))
            {
                return cachedWeather!;
            }

            var url = $"{_baseUrl}/weather?lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}&appid={_apiKey}&units=metric&lang=pt_br";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json)!;

            var weather = new CurrentWeatherDto
            {
                City = data.name,
                State = GetStateFromCoordinates(latitude, longitude),
                Country = data.sys.country,
                Temperature = data.main.temp,
                FeelsLike = data.main.feels_like,
                Humidity = data.main.humidity,
                Description = data.weather[0].description,
                Icon = $"https://openweathermap.org/img/w/{data.weather[0].icon}.png",
                UpdatedAt = DateTime.UtcNow
            };

            // Cache por 10 minutos
            _cache.Set(cacheKey, weather, TimeSpan.FromMinutes(10));

            return weather;
        }

        public async Task<List<WeatherForecastDto>> GetWeatherForecastAsync(double latitude, double longitude)
        {
            var cacheKey = $"weather_forecast_{latitude}_{longitude}";

            if (_cache.TryGetValue(cacheKey, out List<WeatherForecastDto>? cachedForecast))
            {
                return cachedForecast!;
            }

            var url = $"{_baseUrl}/forecast?lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}&appid={_apiKey}&units=metric&lang=pt_br&cnt=48";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json)!;

            var forecasts = new List<WeatherForecastDto>();
            var dailyData = new Dictionary<string, DailyWeatherData>();

            // Agrupar por dia
            foreach (var item in data.list)
            {
                DateTime dt = DateTimeOffset.FromUnixTimeSeconds((long)item.dt).DateTime;
                string dateKey = dt.Date.ToString("yyyy-MM-dd");

                if (!dailyData.ContainsKey(dateKey))
                {
                    dailyData[dateKey] = new DailyWeatherData
                    {
                        Date = dt.Date,
                        Temps = new List<double>(),
                        Descriptions = new List<string>(),
                        Icons = new List<string>(),
                        Rain = new List<double>()
                    };
                }

                dailyData[dateKey].Temps.Add((double)item.main.temp);
                dailyData[dateKey].Descriptions.Add((string)item.weather[0].description);
                dailyData[dateKey].Icons.Add((string)item.weather[0].icon);

                if (item.rain != null && item.rain["3h"] != null)
                {
                    dailyData[dateKey].Rain.Add((double)item.rain["3h"]);
                }
            }

            // Criar previsões diárias (próximos 6 dias)
            foreach (var day in dailyData.Values.Skip(1).Take(6))
            {
                var forecast = new WeatherForecastDto
                {
                    Date = day.Date,
                    TemperatureMin = day.Temps.Min(),
                    TemperatureMax = day.Temps.Max(),
                    Description = day.Descriptions
                        .GroupBy(x => x)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key,
                    Icon = $"https://openweathermap.org/img/w/{day.Icons.GroupBy(x => x).OrderByDescending(g => g.Count()).First().Key}.png",
                    ChanceOfRain = day.Rain.Count > 0 ? (day.Rain.Count / (double)day.Temps.Count) * 100 : 0
                };

                forecasts.Add(forecast);
            }

            // Cache por 1 hora
            _cache.Set(cacheKey, forecasts, TimeSpan.FromHours(1));

            return forecasts;
        }

        public async Task<List<WeatherAlertDto>> GetWeatherAlertsAsync(double latitude, double longitude)
        {
            // Simulação de alertas - em produção, integrar com API real de alertas
            var alerts = new List<WeatherAlertDto>();

            // Simular alerta se umidade > 80%
            var currentWeather = await GetCurrentWeatherAsync(latitude, longitude);

            if (currentWeather.Humidity > 80)
            {
                alerts.Add(new WeatherAlertDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Chuva Forte",
                    Severity = "Moderada",
                    Title = "Possibilidade de Chuva Forte",
                    Description = "Alta umidade pode resultar em chuvas fortes nas próximas horas",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(6),
                    AffectedAreas = new List<string> { currentWeather.City }
                });
            }

            // Simular alerta de temperatura
            if (currentWeather.Temperature > 35)
            {
                alerts.Add(new WeatherAlertDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Calor Extremo",
                    Severity = "Alta",
                    Title = "Alerta de Temperatura Elevada",
                    Description = "Temperaturas acima de 35°C. Mantenha-se hidratado e evite exposição ao sol",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(8),
                    AffectedAreas = new List<string> { currentWeather.City }
                });
            }

            if (alerts.Count == 0)
            {
                alerts.Add(new WeatherAlertDto
                {
                    Id = "no-alert",
                    Type = "Informativo",
                    Severity = "Baixa",
                    Title = "Sem alertas no momento",
                    Description = "Não há alertas meteorológicos para sua região",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow,
                    AffectedAreas = new List<string> { currentWeather.City }
                });
            }

            return alerts;
        }

        public async Task<WeatherResponseDto> GetCompleteWeatherInfoAsync(double latitude, double longitude)
        {
            var currentTask = GetCurrentWeatherAsync(latitude, longitude);
            var forecastTask = GetWeatherForecastAsync(latitude, longitude);
            var alertsTask = GetWeatherAlertsAsync(latitude, longitude);

            await Task.WhenAll(currentTask, forecastTask, alertsTask);

            var response = new WeatherResponseDto
            {
                Current = await currentTask,
                Forecast = await forecastTask,
                Alerts = await alertsTask
            };

            // Adicionar HATEOAS links
            response.Links = new Dictionary<string, object>
            {
                { "self", new { href = $"/api/weather?latitude={latitude}&longitude={longitude}", method = "GET" } },
                { "current", new { href = $"/api/weather/current?latitude={latitude}&longitude={longitude}", method = "GET" } },
                { "forecast", new { href = $"/api/weather/forecast?latitude={latitude}&longitude={longitude}", method = "GET" } },
                { "alerts", new { href = $"/api/weather/alerts?latitude={latitude}&longitude={longitude}", method = "GET" } }
            };

            response.Current.Links = new Dictionary<string, object>
            {
                { "forecast", new { href = $"/api/weather/forecast?latitude={latitude}&longitude={longitude}", method = "GET" } },
                { "alerts", new { href = $"/api/weather/alerts?latitude={latitude}&longitude={longitude}", method = "GET" } }
            };

            return response;
        }

        public async Task<CurrentWeatherDto> GetWeatherByLocationIdAsync(int locationId, int userId)
        {
            var location = await _locationRepository.GetUserLocationByIdAsync(userId, locationId);

            if (location == null)
            {
                throw new KeyNotFoundException("Localização não encontrada");
            }

            return await GetCurrentWeatherAsync(location.Latitude, location.Longitude);
        }

        public async Task<(double latitude, double longitude, string formattedAddress)> GetCoordinatesFromAddressAsync(string city, string state, string country)
        {
            var cacheKey = $"geocoding_{city}_{state}_{country}".ToLower().Replace(" ", "_");

            if (_cache.TryGetValue(cacheKey, out (double lat, double lon, string addr) cached))
            {
                return cached;
            }

            // Formatar query para a API
            var query = Uri.EscapeDataString($"{city},{state},{country}");
            var url = $"http://api.openweathermap.org/geo/1.0/direct?q={query}&limit=1&appid={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var locations = JsonConvert.DeserializeObject<List<dynamic>>(json);

            if (locations == null || locations.Count == 0)
            {
                throw new KeyNotFoundException($"Localização não encontrada para: {city}, {state}, {country}");
            }

            var location = locations[0];
            double latitude = (double)location.lat;
            double longitude = (double)location.lon;

            // Construir endereço formatado
            string locationName = (string)location.name;
            string locationState = location.state != null ? (string)location.state : state;
            string locationCountry = (string)location.country;
            string formattedAddress = $"{locationName}, {locationState}, {locationCountry}";

            var result = (latitude, longitude, formattedAddress);

            // Cache por 24 horas
            _cache.Set(cacheKey, result, TimeSpan.FromHours(24));

            return result;
        }

        private string GetStateFromCoordinates(double latitude, double longitude)
        {
            // Implementação simplificada - em produção, usar API de geocoding reverso
            // Para São Paulo
            if (latitude >= -24.0 && latitude <= -22.0 && longitude >= -47.0 && longitude <= -45.0)
            {
                return "SP";
            }
            // Para Rio de Janeiro
            else if (latitude >= -23.0 && latitude <= -22.0 && longitude >= -44.0 && longitude <= -42.0)
            {
                return "RJ";
            }
            // Para Minas Gerais
            else if (latitude >= -22.0 && latitude <= -18.0 && longitude >= -47.0 && longitude <= -43.0)
            {
                return "MG";
            }
            // Para Paraná
            else if (latitude >= -26.0 && latitude <= -23.0 && longitude >= -54.0 && longitude <= -48.0)
            {
                return "PR";
            }

            return "BR";
        }

        // Classe auxiliar para evitar problemas com dynamic
        private class DailyWeatherData
        {
            public DateTime Date { get; set; }
            public List<double> Temps { get; set; } = new();
            public List<string> Descriptions { get; set; } = new();
            public List<string> Icons { get; set; } = new();
            public List<double> Rain { get; set; } = new();
        }
    }
}