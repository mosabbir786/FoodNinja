using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using FoodNinja.Model;
using FoodNinja.Pages.Popups;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using System.Collections.ObjectModel;

namespace FoodNinja.Pages;

public partial class HomePage : ContentPage
{
    private FirebaseManager firebaseManager;
	public HomePage()
	{
		InitializeComponent();
        firebaseManager = new FirebaseManager();
        this.BindingContext = new HomePageViewModel(Navigation, firebaseManager);   
	}

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await this.ShowPopupAsync(new ShowExitConfirmationPopup());
        });
        return true;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        var viewModel = BindingContext as HomePageViewModel;
        if (viewModel != null)
        {
            await Task.Delay(1);
            viewModel.CurrentPage = TypeOfPage.HomePage;

            await viewModel.LoadUnreadNotificationCount();
            WeakReferenceMessenger.Default.Register<NotificationBadgeMessage>(this, (r, message) =>
            {
               viewModel.UnreadNotificationCount = message.Value;
            });
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
        WeakReferenceMessenger.Default.Unregister<NotificationBadgeMessage>(this);
    }

    #region Clicked Event

    private void firstSearchbar_Focused(object sender, FocusEventArgs e)
    {
        menuList.Margin = new Thickness(0, 0, 0, 90);
    }
    private void firstSearchbar_Unfocused(object sender, FocusEventArgs e)
    {
        menuList.Margin = new Thickness(0, 0, 0, 0);
    }
    private void firstSearchbar_SearchButtonPressed(object sender, EventArgs e)
    {
        PerformSearch(firstSearchbar.Text);
    }
    private async void firstSearchbar_TextChanged(object sender, TextChangedEventArgs e)
    {
        await Task.Delay(1);
        var container = BindingContext as HomePageViewModel;
        if (container != null && container.RestaurantList != null)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                var filteredRestaurantList = container.RestaurantList.Where(s => !string.IsNullOrEmpty(s.RestaurantName) && s.RestaurantName.StartsWith(e.NewTextValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                container.RestaurantList = new ObservableCollection<NearestRestaurantModel>(filteredRestaurantList);
                if (!filteredRestaurantList.Any())
                {
                    container.RestaurantList = new ObservableCollection<NearestRestaurantModel>(container.Restaurant.Take(3));
                }
            }
            else
            {
                container.RestaurantList = new ObservableCollection<NearestRestaurantModel>(container.Restaurant.Take(3));
                if (string.IsNullOrEmpty(e.NewTextValue))
                {
                    firstSearchbar?.Unfocus();
                }
            }
        }
    }
    private async void ViewMoreRestaurant_Tapped(object sender, TappedEventArgs e)
    {
        vMore1Border.BackgroundColor = Colors.Gray;
        for (double i = 1; i >= 0; i -= 0.1)
        {
            vMore1Border.Opacity = i;
            await Task.Delay(20);
        }
        await Task.Delay(50);
        vMore1Border.BackgroundColor = Colors.Transparent;
        for (double i = 0; i <= 1; i += 0.1)
        {
            vMore1Border.Opacity = i;
            await Task.Delay(20);
        }
    }
    private async void RestaurantFrame_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            await ApplyTapAnimation(border);
            await Task.Delay(50);
            await ResetBorderAnimation(border);
        }
    }
    private async void ViewMoreMenu_Tapped(object sender, TappedEventArgs e)
    {
        vMore2Border.BackgroundColor = Colors.Gray;
        for (double i = 1; i >= 0; i -= 0.1)
        {
            vMore2Border.Opacity = i;
            await Task.Delay(20);
        }
        await Task.Delay(50);
        vMore2Border.BackgroundColor = Colors.Transparent;
        for (double i = 0; i <= 1; i += 0.1)
        {
            vMore2Border.Opacity = i;
            await Task.Delay(20);
        }
    }
    #endregion

    #region Methods
    private async Task ApplyTapAnimation(Border border)
    {
        border.BackgroundColor = Colors.Gray;
        for (double i = 1; i >= 0; i -= 0.1)
        {
            border.Opacity = i;
            await Task.Delay(20);
        }
    }
    private async Task ResetBorderAnimation(Border border)
    {
        border.BackgroundColor = Color.FromArgb("#252525");
        for (double i = 0; i <= 1; i += 0.1)
        {
            border.Opacity = i;
            await Task.Delay(20);
        }
    }
    private async void PerformSearch(string searchText)
    {
        firstSearchbar.Text = string.Empty;
        var container = BindingContext as HomePageViewModel;
        if (container != null && container.RestaurantList != null)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredRestaurantList = container.RestaurantList.Where(s => !string.IsNullOrEmpty(s.RestaurantName)
                    && s.RestaurantName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                container.RestaurantList = new ObservableCollection<NearestRestaurantModel>(filteredRestaurantList);
                if (!filteredRestaurantList.Any())
                {
                    await Toast.Make("No restaurants found with this name").Show();
                    container.RestaurantList = new ObservableCollection<NearestRestaurantModel>(container.Restaurant.Take(3));
                }
                firstSearchbar.Unfocus();
            }
            else
            {
                container.RestaurantList = new ObservableCollection<NearestRestaurantModel>(container.Restaurant.Take(3));
                firstSearchbar?.Unfocus();
            }
        }
    }

    #endregion
}