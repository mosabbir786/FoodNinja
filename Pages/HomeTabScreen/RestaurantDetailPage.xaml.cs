using FoodNinja.Model;
using FoodNinja.Services;
using FoodNinja.ViewModel;

namespace FoodNinja.Pages.HomeTabScreen;

public partial class RestaurantDetailPage : ContentPage
{
	private FirebaseManager firebaseManager;
	private bool isAnimating = false;

    public RestaurantDetailPage(/*NearestRestaurantModel selectedRestaurant*/)
	{
		InitializeComponent();
		firebaseManager = new FirebaseManager();
		this.BindingContext = new RestaurantDetailViewModel(Navigation,firebaseManager);
        if (BindingContext is RestaurantDetailViewModel viewModel)
        {
            viewModel.FoodAddedToCart += OnFoodAddedToCart;
        }
    }

    private async void OnFoodAddedToCart()
    {
        if (!isAnimating)
        {
            isAnimating = true;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RestaurantDetailViewModel viewModel)
        {
            OnPropertyChanged(nameof(viewModel.NumberOfItemInCart));
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        isAnimating = false;
        if (BindingContext is RestaurantDetailViewModel viewModel)
        {
            viewModel.NumberOfItemInCart = 0;
            viewModel.CartValueText = string.Empty;
            cartNotificationBorder.TranslationY = 100;
            cartNotificationBorder.Opacity = 0;
        }
    }
}