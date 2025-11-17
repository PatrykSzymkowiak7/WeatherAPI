namespace WeatherAPI.Models.DTO
{
    public class WeatherResponseDto
    {
        public string City { get; set; }
        public double Temperature { get; set; }
        public string Conditions { get; set; }
        public double Humidity { get; set; }
    }
}
