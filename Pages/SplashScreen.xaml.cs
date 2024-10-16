using FoodNinja.Pages.Onboarding_Screen;

namespace FoodNinja.Pages;

public partial class SplashScreen : ContentPage
{
	public SplashScreen()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(2000).ContinueWith(t =>
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Application.Current.MainPage = new NavigationPage(new FirstOnboarding());
            });
        });
    }
}