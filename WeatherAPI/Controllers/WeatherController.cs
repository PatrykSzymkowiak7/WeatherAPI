using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public WeatherController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            var apiKey = _config["WEATHER_API_KEY"];
            if (string.IsNullOrEmpty(apiKey))
                return BadRequest("API Key is not configured.");

            var client = _httpClientFactory.CreateClient();
            var url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city}?unitGroup=metric&key={apiKey}&contentType=json";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return BadRequest("Could not fetch weather data");

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
    }
}
