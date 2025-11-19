// The project consists of more than one module (assembly)
using MyWeatherApp.Core.Models; // Model dependency

namespace MyWeatherApp.Core.Services // Project service namespace
{
    // applied interface
    public interface IWeatherService // Service contract
    {
        // Async method definition
        // Default and named arguments are used
        // Operators ?, ?[], ??, or ??= are used
        Task<WeatherData> GetWeatherAsync(string? timezone = "Europe/Vilnius");
    }
}