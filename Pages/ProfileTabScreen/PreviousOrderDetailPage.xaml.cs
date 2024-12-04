using CommunityToolkit.Maui.Alerts;
using FoodNinja.Model;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using System.Collections.ObjectModel;

namespace FoodNinja.Pages.ProfileTabScreen;

public partial class PreviousOrderDetailPage : ContentPage
{
	private readonly FirebaseManager firebaseManager;
	public PreviousOrderDetailPage()
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
		this.BindingContext = new OrderDetailsViewModel(Navigation,firebaseManager);
	}

    private void firstSearchbar_Unfocused(object sender, FocusEventArgs e)
    {
        firstSearchbar.Unfocus();
    }

    private void firstSearchbar_SearchButtonPressed(object sender, EventArgs e)
    {
        PerformSearch(firstSearchbar.Text);
    }

    private async void PerformSearch(string searchText)
    {
        var container = BindingContext as OrderDetailsViewModel;
        if (container != null && container.AllOrderList != null)
        {
            var originalList = container.AllOrderList.ToList();
            if(string.IsNullOrEmpty(searchText))
            {
                var filteredPreviousOrderList = originalList
                   .Where(s => !string.IsNullOrEmpty(s.RestaurantName) &&
                   s.RestaurantName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                   .ToList();
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(async() =>
                {
                    await Toast.Make("No previous order found with this restaurant").Show();

                });
                previousOrderCollection.ItemsSource = container.AllOrderList;
                firstSearchbar.Text = string.Empty;
                firstSearchbar.Unfocus();
            }
        }
    }

    private async void firstSearchbar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var container = BindingContext as OrderDetailsViewModel;
        if (container != null && container.AllOrderList != null)
        {
            var originalList = container.AllOrderList.ToList();
            if(string.IsNullOrEmpty(e.NewTextValue))
            {
                var filteredPreviousOrderList = originalList
                    .Where(s => !string.IsNullOrEmpty(s.RestaurantName) &&
                    s.RestaurantName.StartsWith(e.NewTextValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                container.AllOrderList = new ObservableCollection<OrderPlacedModel>(filteredPreviousOrderList);
                if(filteredPreviousOrderList.Any())
                {
                    container.AllOrderList = new ObservableCollection<OrderPlacedModel>(filteredPreviousOrderList);
                }
                else
                {
                    await Toast.Make("No previous order found with this restaurant").Show();
                    container.AllOrderList = new ObservableCollection<OrderPlacedModel>(originalList);
                }
            }
            else
            {
                await Toast.Make("No previous order found with this restaurant").Show();
                container.AllOrderList = new ObservableCollection<OrderPlacedModel>(originalList);
            }
        }
        else
        {
            previousOrderCollection.ItemsSource = container.AllOrderList;
        }
    }
}