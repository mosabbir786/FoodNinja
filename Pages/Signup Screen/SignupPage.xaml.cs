using FoodNinja.Services;
using FoodNinja.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace FoodNinja.Pages.Signup_Screen;

public partial class SignupPage : ContentPage
{
	public SignupPage()
	{
		InitializeComponent();
        var firebaseManager = new FirebaseManager();
        BindingContext = new SignupViewModel(Navigation, firebaseManager);
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
    private async void UserNameEntry_Completed(object sender, EventArgs e)
    {
        await userNameBorder.ScaleTo(1.1, 100);
        await userNameBorder.ScaleTo(1, 100);
        await Task.Delay(50);

        emailEntry.Focus();
    }

    private async void EmailEntry_Completed(object sender, EventArgs e)
    {
        await emailBorder.ScaleTo(1.1, 100);
        await emailBorder.ScaleTo(1, 100);
        await Task.Delay(50);

        passwordEntry.Focus();
    }

    private async void PasswordEntry_Completed(object sender, EventArgs e)
    {
        await passwordBorder.ScaleTo(1.1, 100);
        await passwordBorder.ScaleTo(1, 100);
    }

    private void CheckBox1_Tapped(object sender, TappedEventArgs e)
    {
        check1.IsVisible = !check1.IsVisible;
        int checkBox1Status = GetCheckBox1Status();
    }

    private int GetCheckBox1Status()
    {
        return check1.IsVisible ? 1 : 0;
    }
    private void CheckBox2_Tapped(object sender, TappedEventArgs e)
    {
        check2.IsVisible = !check2.IsVisible;
        int checkbox2Status = GetCheckBox2Status();
    }

    private int GetCheckBox2Status()
    {
        return check2.IsVisible ? 1 : 0;
    }
    private async void AlreadyAccount_Tapped(object sender, TappedEventArgs e)
    {
        await alreadyAccountLbl.ScaleTo(1.1, 100);
        await alreadyAccountLbl.ScaleTo(1, 100);
    }
}