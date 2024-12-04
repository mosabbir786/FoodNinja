using CommunityToolkit.Mvvm.Messaging;
using FoodNinja.Pages;
using FoodNinja.Pages.Onboarding_Screen;
using FoodNinja.Pages.ProfileTabScreen;
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
            Preferences.Remove("ReturnFromPage");
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);
            if (isLoggedIn)
            {
                 MainPage = new AppShell();
            }
            else
            {
                 MainPage = new NavigationPage(new SplashScreen());
            }
            DependencyService.Register<RestaurantService>();
            WeakReferenceMessenger.Default.Register<PushNotificationReceived>(this, (recipient, message) =>
            {
                HandleNavigationFromNotification();
            });
        }

        private void HandleNavigationFromNotification()
        {
            if (Preferences.ContainsKey("NavigationID"))
            {
                string navigationId = Preferences.Get("NavigationID", string.Empty);
                Preferences.Remove("NavigationID");

                if(!string.IsNullOrEmpty(navigationId))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new PreviousOrderDetailPage());
                    });
                }
            }
        }
        protected override async void OnStart()
        {
            base.OnStart();
            Preferences.Remove("ReturnFromPage");
            HandleNavigationFromNotification();
        }
        protected override void OnResume()
        {
            base.OnResume();
            HandleNavigationFromNotification();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }
    }
}
