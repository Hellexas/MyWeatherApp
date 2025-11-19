using MyWeatherApp.Core.Models;
using MyWeatherApp.Core.Services;
using MyWeatherApp.Core.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MyWeatherApp.Resources.Strings;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Graphics;

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

        [ObservableProperty]
        private string _lastUpdatedDisplay = " ";

        [ObservableProperty]
        private string _windDisplay;

        [ObservableProperty]
        private string _humidityDisplay;

        [ObservableProperty]
        private string _feelsLikeDisplay;

        [ObservableProperty]
        private string _hourlyForecastTitle;

        [ObservableProperty]
        private string _dailyForecastTitle;

        [ObservableProperty]
        private Brush _backgroundBrush;

        public ObservableCollection<DailyForecastItem> DailyForecast { get; } = new();
        public ObservableCollection<HourlyForecastItem> HourlyForecast { get; } = new();

        public WeatherViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService;
            CurrentWeatherDescription = AppStrings.LoadingWeather;

            UpdateBackground(0, 1);

            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMinutes(5);
            _timer.Tick += (s, e) => Timer_Tick();
            _timer.Start();
        }

        public bool IsNotLoading => !IsLoading;

        [RelayCommand(CanExecute = nameof(IsNotLoading))]
        private async Task LoadWeatherAsync()
        {
            IsLoading = true;

            try
            {
                WeatherData = await _weatherService.GetWeatherAsync(timezone: "Europe/Vilnius");

                if (WeatherData != null)
                {
                    DailyForecast.Clear();
                    HourlyForecast.Clear();

                    var (icon, descriptionKey) = WeatherCodeHelper.GetWeatherDisplayInfo(
                        WeatherData.Current.WeatherCode,
                        WeatherData.Current.IsDay == 1);

                    CurrentWeatherDescription = AppStringsHelper.GetString(descriptionKey);
                    CurrentWeatherIcon = icon;

                    WindDisplay = string.Format(AppStrings.Wind, WeatherData.Current.WindSpeed10m);
                    HumidityDisplay = string.Format(AppStrings.Humidity, WeatherData.Current.RelativeHumidity2m);
                    FeelsLikeDisplay = string.Format(AppStrings.FeelsLike, WeatherData.Current.ApparentTemperature);

                    HourlyForecastTitle = AppStrings.HourlyForecastTitle;
                    DailyForecastTitle = AppStrings.SevenDayForecastTitle;

                    UpdateBackground(WeatherData.Current.WeatherCode, WeatherData.Current.IsDay);

                    ProcessDailyForecast();
                    ProcessHourlyForecast();

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

            // Calculate global range ONLY for positioning (Layout)
            double weekMin = WeatherData.Daily.Temperature2mMin.Min();
            double weekMax = WeatherData.Daily.Temperature2mMax.Max();
            double tempRange = weekMax - weekMin;
            if (tempRange < 1) tempRange = 1;

            for (int i = 0; i < WeatherData.Daily.Time.Length; i++)
            {
                var date = DateTime.Parse(WeatherData.Daily.Time[i], CultureInfo.InvariantCulture);
                var (icon, descriptionKey) = WeatherCodeHelper.GetWeatherDisplayInfo(WeatherData.Daily.WeatherCode[i], true);

                double dayMin = WeatherData.Daily.Temperature2mMin[i];
                double dayMax = WeatherData.Daily.Temperature2mMax[i];

                // Positioning: Relative to week (Fills the space)
                double startFactor = (dayMin - weekMin) / tempRange;
                double widthFactor = (dayMax - dayMin) / tempRange;
                if (widthFactor < 0.05) widthFactor = 0.05;

                var dayItem = new DailyForecastItem
                {
                    Date = date,
                    DayOfWeek = GetLocalizedDayName(date, i),
                    DateDisplay = GetLocalizedDateString(date),
                    WeatherDescription = AppStringsHelper.GetString(descriptionKey),
                    WeatherIcon = icon,
                    MaxTemp = dayMax,
                    MinTemp = dayMin,
                    PrecipitationProbability = WeatherData.Daily.PrecipitationProbabilityMax[i],

                    BarStartFactor = startFactor,
                    BarWidthFactor = widthFactor,
                };
                DailyForecast.Add(dayItem);
            }
        }

        // ... [ProcessHourlyForecast, etc. unchanged] ...
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
            if (index == 0) return AppStrings.Today;
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => AppStrings.Monday,
                DayOfWeek.Tuesday => AppStrings.Tuesday,
                DayOfWeek.Wednesday => AppStrings.Wednesday,
                DayOfWeek.Thursday => AppStrings.Thursday,
                DayOfWeek.Friday => AppStrings.Friday,
                DayOfWeek.Saturday => AppStrings.Saturday,
                DayOfWeek.Sunday => AppStrings.Sunday,
                _ => date.DayOfWeek.ToString()
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
                _ => date.ToString("MMM")
            };
            return $"{month} {date.Day}";
        }

        private void Timer_Tick()
        {
            if (LoadWeatherCommand.CanExecute(null))
                _ = LoadWeatherCommand.ExecuteAsync(null);
        }

        private void UpdateBackground(int code, int isDayInt)
        {
            bool isDay = isDayInt == 1;

            Color startColor, endColor;

            if (code <= 1)
            {
                if (isDay)
                {
                    startColor = Color.FromArgb("#2980B9");
                    endColor = Color.FromArgb("#6DD5FA");
                }
                else
                {
                    startColor = Color.FromArgb("#0f2027");
                    endColor = Color.FromArgb("#2c5364");
                }
            }
            else if (code <= 3 || code == 45 || code == 48)
            {
                if (isDay)
                {
                    startColor = Color.FromArgb("#606c88");
                    endColor = Color.FromArgb("#3f4c6b");
                }
                else
                {
                    startColor = Color.FromArgb("#232526");
                    endColor = Color.FromArgb("#414345");
                }
            }
            else if ((code >= 51 && code <= 67) || (code >= 80 && code <= 82))
            {
                if (isDay)
                {
                    startColor = Color.FromArgb("#373B44");
                    endColor = Color.FromArgb("#4286f4");
                }
                else
                {
                    startColor = Color.FromArgb("#000000");
                    endColor = Color.FromArgb("#434343");
                }
            }
            else if ((code >= 71 && code <= 77) || code == 85 || code == 86)
            {
                if (isDay)
                {
                    startColor = Color.FromArgb("#83a4d4");
                    endColor = Color.FromArgb("#b6fbff");
                }
                else
                {
                    startColor = Color.FromArgb("#0F2027");
                    endColor = Color.FromArgb("#2C5364");
                }
            }
            else if (code >= 95)
            {
                startColor = Color.FromArgb("#141E30");
                endColor = Color.FromArgb("#243B55");
            }
            else
            {
                startColor = Color.FromArgb("#2C3E50");
                endColor = Color.FromArgb("#4CA1AF");
            }

            var gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };
            gradient.GradientStops.Add(new GradientStop(startColor, 0.0f));
            gradient.GradientStops.Add(new GradientStop(endColor, 1.0f));

            BackgroundBrush = gradient;
        }
    }
}