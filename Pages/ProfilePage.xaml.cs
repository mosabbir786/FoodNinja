using CommunityToolkit.Maui.Views;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using FoodNinja.Views;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace FoodNinja.Pages;

public partial class ProfilePage : ContentPage
{
	private FirebaseManager firebaseManager;
	public ProfilePage()
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
		this.BindingContext = new ProfilePageViewModel(Navigation, firebaseManager);
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        if (BindingContext is ProfilePageViewModel viewModel)
        {
           await viewModel.FetchUserDataAsync();
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
    }
    private async void AddPaymentButton_Clicked(object sender, EventArgs e)
    {
		await addMoreBtn.ScaleTo(1.1, 100);
		await addMoreBtn.ScaleTo(1, 100);
    }
    private async void EditAddress_Clicked(object sender, EventArgs e)
    {
        await editAddressBottomSheet.OpenBottomSheet();
    }

}