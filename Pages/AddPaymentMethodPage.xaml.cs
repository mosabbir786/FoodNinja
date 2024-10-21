using FoodNinja.Services;
using FoodNinja.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace FoodNinja.Pages;

public partial class AddPaymentMethodPage : ContentPage
{
    private FirebaseManager firebaseManager;

    public AddPaymentMethodPage()
	{
		InitializeComponent();
        firebaseManager = new FirebaseManager();
        this.BindingContext = new AddPaymentMethodViewModel(Navigation, firebaseManager);
        paypalBottomSheet.BottomSheetClosed += OnBottomSheetClosed;
        visaBottomSheet.BottomSheetClosed += OnVisaBottomSheetClosed;
        payoneerBottomSheet.BottomSheetClosed += OnPayoneerBottomSheetClosed;
    }

    

    private async void BackImageButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnBottomSheetClosed(object sender, EventArgs e)
    {
        if (BindingContext is AddPaymentMethodViewModel viewModel)
        {
            viewModel.PaypalBorderColor = Colors.Transparent;
            await Task.Delay(100);
            await Dispatcher.DispatchAsync(() =>
            {
                paypalEntry.Unfocus();
            });
        }
    }

    private async void OnVisaBottomSheetClosed(object sender, EventArgs e)
    {
        if (BindingContext is AddPaymentMethodViewModel viewModel)
        {
            viewModel.VisaBorderColor = Colors.Transparent;
            await Task.Delay(100);
            await Dispatcher.DispatchAsync(() =>
            {
                cardNumberEntry.Unfocus();
                cvvEntry.Unfocus();
            });
        }
    }

    private async void OnPayoneerBottomSheetClosed(object sender, EventArgs e)
    {
        if (BindingContext is AddPaymentMethodViewModel viewModel)
        {
            viewModel.PayoneerBorderColor = Colors.Transparent;
            await Task.Delay(100);
            await Dispatcher.DispatchAsync(() =>
            {
                payoneerEntry.Unfocus();
            });
        }
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
        var profilePageViewModel = (ProfilePageViewModel)BindingContext;
        profilePageViewModel.IsUpdated = true;

    }

    private void Paypal_Tapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is AddPaymentMethodViewModel viewModel)
        {
            viewModel.PaypalBorderColor = Color.FromArgb("#37d582");
            viewModel.SetCurrentBottomSheet(paypalBottomSheet);
            paypalBottomSheet.OpenBottomSheet();
        }
    }
    private void Visa_Tapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is AddPaymentMethodViewModel viewModel)
        {
            viewModel.VisaBorderColor = Color.FromArgb("#37d582");
            viewModel.SetCurrentBottomSheet(visaBottomSheet);
            visaBottomSheet.OpenBottomSheet();
        }
    }

    private void Payoneer_Tapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is AddPaymentMethodViewModel viewModel)
        {
            viewModel.PayoneerBorderColor = Color.FromArgb("#37d582");
            viewModel.SetCurrentBottomSheet(payoneerBottomSheet);
            payoneerBottomSheet.OpenBottomSheet();
        }

    }

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        expiryDatePicker.TextColor = Colors.White;
    }

    private void ADDPaypalButton_Clicked(object sender, EventArgs e)
    {
        paypalEntry.Unfocus();
        
    }
}