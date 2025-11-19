using System.Resources;

namespace MyWeatherApp.Resources.Strings
{
    public static class AppStringsHelper
    {
        private static readonly ResourceManager _resourceManager =
            new ResourceManager(typeof(AppStrings));

        public static string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            // This looks up the string using the app's CURRENT culture
            string? localizedString = _resourceManager.GetString(key, AppStrings.Culture);

            // Return the found string, or the key name if not found
            return localizedString ?? key;
        }
    }
}