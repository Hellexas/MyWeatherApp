using MyWeatherApp.Helpers; // Helper dependency

namespace MyWeatherApp // Project namespace
{
    // Partial class
    public partial class App : Application // Main application class
    {
        public App() // Constructor
        {
            InitializeComponent(); // Load XAML components

            LocalizationHelper.LoadLanguage(); // Load language settings

            MainPage = new AppShell(); // Set the root page
        }
    }
}