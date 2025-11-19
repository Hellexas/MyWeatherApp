using System;
using System.Collections.Generic;

namespace MyWeatherApp.Core.Helpers
{
    [Flags]
    public enum WeatherCondition
    {
        None = 0,
        Clear = 1 << 0,
        Clouds = 1 << 1,
        Fog = 1 << 2,
        Drizzle = 1 << 3,
        Rain = 1 << 4,
        Snow = 1 << 5,
        Showers = 1 << 6,
        Thunderstorm = 1 << 7,
        Light = 1 << 8,
        Moderate = 1 << 9,
        Heavy = 1 << 10,
        Freezing = 1 << 11,
        Hail = 1 << 12
    }

    public static class WeatherCodeHelper
    {
        private static readonly Dictionary<int, WeatherCondition> _codeMap = new()
        {
            {0, WeatherCondition.Clear},
            {1, WeatherCondition.Clear | WeatherCondition.Light | WeatherCondition.Clouds},
            {2, WeatherCondition.Clouds | WeatherCondition.Moderate},
            {3, WeatherCondition.Clouds | WeatherCondition.Heavy},
            {45, WeatherCondition.Fog},
            {48, WeatherCondition.Fog | WeatherCondition.Freezing},
            {51, WeatherCondition.Drizzle | WeatherCondition.Light},
            {53, WeatherCondition.Drizzle | WeatherCondition.Moderate},
            {55, WeatherCondition.Drizzle | WeatherCondition.Heavy},
            {56, WeatherCondition.Drizzle | WeatherCondition.Light | WeatherCondition.Freezing},
            {57, WeatherCondition.Drizzle | WeatherCondition.Heavy | WeatherCondition.Freezing},
            {61, WeatherCondition.Rain | WeatherCondition.Light},
            {63, WeatherCondition.Rain | WeatherCondition.Moderate},
            {65, WeatherCondition.Rain | WeatherCondition.Heavy},
            {66, WeatherCondition.Rain | WeatherCondition.Light | WeatherCondition.Freezing},
            {67, WeatherCondition.Rain | WeatherCondition.Heavy | WeatherCondition.Freezing},
            {71, WeatherCondition.Snow | WeatherCondition.Light},
            {73, WeatherCondition.Snow | WeatherCondition.Moderate},
            {75, WeatherCondition.Snow | WeatherCondition.Heavy},
            {77, WeatherCondition.Snow},
            {80, WeatherCondition.Rain | WeatherCondition.Showers | WeatherCondition.Light},
            {81, WeatherCondition.Rain | WeatherCondition.Showers | WeatherCondition.Moderate},
            {82, WeatherCondition.Rain | WeatherCondition.Showers | WeatherCondition.Heavy},
            {85, WeatherCondition.Snow | WeatherCondition.Showers | WeatherCondition.Light},
            {86, WeatherCondition.Snow | WeatherCondition.Showers | WeatherCondition.Heavy},
            {95, WeatherCondition.Thunderstorm},
            {96, WeatherCondition.Thunderstorm | WeatherCondition.Hail | WeatherCondition.Light},
            {99, WeatherCondition.Thunderstorm | WeatherCondition.Hail | WeatherCondition.Heavy}
        };

        public static (string Icon, string Description) GetWeatherDisplayInfo(int code, bool isDay)
        {
            _codeMap.TryGetValue(code, out var condition);
            return GetWeatherDisplayInfo(condition, isDay);
        }

        private static (string Icon, string Description) GetWeatherDisplayInfo(WeatherCondition condition, bool isDay)
        {
            // Reverted to Font Awesome Unicode Characters
            string icon;
            string descriptionKey;

            icon = (condition, isDay) switch
            {
                (var c, _) when (c & WeatherCondition.Thunderstorm) != 0 => "\uf0e7", // bolt
                (var c, _) when (c & WeatherCondition.Snow) != 0 => "\uf2dc", // snowflake
                (var c, true) when (c & (WeatherCondition.Rain | WeatherCondition.Drizzle | WeatherCondition.Showers)) != 0 => "\uf740", // cloud-sun-rain
                (var c, false) when (c & (WeatherCondition.Rain | WeatherCondition.Drizzle | WeatherCondition.Showers)) != 0 => "\uf73c", // cloud-moon-rain
                (var c, _) when (c & WeatherCondition.Fog) != 0 => "\uf75f", // smog
                (var c, true) when (c & WeatherCondition.Clouds) != 0 => "\uf6c4", // cloud-sun
                (var c, false) when (c & WeatherCondition.Clouds) != 0 => "\uf6c3", // cloud-moon
                (_, true) => "\uf185", // sun
                (_, false) => "\uf186", // moon
            };

            descriptionKey = condition switch
            {
                var c when (c & WeatherCondition.Thunderstorm) != 0 => "WeatherThunderstorm",
                var c when (c & WeatherCondition.Snow) != 0 && (c & WeatherCondition.Heavy) != 0 => "WeatherSnowHeavy",
                var c when (c & WeatherCondition.Snow) != 0 => "WeatherSnow",
                var c when (c & WeatherCondition.Rain) != 0 && (c & WeatherCondition.Heavy) != 0 => "WeatherRainHeavy",
                var c when (c & WeatherCondition.Rain) != 0 => "WeatherRain",
                var c when (c & WeatherCondition.Drizzle) != 0 => "WeatherDrizzle",
                var c when (c & WeatherCondition.Fog) != 0 => "WeatherFog",
                var c when (c & WeatherCondition.Clouds) != 0 && (c & WeatherCondition.Heavy) != 0 => "WeatherOvercast",
                var c when (c & WeatherCondition.Clouds) != 0 => "WeatherPartlyCloudy",
                WeatherCondition.Clear when (condition & WeatherCondition.Clouds) != 0 => "WeatherMainlyClear",
                WeatherCondition.Clear => "WeatherClear",
                _ => "WeatherUnknown"
            };

            if (condition == (WeatherCondition.Clear | WeatherCondition.Light | WeatherCondition.Clouds))
            {
                descriptionKey = "WeatherMainlyClear";
            }

            return (icon, descriptionKey);
        }
    }
}