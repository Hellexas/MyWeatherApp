using System;
using System.Globalization;

namespace MyWeatherApp.Core.Models
{
    public sealed class DailyForecastItem : ForecastItemBase, IEquatable<DailyForecastItem>, IFormattable
    {
        public string WeatherIcon { get; set; }
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public string DateDisplay { get; set; }
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public int PrecipitationProbability { get; set; }

        // --- POSITIONING PROPERTIES ---
        // (Calculated relative to the week's range for layout)
        public double BarStartFactor { get; set; }
        public double BarWidthFactor { get; set; }

        // Removed Color Properties - Logic moved to UI Converter

        public bool Equals(DailyForecastItem? other)
        {
            if (other is null) return false;
            return Date.Date == other.Date.Date;
        }

        public override bool Equals(object? obj) => Equals(obj as DailyForecastItem);
        public override int GetHashCode() => Date.Date.GetHashCode();

        public static bool operator ==(DailyForecastItem? left, DailyForecastItem? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(DailyForecastItem? left, DailyForecastItem? right) => !(left == right);

        public void Deconstruct(out DateTime date, out double max, out double min)
        {
            date = this.Date;
            max = this.MaxTemp;
            min = this.MinTemp;
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            formatProvider ??= CultureInfo.CurrentCulture;
            format = (format ?? "G").ToUpperInvariant();

            return format switch
            {
                "G" => $"{DayOfWeek} ({DateDisplay})",
                "T" => $"{DayOfWeek}: {MaxTemp:F0}° / {MinTemp:F0}°",
                "D" => DateDisplay,
                "F" => $"{DayOfWeek} ({DateDisplay}): {WeatherDescription}. High {MaxTemp:F0}°, Low {MinTemp:F0}°. {PrecipitationProbability}% chance of rain.",
                _ => throw new FormatException($"The '{format}' format string is not supported.")
            };
        }
        public override string ToString() => ToString("G", CultureInfo.CurrentCulture);
    }
}