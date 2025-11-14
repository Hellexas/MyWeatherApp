using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MyWeatherApp.Core.Models
{
    //Inherits from abstract class
    // Implements IEquatable and IFormattable
    public sealed class DailyForecastItem : ForecastItemBase, IEquatable<DailyForecastItem>, IFormattable
    {
        public string WeatherIcon { get; set; }
        public DateTime Date { get; set; } //store the full DateTime object
        public string DayOfWeek { get; set; }
        public string DateDisplay { get; set; } // <-- This is the new property
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public int PrecipitationProbability { get; set; }

        //Implementation of IEquatable<T>
        public bool Equals(DailyForecastItem? other)
        {
            if (other is null) return false;
            // Two forecasts are equal if they are for the same day
            return Date.Date == other.Date.Date;
        }

        //Using 'is' operator
        public override bool Equals(object? obj) => Equals(obj as DailyForecastItem);
        public override int GetHashCode() => Date.Date.GetHashCode();

        //Implementation of operator overloading
        public static bool operator ==(DailyForecastItem? left, DailyForecastItem? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(DailyForecastItem? left, DailyForecastItem? right) => !(left == right);

        // --- Implementation of a deconstructor ---
        public void Deconstruct(out DateTime date, out double max, out double min)
        {
            date = this.Date;
            max = this.MaxTemp;
            min = this.MinTemp;
        }

        // Implementation of IFormattable
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            formatProvider ??= CultureInfo.CurrentCulture;
            format = (format ?? "G").ToUpperInvariant();

            return format switch
            {
                // "G" for General (Day (Date))
                "G" => $"{DayOfWeek} ({DateDisplay})", // Use DateDisplay here
                // "T" for Temperature
                "T" => $"{DayOfWeek}: {MaxTemp:F0}° / {MinTemp:F0}°",
                // "D" for Date only
                "D" => DateDisplay, // Use DateDisplay here
                // "F" for Full description
                "F" => $"{DayOfWeek} ({DateDisplay}): {WeatherDescription}. High {MaxTemp:F0}°, Low {MinTemp:F0}°. {PrecipitationProbability}% chance of rain.",
                _ => throw new FormatException($"The '{format}' format string is not supported.")
            };
        }
        public override string ToString() => ToString("G", CultureInfo.CurrentCulture);
    }
}