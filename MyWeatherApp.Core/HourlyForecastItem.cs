using System; // System dependency

namespace MyWeatherApp.Core.Models // Project model namespace
{
    // abstract class (inherits from ForecastItemBase)
    // implemented IComparable<T>
    // Sealed class
    public sealed class HourlyForecastItem : ForecastItemBase, IComparable<HourlyForecastItem> // Hourly forecast data
    {
        // Properties
        public DateTime Time { get; set; }
        public string TimeDisplay { get; set; }
        public double Temperature { get; set; }
        public int PrecipitationChance { get; set; }

        // Implemented IComparable<T>
        // Operators ?, ?[], ??, or ??= are used
        public int CompareTo(HourlyForecastItem? other) // Sort by time
        {
            // The is operator is used
            // Pattern matching is used
            if (other is null) return 1; // Nulls last
            return this.Time.CompareTo(other.Time); // Compare by Time property
        }
    }
}