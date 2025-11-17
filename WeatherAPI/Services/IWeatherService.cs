using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public interface IWeatherService
    {
        Task<WeatherInfo?> GetWeatherAsync(string city);
    }
}
