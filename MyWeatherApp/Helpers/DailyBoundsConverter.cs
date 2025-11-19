using System.Globalization;
using Microsoft.Maui.Graphics;
using MyWeatherApp.Core.Models;

namespace MyWeatherApp.Helpers
{
    /// <summary>
    /// Converts a DailyForecastItem into a Rect for AbsoluteLayout.
    /// Used to position the temperature bar dynamically.
    /// </summary>
    public class DailyBoundsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DailyForecastItem item)
            {
                // Create a Rectangle: X, Y, Width, Height
                // X = BarStartFactor
                // Y = 0 (Top)
                // Width = BarWidthFactor
                // Height = 1 (100% of container height)
                return new Rect(item.BarStartFactor, 0, item.BarWidthFactor, 1);
            }

            // Default fallback
            return new Rect(0, 0, 0, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}