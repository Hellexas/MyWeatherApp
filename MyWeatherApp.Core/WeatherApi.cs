// The project consists of more than one module (assembly)
using MyWeatherApp.Core.Models; // Model dependency
using System.Text.Json; // JSON dependency
using System.Text.Json.Serialization; // JSON property name dependency

namespace MyWeatherApp.Core.Services // Project service namespace
{
    // Sealed class
    // Created and applied interface
    public sealed class WeatherService : IWeatherService // Service implementation
    {
        // Static fields
        private static readonly HttpClient _client;
        // Data structures from System.Collections or System.Collections.Generic are used
        private static readonly Dictionary<string, (double Lat, double Lon)> _cityCoordinates;

        // A static constructor is used
        static WeatherService() // Static ctor for client/data init
        {
            _client = new HttpClient();
            _cityCoordinates = new Dictionary<string, (double Lat, double Lon)>
            {
                { "Europe/Vilnius", (54.72, 25.24) } // Add coordinates
            };
        }

        // Builds the API URL
        // Operators ?, ?[], ??, or ??= are used
        private string GetApiUrl(string? timezone)
        {
            // Operators ?, ?[], ??, or ??= are used
            string tzKey = timezone ?? "Europe/Vilnius"; // Default value

            // Initialization using out arguments
            if (!_cityCoordinates.TryGetValue(tzKey, out var coords)) // Get coords
            {
                coords = _cityCoordinates["Europe/Vilnius"]; // Fallback
            }

            return $"https://api.open-meteo.com/v1/forecast?latitude={coords.Lat}&longitude={coords.Lon}&timezone={tzKey}&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,weather_code,wind_speed_10m&hourly=temperature_2m,cloud_cover,precipitation_probability,weather_code,is_day&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max";
        }

        // Gets weather data from API
        // Default and named arguments are used
        // Operators ?, ?[], ??, or ??= are used
        public async Task<WeatherData> GetWeatherAsync(string? timezone = "Europe/Vilnius")
        {
            string apiUrl = GetApiUrl(timezone); // Build URL

            try
            {
                HttpResponseMessage response = await _client.GetAsync(apiUrl); // Send request
                response.EnsureSuccessStatusCode(); // Check status
                string jsonResponse = await response.Content.ReadAsStringAsync(); // Read response

                // Operators ?, ?[], ??, or ??= are used
                WeatherData? weatherData = JsonSerializer.Deserialize<WeatherData>(jsonResponse); // Deserialize
                // Operators ?, ?[], ??, or ??= are used
                return weatherData ?? throw new JsonException("Failed to deserialize weather data."); // Return or throw
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nAn error occurred: {e.Message}"); // Log error
                // Operators ?, ?[], ??, or ??= are used
                return null; // Return null on fail
            }
        }
    }

    // --- Data Models ---

    // Root weather data object
    public class WeatherData
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("current")]
        public CurrentWeather Current { get; set; }

        [JsonPropertyName("hourly")]
        public HourlyWeather Hourly { get; set; }

        [JsonPropertyName("daily")]
        public DailyWeather Daily { get; set; }
    }

    // Current weather data
    public class CurrentWeather
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double Temperature2m { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int RelativeHumidity2m { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public double ApparentTemperature { get; set; }

        [JsonPropertyName("is_day")]
        public int IsDay { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed10m { get; set; }
    }

    // Hourly weather data (uses arrays)
    public class HourlyWeather
    {
        [JsonPropertyName("time")]
        public string[] Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double[] Temperature2m { get; set; }

        [JsonPropertyName("precipitation_probability")]
        public int[] PrecipitationProbability { get; set; }

        [JsonPropertyName("weather_code")]
        public int[] WeatherCode { get; set; }

        [JsonPropertyName("is_day")]
        public int[] IsDay { get; set; }
    }

    // Daily weather data (uses arrays)
    public class DailyWeather
    {
        [JsonPropertyName("time")]
        public string[] Time { get; set; }

        [JsonPropertyName("weather_code")]
        public int[] WeatherCode { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public double[] Temperature2mMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public double[] Temperature2mMin { get; set; }

        [JsonPropertyName("precipitation_probability_max")]
        public int[] PrecipitationProbabilityMax { get; set; }
    }
}