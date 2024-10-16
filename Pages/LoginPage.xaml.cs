using FoodNinja.Pages.Signup_Screen;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace FoodNinja.Pages;

public partial class LoginPage : ContentPage
{
    public Label OnLabelClicked { get; }

    public LoginPage()
	{
		InitializeComponent();
        var firebaseManager = new FirebaseManager();
        BindingContext = new LoginViewModel(Navigation, firebaseManager);
        SetViewPosition();
        Loaded += (s, e) =>
        {
            FaseAndTranslate(popView, fadeLength: 1000, translateLength: 500);
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    private void FaseAndTranslate(Microsoft.Maui.Controls.VisualElement popView, uint fadeLength = 1000, uint translateLength = 500)
    {
        popView.FadeTo(opacity: 1, fadeLength, Easing.SinInOut);
        popView.TranslateTo(x: 0, y: 0, translateLength, Easing.SinInOut);
    }

    private void SetViewPosition()
    {
        popView.TranslationY = 300;
        popView.Opacity = 0.5;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
    }
    private async void EmailEntry_Completed(object sender, EventArgs e)
    {
        await emailBorder.ScaleTo(1.1, 100);
        await emailBorder.ScaleTo(1, 100);
        await Task.Delay(50);
        passwordEntry.Focus();
    }

    private async void passwordEntry_Completed(object sender, EventArgs e)
    {
        await passwordBorder.ScaleTo(1.1, 100);
        await passwordBorder.ScaleTo(1, 100);
    }

    private async void SignUp_Tapped(object sender, TappedEventArgs e)
    {
        await signUpLabl.ScaleTo(1.1, 100);
        await signUpLabl.ScaleTo(1, 100);
        await Navigation.PushAsync(new SignupPage());
    }
}