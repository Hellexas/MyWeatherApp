using System.Globalization; // CultureInfo dependency
using MyWeatherApp.Resources.Strings; // AppStrings dependency

namespace MyWeatherApp.Helpers // Project helper namespace
{
    // Static helper class
    public static class LocalizationHelper
    {
        // Sets the application language
        public static void SetLanguage(string cultureName)
        {
            // Set resource culture
            AppStrings.Culture = new CultureInfo(cultureName);

            // Save choice
            Preferences.Set("user_language", cultureName);
        }

        // Loads the saved language
        public static void LoadLanguage()
        {
            // Get saved language
            // Default and named arguments are used
            // Operators ?, ?[], ??, or ??= are used
            string cultureName = Preferences.Get("user_language", null);

            if (string.IsNullOrEmpty(cultureName))
            {
                // No language saved, do nothing
                return;
            }

            // Set saved language
            try
            {
                AppStrings.Culture = new CultureInfo(cultureName);
            }
            catch (Exception)
            {
                // Handle bad preference
                // Operators ?, ?[], ??, or ??= are used
                AppStrings.Culture = null; // Fallback
                Preferences.Clear("user_language");
            }
        }
    }
}