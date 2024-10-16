using FoodNinja.Model;
using System.Collections.ObjectModel;

namespace FoodNinja.Pages.HomeTabScreen;

public partial class RestaurantPage : ContentPage
{
	public RestaurantPage(ObservableCollection<NearestRestaurantModel> restaurantList)
	{
		InitializeComponent();
	}
}