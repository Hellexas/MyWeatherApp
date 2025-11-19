using MyWeatherApp.ViewModels;
using MyWeatherApp.Helpers;
using System.Globalization;

namespace MyWeatherApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(WeatherViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Only load weather if we haven't successfully loaded data yet
            // This prevents unnecessary API calls when navigating back or resuming
            if (BindingContext is WeatherViewModel vm && vm.LoadWeatherCommand.CanExecute(null))
            {
                if (vm.WeatherData == null)
                {
                    _ = vm.LoadWeatherCommand.ExecuteAsync(null);
                }
            }
        }

        private void EnglishButton_Clicked(object sender, EventArgs e)
        {
            ChangeLanguage("en");
        }

        private void LithuanianButton_Clicked(object sender, EventArgs e)
        {
            ChangeLanguage("lt");
        }

        private void ChangeLanguage(string cultureCode)
        {
            // 1. Update the underlying localization preference
            LocalizationHelper.SetLanguage(cultureCode);

            // 2. Update the Dynamic Resource Manager (updates XAML bindings instantly)
            var culture = new CultureInfo(cultureCode);
            LocalizationResourceManager.Instance.SetCulture(culture);

            // 3. Refresh the ViewModel data to update data-bound text (like "Light Rain")
            if (BindingContext is WeatherViewModel vm && vm.LoadWeatherCommand.CanExecute(null))
            {
                // Trigger a silent refresh to get translated weather descriptions
                _ = vm.LoadWeatherCommand.ExecuteAsync(null);
            }
        }
    }
}