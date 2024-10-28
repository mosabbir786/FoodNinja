﻿using FoodNinja.Pages;
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

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        //For UI Thread Exceptions
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if(exception != null)
            {
                SentrySdk.CaptureException(exception);
            }
        }

        //For Non-UI Thread Exceptions
        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            SentrySdk.CaptureException(e.Exception);
        }

        protected override void OnStart()
        {
            base.OnStart();
            Preferences.Remove("ReturnFromPage");
        }
        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }
    }
}
