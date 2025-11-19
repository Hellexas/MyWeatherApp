using MyWeatherApp.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyWeatherApp.Core.Services
{
    //Sealed class
    public sealed class WeatherService : IWeatherService
    {
        //single HttpClient instance
        private static readonly HttpClient _client;
        private static readonly Dictionary<string, (double Lat, double Lon)> _cityCoordinates;

        //Static constructor
        static WeatherService()
        {
            _client = new HttpClient();
            _cityCoordinates = new Dictionary<string, (double Lat, double Lon)>
            {
                { "Europe/Vilnius", (54.72, 25.24) }
            };
        }

        //Initialization using out arguments
        // uses null-coalescing ?? operator
        private string GetApiUrl(string? timezone)
        {
            // Set a default timezone if the provided one is null
            string tzKey = timezone ?? "Europe/Vilnius";

            // Use TryGetValue which uses an 'out' argument for initialization
            if (!_cityCoordinates.TryGetValue(tzKey, out var coords))
            {
                coords = _cityCoordinates["Europe/Vilnius"];
            }

            return $"https://api.open-meteo.com/v1/forecast?latitude={coords.Lat}&longitude={coords.Lon}&timezone={tzKey}&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,weather_code,wind_speed_10m&hourly=temperature_2m,cloud_cover,precipitation_probability,weather_code,is_day&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max";
        }

        //Default arguments
        public async Task<WeatherData> GetWeatherAsync(string? timezone = "Europe/Vilnius")
        {
            string apiUrl = GetApiUrl(timezone);

            try
            {
                HttpResponseMessage response = await _client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                //Using null-conditional operator ?. and ??
                WeatherData? weatherData = JsonSerializer.Deserialize<WeatherData>(jsonResponse);
                return weatherData ?? throw new JsonException("Failed to deserialize weather data.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nAn error occurred: {e.Message}");
                return null;
            }
        }
    }

    // --- Data Models ---

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

    //Using arrays instead of List<T> to allow for Range operators
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

    //Using arrays instead of List<T>
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