using FoodNinja.Pages;
using FoodNinja.Pages.Onboarding_Screen;
using FoodNinja.Services;
using Syncfusion.Licensing;

namespace FoodNinja
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("MzUxMTQ3N0AzMjM3MmUzMDJlMzBFYjJ3Z2R4ZzNOV2xTUXRxZ3JLUG8rbzBqWVZIWlBOMmVvOG8vZG1vdDFNPQ==");
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);
            if (isLoggedIn)
            {
                //MainPage = new NavigationPage(new AppShell());
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new NavigationPage(new SplashScreen());
            }
            DependencyService.Register<RestaurantService>();
        }
    }
}
