using MyWeatherApp.Core.Models;
using MyWeatherApp.Core.Services;
using MyWeatherApp.Core.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MyWeatherApp.Resources.Strings;
using System.Globalization;

namespace MyWeatherApp.ViewModels
{
    public partial class WeatherViewModel : ObservableObject
    {
        private readonly IWeatherService _weatherService;

        [ObservableProperty]
        private WeatherData _weatherData;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _currentWeatherDescription;

        [ObservableProperty]
        private string _currentWeatherIcon = "\uf07b";


        // This will hold the text for the "Last updated" label
        [ObservableProperty]
        private string _lastUpdatedDisplay = " ";

        public ObservableCollection<DailyForecastItem> DailyForecast { get; } = new();
        public ObservableCollection<HourlyForecastItem> HourlyForecast { get; } = new();

        public WeatherViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService;
            CurrentWeatherDescription = "Loading weather...";
        }

        public bool IsNotLoading => !IsLoading;

        [RelayCommand(CanExecute = nameof(IsNotLoading))]
        private async Task LoadWeatherAsync()
        {
            IsLoading = true;
            DailyForecast.Clear();
            HourlyForecast.Clear();

            try
            {
                WeatherData = await _weatherService.GetWeatherAsync(timezone: "Europe/Vilnius");

                if (WeatherData != null)
                {
                    var (icon, description) = WeatherCodeHelper.GetWeatherDisplayInfo(
                        WeatherData.Current.WeatherCode,
                        WeatherData.Current.IsDay == 1);

                    CurrentWeatherDescription = description;
                    CurrentWeatherIcon = icon;

                    ProcessDailyForecast();
                    ProcessHourlyForecast();

                    LogForecastProcessing("Daily", "Hourly");

                    // Set the display text to the current time
                    LastUpdatedDisplay = $"Last updated: {DateTime.Now:HH:mm}";
                }
                else
                {
                    CurrentWeatherDescription = "Failed to load data.";
                    LastUpdatedDisplay = "Update failed.";
                }
            }
            catch (Exception ex)
            {
                CurrentWeatherDescription = "An error occurred.";
                Console.WriteLine($"Error in LoadWeatherAsync: {ex.Message}");
                LastUpdatedDisplay = "Update failed.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ProcessDailyForecast()
        {
            if (WeatherData?.Daily?.Time == null) return;

            for (int i = 0; i < WeatherData.Daily.Time.Length; i++)
            {
                var date = DateTime.Parse(WeatherData.Daily.Time[i], CultureInfo.InvariantCulture);
                var (icon, description) = WeatherCodeHelper.GetWeatherDisplayInfo(WeatherData.Daily.WeatherCode[i], true);

                var dayItem = new DailyForecastItem
                {
                    Date = date,
                    DayOfWeek = (i == 0) ? "Today" : date.ToString("dddd"),
                    WeatherDescription = description,
                    WeatherIcon = icon,
                    MaxTemp = WeatherData.Daily.Temperature2mMax[i],
                    MinTemp = WeatherData.Daily.Temperature2mMin[i],
                    PrecipitationProbability = WeatherData.Daily.PrecipitationProbabilityMax[i]
                };
                DailyForecast.Add(dayItem);
            }
        }

        private void ProcessHourlyForecast()
        {
            if (WeatherData?.Hourly?.Time == null || WeatherData.Current?.Time == null) return;

            int startIndex = Array.IndexOf(WeatherData.Hourly.Time, WeatherData.Current.Time);

            if (startIndex == -1)
            {
                var currentTime = DateTime.Parse(WeatherData.Current.Time, CultureInfo.InvariantCulture);
                for (int j = 0; j < WeatherData.Hourly.Time.Length; j++)
                {
                    var hourlyTime = DateTime.Parse(WeatherData.Hourly.Time[j], CultureInfo.InvariantCulture);
                    if (hourlyTime >= currentTime)
                    {
                        startIndex = j;
                        break;
                    }
                }
                if (startIndex == -1) startIndex = 0;
            }

            int hoursToDisplay = 24;
            int endIndex = Math.Min(startIndex + hoursToDisplay, WeatherData.Hourly.Time.Length);

            Range range = startIndex..endIndex;
            string[] timeSlice = WeatherData.Hourly.Time[range];
            double[] tempSlice = WeatherData.Hourly.Temperature2m[range];
            int[] precipSlice = WeatherData.Hourly.PrecipitationProbability[range];
            int[] codeSlice = WeatherData.Hourly.WeatherCode[range];
            int[] isDaySlice = WeatherData.Hourly.IsDay[range];

            for (int i = 0; i < timeSlice.Length; i++)
            {
                var time = DateTime.Parse(timeSlice[i], CultureInfo.InvariantCulture);
                bool isDay = isDaySlice[i] == 1;
                var (icon, description) = WeatherCodeHelper.GetWeatherDisplayInfo(codeSlice[i], isDay);

                var item = new HourlyForecastItem
                {
                    Time = time,
                    TimeDisplay = (i == 0) ? "Now" : time.ToString("HH:00"),
                    Icon = icon,
                    WeatherDescription = description,
                    Temperature = tempSlice[i],
                    PrecipitationChance = precipSlice[i]
                };
                HourlyForecast.Add(item);
            }
        }

        private void LogForecastProcessing(params string[] forecastTypes)
        {
            Console.WriteLine($"Successfully processed {forecastTypes.Length} forecast types:");
            foreach (var type in forecastTypes)
            {
                Console.WriteLine($"- {type}");
            }
        }

        partial void OnIsLoadingChanged(bool value)
        {
            LoadWeatherCommand.NotifyCanExecuteChanged();
        }
    }
}