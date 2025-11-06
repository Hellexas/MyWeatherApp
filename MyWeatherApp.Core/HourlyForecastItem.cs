using System;

namespace MyWeatherApp.Core.Models
{
    // Inherits from abstract class
    // Implements IComparable (for sorting by time)
    public sealed class HourlyForecastItem : ForecastItemBase, IComparable<HourlyForecastItem>
    {
        public DateTime Time { get; set; }
        public string TimeDisplay { get; set; }
        public double Temperature { get; set; }
        public int PrecipitationChance { get; set; }

        // --- Implementation of IComparable<T> ---
        // This defines the "natural" sort order for this object.
        public int CompareTo(HourlyForecastItem? other)
        {
            // We sort by the Time.
            if (other is null) return 1;
            return this.Time.CompareTo(other.Time);
        }
    }
}