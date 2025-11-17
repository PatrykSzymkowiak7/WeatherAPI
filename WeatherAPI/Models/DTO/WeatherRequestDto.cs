using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.DTO
{
    public class WeatherRequestDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string City { get; set; } = "";
    }
}
