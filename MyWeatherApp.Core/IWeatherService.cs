using MyWeatherApp.Core.Models;

namespace MyWeatherApp.Core.Services
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherAsync(string? timezone = "Europe/Vilnius");
    }
}