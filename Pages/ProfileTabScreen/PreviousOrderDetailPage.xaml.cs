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

    private void firstSearchbar_Focused(object sender, FocusEventArgs e)
    {

    }

    private void firstSearchbar_Unfocused(object sender, FocusEventArgs e)
    {

    }

    private void firstSearchbar_SearchButtonPressed(object sender, EventArgs e)
    {

    }

    private async void firstSearchbar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var container = BindingContext as OrderDetailsViewModel;
        if (container != null && container.AllOrderList != null)
        {
            if(!string.IsNullOrEmpty(e.NewTextValue))
            {
                var filteredPrevoiusOrderList = container.AllOrderList.Where(s => !string.IsNullOrEmpty(s.RestaurantName) && s.RestaurantName.StartsWith(e.NewTextValue, StringComparison.OrdinalIgnoreCase)).ToList();
                container.AllOrderList = new ObservableCollection<OrderPlacedModel>(filteredPrevoiusOrderList); 
            }
            else
            {
                await Toast.Make("No Oorder Found").Show();
                if (string.IsNullOrEmpty(e.NewTextValue))
                {
                    firstSearchbar?.Unfocus();
                    
                }
            }
        }
    }
}