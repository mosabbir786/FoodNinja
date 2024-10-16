using FoodNinja.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace FoodNinja.Pages.Signup_Screen;

public partial class SecondSignUp : ContentPage
{
	public SecondSignUp(string email)
	{
		InitializeComponent();
        BindingContext = new SecondSignupViewModel(Navigation, email);
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
    }
    private async void FirstNameEntry_Completed(object sender, EventArgs e)
    {
        await firstNameBorder.ScaleTo(1.1, 100);
        await firstNameBorder.ScaleTo(1, 100);
        await Task.Delay(50);
        lastNameEntry.Focus();
    }

    private async void LastNameEntry_Completed(object sender, EventArgs e)
    {
        await lastNameBorder.ScaleTo(1.1, 100);
        await lastNameBorder.ScaleTo(1, 100);
        await Task.Delay(50);
        mobileEntry.Focus();
    }

    private async void MobileEntry_Completed(object sender, EventArgs e)
    {
        await mobileBorder.ScaleTo(1.1, 100);
        await mobileBorder.ScaleTo(1, 100);
        mobileEntry.Unfocus();
    }
}