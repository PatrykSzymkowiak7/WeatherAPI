using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherAPI.Controllers;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<WeatherInfo?> GetWeatherAsync(string city)
        {
            _logger.LogInformation("Received request for weather in city: {City}", city);

            try
            {
                var apiKey = _config["WEATHER_API_KEY"];
                if (string.IsNullOrEmpty(apiKey))
                    throw new Exception("Missing WEATHER_API_KEY");

                var client = _httpClientFactory.CreateClient();
                var url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city}?unitGroup=metric&key={apiKey}&contentType=json";
                
                _logger.LogInformation("Calling external API for city: {City}", city);

                var response = await client.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("External API returned an error: {StatusCode} for city: {City}", response.StatusCode, city);
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("External API returned an error: {StatusCode} for city: {City}", response.StatusCode, city);
                    throw new Exception($"External API error: {response.StatusCode}");
                }


                var json = await response.Content.ReadAsStringAsync();
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;
                var cityName = root.GetProperty("address").GetString();
                var currentConditions = root.GetProperty("currentConditions");

                var result = new WeatherInfo
                {
                    City = cityName,
                    Temperature = currentConditions.GetProperty("temp").GetDouble(),
                    Humidity = currentConditions.GetProperty("humidity").GetDouble(),
                    Conditions = currentConditions.GetProperty("conditions").GetString()
                };

                _logger.LogInformation("Weather data fetched successfully for city: {City}", city);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Exception occurred when requesting weather for city: {City}", city);
                throw;
            }
        }
    }
}
