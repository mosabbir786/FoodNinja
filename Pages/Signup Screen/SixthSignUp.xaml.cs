using FoodNinja.Services;
using FoodNinja.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace FoodNinja.Pages.Signup_Screen;

public partial class SixthSignUp : ContentPage
{
	public SixthSignUp(string email, string firstName, string lastName, string mobileNumber, string finalImage)
	{
		InitializeComponent();
        var firebaseManager = new FirebaseManager();
        this.BindingContext = new SixthSignUpViewModel(Navigation, email, firstName, lastName, mobileNumber, finalImage, firebaseManager);
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
    private async void LocationBorder_Tapped(object sender, TappedEventArgs e)
    {
        await locationBorder.ScaleTo(1.1, 100);
        await locationBorder.ScaleTo(1, 100);
    }

    private async void houseEntry_Completed(object sender, EventArgs e)
    {
        await houseBorder.ScaleTo(1.1, 100);
        await houseBorder.ScaleTo(1, 100);
        await Task.Delay(50);
        cityEntry.Focus();
    }

    private async void cityEntry_Completed(object sender, EventArgs e)
    {
        await cityBorder.ScaleTo(1.1, 100);
        await cityBorder.ScaleTo(1, 100);
        await Task.Delay(50);
        stateEntry.Focus();
    }

    private async void stateEntry_Completed(object sender, EventArgs e)
    {
        await stateBorder.ScaleTo(1.1, 100);
        await stateBorder.ScaleTo(1, 100);
        await Task.Delay(50);
        pincodeEntry.Focus();
    }

    private async void pincodeEntry_Completed(object sender, EventArgs e)
    {
        await pincodeBorder.ScaleTo(1.1, 100);
        await pincodeBorder.ScaleTo(1, 100);
    }
}