using FoodNinja.Model;
using FoodNinja.ViewModel;
using System.Collections.ObjectModel;

namespace FoodNinja.Pages.HomeTabScreen;

public partial class RestaurantPage : ContentPage
{
	public RestaurantPage()
	{
		InitializeComponent();
        this.BindingContext = new RestaurantPageViewModel(Navigation);
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var container = BindingContext as RestaurantPageViewModel;
        if (container != null && container.RestaurantList != null)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                var filteredList = container.RestaurantList
                    .Where(s => !string.IsNullOrEmpty(s.RestaurantName) && s.RestaurantName.StartsWith(e.NewTextValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                restroCollection.ItemsSource = filteredList;
                loader.IsAnimationEnabled = filteredList.Count == 0;
                loader.IsVisible = filteredList.Count == 0;

            }
            else
            {
                Dispatcher.Dispatch(() => searchbar.Unfocus());
                restroCollection.ItemsSource = container.RestaurantList;
                loader.IsAnimationEnabled = false;
                loader.IsVisible = false;
            }
        }
    }
    private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        Performsearch(searchbar.Text);
    }
    private void Performsearch(string searchText)
    {
        var container = BindingContext as RestaurantPageViewModel;
        if (container != null && container.RestaurantList != null)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredList = container.RestaurantList
                    .Where(s => !string.IsNullOrEmpty(s.RestaurantName) && s.RestaurantName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                restroCollection.ItemsSource = filteredList;
                loader.IsVisible = filteredList.Count == 0;
                loader.IsAnimationEnabled = filteredList.Count == 0;
                searchbar.Unfocus();
            }
            else
            {
                restroCollection.ItemsSource = container.RestaurantList;
                loader.IsAnimationEnabled = false;
                loader.IsVisible = false;
                searchbar.Unfocus();
            }

        }
    }
    private async void RestaurantFrame_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            await ApplyTapAnimation(border);
            await Task.Delay(200);
            await ResetBorderAnimation(border);
        }
    }
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
}