using System.Resources; // ResourceManager dependency

namespace MyWeatherApp.Resources.Strings // Project strings namespace
{
    // Static helper class
    public static class AppStringsHelper
    {
        // Resource manager instance
        private static readonly ResourceManager _resourceManager =
            new ResourceManager(typeof(AppStrings));

        // Gets a localized string
        public static string GetString(string key)
        {
            if (string.IsNullOrEmpty(key)) // Check key
                return string.Empty; // Return empty

            // Look up string by culture
            // Operators ?, ?[], ??, or ??= are used
            string? localizedString = _resourceManager.GetString(key, AppStrings.Culture);

            // Return string or key
            // Operators ?, ?[], ??, or ??= are used
            return localizedString ?? key;
        }
    }
}