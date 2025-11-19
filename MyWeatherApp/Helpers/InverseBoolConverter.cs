using System.Globalization;
using Microsoft.Maui.Controls;

namespace MyWeatherApp.Helpers
{
    /// A custom value converter that inverts a boolean value.
    /// This is used in XAML to hide an element when a property is 'true'
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            // It checks if the value is actually a bool.
            if (value is not bool boolValue)
                return false; // Return a default value if it's not a bool

            // Return the opposite of the boolean value
            return !boolValue;
        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {

            // This is the "nullable-aware" and safe version.
            if (value is not bool boolValue)
                return false; // Return a default value

            // Return the opposite (which is the original)
            return !boolValue;
        }
    }
}