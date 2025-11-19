using MyWeatherApp.ViewModels;
using MyWeatherApp.Helpers; // <-- Make sure this is here

namespace MyWeatherApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(WeatherViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is WeatherViewModel vm && vm.LoadWeatherCommand.CanExecute(null))
            {
                _ = vm.LoadWeatherCommand.ExecuteAsync(null);
            }
        }

        // v-- ALL THE NEW METHODS MUST GO INSIDE THIS CLASS --v

        private void EnglishButton_Clicked(object sender, EventArgs e)
        {
            // 1. Set language to English
            LocalizationHelper.SetLanguage("en");

            // 2. Reload the page to apply changes
            ReloadPage();
        }

        private void LithuanianButton_Clicked(object sender, EventArgs e)
        {
            // 1. Set language to Lithuanian
            LocalizationHelper.SetLanguage("lt");

            // 2. Reload the page to apply changes
            ReloadPage();
        }

        private void ReloadPage()
        {
            // Re-create the main page and shell to force a full UI reload
            Application.Current.MainPage = new AppShell();
        }

    } // <-- This is the FINAL closing brace for the MainPage class
}