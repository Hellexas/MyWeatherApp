using System.Globalization; // CultureInfo dependency
using Microsoft.Maui.Controls; // IValueConverter dependency

namespace MyWeatherApp.Helpers // Project helper namespace
{
    // Custom value converter
    public class InverseBoolConverter : IValueConverter
    {
        // Operators ?, ?[], ??, or ??= are used
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) // Convert method
        {
            // The is operator is used
            // Pattern matching is used
            if (value is not bool boolValue) // Check type
                return false; // Default value

            return !boolValue; // Return inverted value
        }

        // Operators ?, ?[], ??, or ??= are used
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) // Convert back method
        {
            // The is operator is used
            // Pattern matching is used
            if (value is not bool boolValue) // Check type
                return false; // Default value

            return !boolValue; // Return inverted value
        }
    }
}