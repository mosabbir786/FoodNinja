using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using FoodNinja.Pages.Popups;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using FoodNinja.Views;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using System.Diagnostics;

namespace FoodNinja.Pages;

public partial class ProfilePage : ContentPage
{
	private FirebaseManager firebaseManager;
	public ProfilePage()
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
		this.BindingContext = new ProfilePageViewModel(Navigation, firebaseManager);
        if(BindingContext is  ProfilePageViewModel viewModel)
        {
            viewModel.SetCurrentBottomSheet(editAddressBottomSheet);
        }
        editAddressBottomSheet.BottomSheetClosed += OnBottomSheetClosed;
	}

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await this.ShowPopupAsync(new ShowExitConfirmationPopup());
        });
        return true;
    }
    private async void OnBottomSheetClosed(object? sender, EventArgs e)
    {
       // await Toast.Make("Addreess Updated").Show();
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
        if(addMoreBtn != null)
        {
            await addMoreBtn.ScaleTo(1.1, 100);
            await addMoreBtn.ScaleTo(1, 100);
        }
    }
    private async void EditAddress_Clicked(object sender, EventArgs e)
    {
        try
        {
            await editAddressBottomSheet.OpenBottomSheet();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening bottom sheet: {ex.Message}");
        }
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
        pincodeEntry.Unfocus();
    }
}