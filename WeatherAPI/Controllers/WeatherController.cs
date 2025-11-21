using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Models;
using WeatherAPI.Models.DTO;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] WeatherRequestDto request)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _weatherService.GetWeatherAsync(request.City);

                if (result == null)
                    return NotFound(new
                    {
                        error = "City not found",
                        request.City
                    });

                var dto = new WeatherResponseDto
                {
                    City = result.City,
                    Temperature = result.Temperature,
                    Conditions = result.Conditions,
                    Humidity = result.Humidity
                };

                return Ok(dto);
            }
            catch(Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: 500);
            }
        }
    }
}
