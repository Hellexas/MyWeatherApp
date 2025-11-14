using MyWeatherApp.Helpers;

namespace MyWeatherApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            LocalizationHelper.LoadLanguage();

            MainPage = new AppShell();
        }
    }
}