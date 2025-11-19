using MyWeatherApp.Resources.Strings;
using System.ComponentModel;
using System.Globalization;

namespace MyWeatherApp.Helpers
{
    public class LocalizationResourceManager : INotifyPropertyChanged
    {
        private static LocalizationResourceManager _instance;
        public static LocalizationResourceManager Instance => _instance ??= new LocalizationResourceManager();

        public object this[string resourceKey] => AppStrings.ResourceManager.GetObject(resourceKey, AppStrings.Culture) ?? Array.Empty<byte>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetCulture(CultureInfo culture)
        {
            AppStrings.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); // Update all bindings
        }
    }
}