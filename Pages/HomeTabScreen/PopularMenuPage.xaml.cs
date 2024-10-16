using FoodNinja.Model;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using System.Collections.ObjectModel;

namespace FoodNinja.Pages.HomeTabScreen;

public partial class PopularMenuPage : ContentPage
{
	private FirebaseManager firebaseManager;
    private bool isAnimating = false;
    public PopularMenuPage()
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
        this.BindingContext = new PopularMenuViewModel(Navigation,firebaseManager);
        if (BindingContext is PopularMenuViewModel viewModel)
        {
            viewModel.FoodAddedToCart += OnFoodAddedToCart;
        }
    }

    private async void OnFoodAddedToCart()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            cartNotificationBorder.IsVisible = true;
            FadeAndTranslate(cartNotificationBorder);
            await Task.Delay(100);
        }
    }

    private async void FadeAndTranslate(Microsoft.Maui.Controls.VisualElement popview, uint fadeLength = 1000, uint translateLength = 500)
    {
        SetViewPosition(popview);
        await popview.FadeTo(1, fadeLength, Easing.SinInOut);
        await popview.TranslateTo(0, 0, translateLength, Easing.SinInOut);
    }
    private void SetViewPosition(VisualElement popview)
    {
        popview.TranslationY = 300;
        popview.Opacity = 0.5;
    }

    private void searchbar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var container = BindingContext as PopularMenuViewModel;
        if (container != null && container.PopularMenueList != null)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                var filteredList = container.PopularMenueList
                    .Where(s => !string.IsNullOrEmpty(s.DishName) && s.DishName.StartsWith(e.NewTextValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                menuList.ItemsSource = filteredList;
                loader.IsAnimationEnabled = filteredList.Count == 0;
                loader.IsVisible = filteredList.Count == 0;
                popularMenuListFooter.IsVisible = filteredList.Count > 0;
            }
            else
            {
                Dispatcher.Dispatch(() => searchbar.Unfocus());
                menuList.ItemsSource = container.PopularMenueList;
                loader.IsVisible = false;
                popularMenuListFooter.IsVisible = true;
            }
        }
    }

    private void searchbar_SearchButtonPressed(object sender, EventArgs e)
    {
        Performsearch(searchbar.Text);
    }
    private void Performsearch(string searchText)
    {
        var container = BindingContext as PopularMenuViewModel;
        if (container != null && container.PopularMenueList != null)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredList = container.PopularMenueList
                    .Where(s => !string.IsNullOrEmpty(s.DishName) && s.DishName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                menuList.ItemsSource = filteredList;
                loader.IsVisible = filteredList.Count == 0;
                loader.IsAnimationEnabled = filteredList.Count == 0;
                popularMenuListFooter.IsVisible = filteredList.Count > 0;
                searchbar.Unfocus();
            }
            else
            {
                menuList.ItemsSource = container.PopularMenueList;
                popularMenuListFooter.IsVisible = true;
                loader.IsAnimationEnabled = false;
                loader.IsVisible = false;
                searchbar.Unfocus();
            }

        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        isAnimating = false;
        if (BindingContext is PopularMenuViewModel viewModel)
        {
            viewModel.NumberOfItemInCart = 0;
            viewModel.CartValueText = string.Empty;
        }
    }
}