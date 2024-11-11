using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using FoodNinja.Model;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using Google.Apis.Auth.OAuth2;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using System.Collections.ObjectModel;
using FoodNinja.Pages.ProfileTabScreen;
using Microsoft.Extensions.Logging;

namespace FoodNinja.Pages.CartTabScreen;

public partial class ConfirmOrderPages : ContentPage
{
    private ObservableCollection<AddFoodToCart> cartDataList;
    private FirebaseManager firebaseManager;
    private string _deviceToken;

    public ConfirmOrderPages(ObservableCollection<AddFoodToCart> cartDataList, double subTotal, double totalPrice)
    {
        InitializeComponent();
        firebaseManager = new FirebaseManager();
        var permissionHelper = new NotificationPermissionHelper();
        this.cartDataList = cartDataList;
        this.BindingContext = new ConfirmOrderViewModel(firebaseManager, Navigation, cartDataList, subTotal, totalPrice,permissionHelper);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        if (BindingContext is ConfirmOrderViewModel viewModel)
        {
            string returnFromPage = Preferences.Get("ReturnFromPage", string.Empty);
            if(returnFromPage == "SuccessfullOrderPlacedPage")
            {
                viewModel.ReturnFromPage = returnFromPage;
                Preferences.Clear("ReturnFromPage");
                returnFromPage = string.Empty;
                cartDataList.Clear();
            }

            viewModel.IsLoading = true;
            await viewModel.HandleNotificationPermission();
            await viewModel.FetchUserDataAsync();
            await viewModel.FetchFirebaseToken();
            viewModel.IsLoading = false;

        }

        /*WeakReferenceMessenger.Default.Register<PushNotificationReceived>(this, (r, m) =>
        {
            string msg = m.Value;
            NavigateWhenCLickOnNotification();
        });*/
        if (Preferences.ContainsKey("DeviceToken"))
        {
            _deviceToken = Preferences.Get("DeviceToken", "");
        }
        ReadFireBaseAdminSdk();
       // NavigateWhenCLickOnNotification();
    }

    private async void NavigateWhenCLickOnNotification()
    {
        if (Preferences.ContainsKey("NavigationID"))
        {
            await Navigation.PushAsync(new PreviousOrderDetailPage());
        }
    }

    private async void ReadFireBaseAdminSdk()
    {
        var stream = await FileSystem.OpenAppPackageFileAsync("firebaseAdminSdk.json");
        var reader = new StreamReader(stream);
        var jsonContent = reader.ReadToEnd();
        if (FirebaseMessaging.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(jsonContent)
            });
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);

        if (BindingContext is ConfirmOrderViewModel viewModel)
        {
            viewModel.ReturnFromPage = string.Empty;
        }
        Preferences.Remove("ReturnFromPage");
        WeakReferenceMessenger.Default.Unregister<PushNotificationReceived>(this);
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
            bool isEmpty = string.IsNullOrEmpty(viewModel.HouseFlatBlockNo) &&
                           string.IsNullOrEmpty(viewModel.CityArea) &&
                           string.IsNullOrEmpty(viewModel.State) &&
                           string.IsNullOrEmpty(viewModel.Pincode);
            if(isEmpty)
            {
                await Toast.Make("Please fill in at least one field to edit your profile.").Show();
                return;
            }
            else
            {
                await viewModel.UpdateUserAddress();
                await simpleBottomSheet.CloseBottomSheet();
                await viewModel.FetchUserDataAsync();
                viewModel.HouseFlatBlockNo = string.Empty;
                viewModel.CityArea = string.Empty;
                viewModel.State = string.Empty;
                viewModel.Pincode = string.Empty;
            }
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
    private async void addAddressBtn_Clicked(object sender, EventArgs e)
    {
        var tasks = new List<Task>
        {
            addAddressBtn.ScaleTo(0.8, 100, Easing.CubicInOut),
            location.ScaleTo(0.8, 100, Easing.CubicInOut),
            address.ScaleTo(0.8, 100, Easing.CubicInOut),
        };

        await Task.WhenAll(tasks);

        tasks = new List<Task>
        {
            addAddressBtn.ScaleTo(1, 100, Easing.CubicInOut),
            location.ScaleTo(1, 100, Easing.CubicInOut),
            addAddressBtn.ScaleTo(1, 100, Easing.CubicInOut),
        };

        await simpleBottomSheet.OpenBottomSheet();
    }
}