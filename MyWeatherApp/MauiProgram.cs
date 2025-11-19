using MyWeatherApp.ViewModels; // Viewmodel dependency
// The project consists of more than one module (assembly)
using MyWeatherApp.Core.Services; // Service dependency
using Microsoft.Extensions.Logging; // Logging dependency

namespace MyWeatherApp // Project namespace
{
    // Main program entry
    public static class MauiProgram
    {
        // Creates the Maui app
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder(); // App builder
            builder
                .UseMauiApp<App>() // Use App class
                                   // Delegates or lambda functions are used
                .ConfigureFonts(fonts => // Font configuration
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FontAwesomeSolid");
                });

            // Dependency Injection
            builder.Services.AddSingleton<IWeatherService, WeatherService>();
            builder.Services.AddTransient<WeatherViewModel>();
            builder.Services.AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug(); // Enable debug logging
#endif

            return builder.Build(); // Build and return app
        }
    }
}