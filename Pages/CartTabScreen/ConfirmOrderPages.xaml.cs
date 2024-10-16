using FoodNinja.Model;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using System.Collections.ObjectModel;

namespace FoodNinja.Pages.CartTabScreen;

public partial class ConfirmOrderPages : ContentPage
{
    private ObservableCollection<AddFoodToCart> cartDataList;
    string ReturnFromPage;//These changese are done on 7-10-2024
    private FirebaseManager firebaseManager;
    public ConfirmOrderPages(ObservableCollection<AddFoodToCart> cartDataList, double subTotal, double totalPrice)
    {
        InitializeComponent();
        firebaseManager = new FirebaseManager();
        this.cartDataList = cartDataList;     //These changese are done on 7-10-2024
        this.BindingContext = new ConfirmOrderViewModel(firebaseManager, Navigation, cartDataList, subTotal, totalPrice);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        if (BindingContext is ConfirmOrderViewModel viewModel)
        {
            ReturnFromPage = Preferences.Get("ReturnFromPage", string.Empty); //These changese are done on 7-10-2024 Line 28 to 35
            if (ReturnFromPage == "SuccessfullOrderPlacedPage")
            {
                viewModel.ReturnFromPage = ReturnFromPage;
                Preferences.Clear("ReturnFromPage");
                ReturnFromPage = string.Empty;
                cartDataList.Clear();
            }

            viewModel.IsLoading = true;
            await viewModel.FetchUserDataAsync();
            viewModel.IsLoading = false;
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);

        //These changese are done on 7-10-2024 from Line 47 to 53
        if (BindingContext is ConfirmOrderViewModel viewModel)
        {
            viewModel.ReturnFromPage = string.Empty;
        }
        ReturnFromPage = string.Empty;
        Preferences.Remove("ReturnFromPage");
    }
    private async void EditAddressLbl_Tapped(object sender, TappedEventArgs e)
    {
        await ApplyTapAnimation();
        await Task.Delay(100);
        await ResetBorderAnimation();
        await simpleBottomSheet.OpenBottomSheet();
    }
    private async Task ApplyTapAnimation()
    {
        editAddressBg.BackgroundColor = Colors.Gray;
        for (double i = 1; i >= 0; i -= 0.1)
        {
            editAddressBg.Opacity = i;
            await Task.Delay(20);
        }
    }
    private async Task ResetBorderAnimation()
    {
        editAddressBg.BackgroundColor = Color.FromArgb("#252525");
        for (double i = 0; i <= 1; i += 0.1)
        {
            editAddressBg.Opacity = i;
            await Task.Delay(20);
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
    }
    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await simpleBottomSheet.CloseBottomSheet();
    }
    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            pincodeEntry.Unfocus();
        });
        if (BindingContext is ConfirmOrderViewModel viewModel)
        {
            await viewModel.UpdateUserAddress();
            await simpleBottomSheet.CloseBottomSheet();
        }
    }

    private async void AddMorePayment_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            await ApplyTapFadeAnimation(border);
            await Task.Delay(100);
            await ResetBorderFadeAnimation(border);
        }
    }
    private async Task ApplyTapFadeAnimation(Border border)
    {
        border.BackgroundColor = Colors.Gray;
        for (double i = 1; i >= 0; i -= 0.1)
        {
            border.Opacity = i;
            await Task.Delay(20);
        }
    }
    private async Task ResetBorderFadeAnimation(Border border)
    {
        border.BackgroundColor = Color.FromArgb("#252525");
        for (double i = 0; i <= 1; i += 0.1)
        {
            border.Opacity = i;
            await Task.Delay(20);
        }
    }
}