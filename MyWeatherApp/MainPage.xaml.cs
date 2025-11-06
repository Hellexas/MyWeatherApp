using MyWeatherApp.ViewModels;

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
    }
}