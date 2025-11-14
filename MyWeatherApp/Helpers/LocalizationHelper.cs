using System.Globalization;
using MyWeatherApp.Resources.Strings;

namespace MyWeatherApp.Helpers
{
    public static class LocalizationHelper
    {
        public static void SetLanguage(string cultureName)
        {
            // 1. Set the culture for the AppStrings resource file
            AppStrings.Culture = new CultureInfo(cultureName);

            // 2. Persist the user's choice
            // This ensures the app remembers the language next time it starts
            Preferences.Set("user_language", cultureName);
        }

        public static void LoadLanguage()
        {
            // 1. Check if a language is already saved
            string cultureName = Preferences.Get("user_language", null);

            if (string.IsNullOrEmpty(cultureName))
            {
                // No language saved, use the device default (which is the
                // default behavior, so we do nothing)
                return;
            }

            // 2. A language was saved, so set it
            try
            {
                AppStrings.Culture = new CultureInfo(cultureName);
            }
            catch (Exception)
            {
                // Handle a potential bad culture name in preferences
                AppStrings.Culture = null; // Fall back to device default
                Preferences.Clear("user_language");
            }
        }
    }
}