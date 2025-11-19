using MyWeatherApp.ViewModels;
using MyWeatherApp.Core.Services;
using Microsoft.Extensions.Logging;

namespace MyWeatherApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // Registers the default app fonts
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FontAwesomeSolid");
                });

            // --- DEPENDENCY INJECTION SECTION ---
            builder.Services.AddSingleton<IWeatherService, WeatherService>();

            builder.Services.AddTransient<WeatherViewModel>();
            builder.Services.AddTransient<MainPage>();
            // --- END OF INJECTION SECTION ---


#if DEBUG
            // Enables debug logging
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}