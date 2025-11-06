namespace MyWeatherApp.Core.Models
{
    public abstract class ForecastItemBase
    {
        public string Icon { get; set; }
        public string WeatherDescription { get; set; }
    }
}