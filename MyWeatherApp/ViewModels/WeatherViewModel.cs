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
        private IDispatcherTimer _timer;

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
            CurrentWeatherDescription = AppStrings.LoadingWeather;

            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMinutes(5); //auto update every 5 mins
            _timer.Tick += (s, e) => Timer_Tick();
            _timer.Start();
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
                    var (icon, descriptionKey) = WeatherCodeHelper.GetWeatherDisplayInfo(
                        WeatherData.Current.WeatherCode,
                        WeatherData.Current.IsDay == 1);

                    CurrentWeatherDescription = AppStringsHelper.GetString(descriptionKey);
                    CurrentWeatherIcon = icon;

                    ProcessDailyForecast();
                    ProcessHourlyForecast();

                    LogForecastProcessing("Daily", "Hourly");

                    // Set the display text to the current time
                    LastUpdatedDisplay = string.Format(AppStrings.LastUpdated, DateTime.Now);
                }
                else
                {
                    CurrentWeatherDescription = AppStrings.FailedToLoad;
                    LastUpdatedDisplay = AppStrings.UpdateFailed;
                }
            }
            catch (Exception ex)
            {
                CurrentWeatherDescription = AppStrings.FailedToLoad;
                Console.WriteLine($"Error in LoadWeatherAsync: {ex.Message}");
                LastUpdatedDisplay = AppStrings.UpdateFailed;
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
                var (icon, descriptionKey) = WeatherCodeHelper.GetWeatherDisplayInfo(WeatherData.Daily.WeatherCode[i], true);

                var dayItem = new DailyForecastItem
                {
                    Date = date,
                    DayOfWeek = GetLocalizedDayName(date, i),
                    DateDisplay = GetLocalizedDateString(date),
                    WeatherDescription = AppStringsHelper.GetString(descriptionKey),
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
                var (icon, descriptionKey) = WeatherCodeHelper.GetWeatherDisplayInfo(codeSlice[i], isDay);

                var item = new HourlyForecastItem
                {
                    Time = time,
                    TimeDisplay = (i == 0) ? AppStrings.TimeNow : time.ToString("HH:00"),
                    Icon = icon,
                    WeatherDescription = AppStringsHelper.GetString(descriptionKey),
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

        private string GetLocalizedDayName(DateTime date, int index)
        {
            if (index == 0)
            {
                return AppStrings.Today;
            }

            // to return the matching string from AppStrings.resx
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => AppStrings.Monday,
                DayOfWeek.Tuesday => AppStrings.Tuesday,
                DayOfWeek.Wednesday => AppStrings.Wednesday,
                DayOfWeek.Thursday => AppStrings.Thursday,
                DayOfWeek.Friday => AppStrings.Friday,
                DayOfWeek.Saturday => AppStrings.Saturday,
                DayOfWeek.Sunday => AppStrings.Sunday,
                _ => date.DayOfWeek.ToString() // Fallback just in case
            };
        }
        private string GetLocalizedDateString(DateTime date)
        {
            string month = date.Month switch
            {
                1 => AppStrings.Month1,
                2 => AppStrings.Month2,
                3 => AppStrings.Month3,
                4 => AppStrings.Month4,
                5 => AppStrings.Month5,
                6 => AppStrings.Month6,
                7 => AppStrings.Month7,
                8 => AppStrings.Month8,
                9 => AppStrings.Month9,
                10 => AppStrings.Month10,
                11 => AppStrings.Month11,
                12 => AppStrings.Month12,
                _ => date.ToString("MMM") // Fallback to 3-letter abbreviation
            };

            return $"{month} {date.Day}";
        }

        private void Timer_Tick()
        {
            // Check if a refresh is already in progress
            if (LoadWeatherCommand.CanExecute(null))
            {
                _ = LoadWeatherCommand.ExecuteAsync(null);
            }
        }
    }
}