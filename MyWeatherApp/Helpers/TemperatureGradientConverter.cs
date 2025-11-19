using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MyWeatherApp.Core.Models;

namespace MyWeatherApp.Helpers
{
    /// <summary>
    /// Dynamic LinearGradientBrush based on Absolute Temperature.
    /// Pivots with a neutral grey zone between -0.5°C and +0.5°C.
    /// Positive > 0.5 = Red, Negative < -0.5 = Blue.
    /// </summary>
    public class TemperatureGradientConverter : IValueConverter
    {
        // Configuration for "Max Intensity"
        // Temperatures at or beyond these values get the full color.
        private const double MaxHeat = 23.0; // 21°C is Full Red
        private const double MaxCold = -11.0; // -11°C is Full Blue

        // The range where the color remains strictly neutral/grey
        private const double NeutralThreshold = 0.5;

        // Colors
        private readonly Color _hotColor = Color.FromArgb("#FF5252"); // Bright Red
        private readonly Color _coldColor = Color.FromArgb("#448AFF"); // Bright Blue
        private readonly Color _neutralColor = Color.FromArgb("#F5F5F5"); // Light Grey

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DailyForecastItem item)
            {
                var brush = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)
                };

                // Calculate start and end colors based on Absolute Temp
                Color startColor = GetColorForTemp(item.MinTemp);
                Color endColor = GetColorForTemp(item.MaxTemp);

                brush.GradientStops.Add(new GradientStop(startColor, 0.0f));

                // Insert stops to preserve the neutral grey zone (-0.5 to +0.5)
                double range = item.MaxTemp - item.MinTemp;

                if (range > 0)
                {
                    // 1. If the range crosses the Cold boundary (-1.5°C)
                    // We insert a Grey stop exactly at -0.5°C
                    if (item.MinTemp < -NeutralThreshold && item.MaxTemp > -NeutralThreshold)
                    {
                        double offset = (-NeutralThreshold - item.MinTemp) / range;
                        brush.GradientStops.Add(new GradientStop(_neutralColor, (float)offset));
                    }

                    // 2. If the range crosses the Warm boundary (+0.5°C)
                    // We insert a Grey stop exactly at +0.5°C
                    if (item.MinTemp < NeutralThreshold && item.MaxTemp > NeutralThreshold)
                    {
                        double offset = (NeutralThreshold - item.MinTemp) / range;
                        brush.GradientStops.Add(new GradientStop(_neutralColor, (float)offset));
                    }
                }

                brush.GradientStops.Add(new GradientStop(endColor, 1.0f));
                return brush;
            }

            return new SolidColorBrush(Colors.Gray);
        }

        private Color GetColorForTemp(double temp)
        {
            // If within the expanded neutral range, return Grey
            if (Math.Abs(temp) <= NeutralThreshold) return _neutralColor;

            if (temp > 0)
            {
                // Heat: Interpolate Grey -> Red
                float t = (float)Math.Clamp(temp / MaxHeat, 0, 1);
                return LerpColor(_neutralColor, _hotColor, t);
            }
            else
            {
                // Cold: Interpolate Grey -> Blue
                float t = (float)Math.Clamp(Math.Abs(temp) / Math.Abs(MaxCold), 0, 1);
                return LerpColor(_neutralColor, _coldColor, t);
            }
        }

        private Color LerpColor(Color a, Color b, float t)
        {
            return Color.FromRgba(
                a.Red + (b.Red - a.Red) * t,
                a.Green + (b.Green - a.Green) * t,
                a.Blue + (b.Blue - a.Blue) * t,
                1.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}