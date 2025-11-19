namespace MyWeatherApp.Core.Models // Project model namespace
{
    //abstract class
    public abstract class ForecastItemBase // Base class for forecast items
    {
        // Properties
        public string Icon { get; set; }
        public string WeatherDescription { get; set; }
    }
}