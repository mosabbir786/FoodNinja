using AlohaKit.Animations;
using CommunityToolkit.Maui.Views;
using FoodNinja.Services;
using FoodNinja.ViewModel;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace FoodNinja.Pages;

public partial class CartPage : ContentPage
{
    private Border _currentItemStack;
    private Border _currentDeleteIconStack;
    double _startX;

    private bool _isAnimating;
    private FirebaseManager firebaseManager;
    public CartPage()
	{
		InitializeComponent();
        firebaseManager = new FirebaseManager();
        this.BindingContext = new CartViewModel(Navigation, firebaseManager);
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (_isAnimating || sender is not Grid grid) return; 
        var itemStack = grid.FindByName<Border>("itemStack");
        var deleteIconStack = grid.FindByName<Border>("deleteIconStack");

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _startX = itemStack.TranslationX;
                break;

            case GestureStatus.Running:
                double newX = _startX + e.TotalX;
                if (newX <= 0 && newX >= -deleteIconStack.Width)
                {
                    itemStack.TranslationX = newX;
                    double opacity = Math.Max(0, Math.Min(1, Math.Abs(newX) / deleteIconStack.Width));
                    deleteIconStack.Opacity = opacity;
                }
                break;

            case GestureStatus.Completed:
                if (Math.Abs(itemStack.TranslationX) > deleteIconStack.Width / 2)
                {
                    ShowDeleteIcon(itemStack, deleteIconStack);
                }
                else
                {
                    ResetSwipe(itemStack, deleteIconStack);
                }
                break;
        }
    }
    private async void ShowDeleteIcon(Border itemStack, Border deleteIconStack)
    {
        if (_isAnimating) return;
        _isAnimating = true;

        itemStack.Margin = new Thickness(0, 0, -30, 0);
        await itemStack.TranslateTo(-deleteIconStack.Width, 0, 250u, Easing.CubicInOut); // Use '250u' for uint
        itemStack.TranslationX = -deleteIconStack.Width;

        _isAnimating = false;
    }
    private async void ResetSwipe(Border itemStack, Border deleteIconStack)
    {

        if (_isAnimating) return; 
        _isAnimating = true; 

        itemStack.Margin = new Thickness(0);
        await itemStack.TranslateTo(0, 0, 250u, Easing.CubicInOut);
        itemStack.TranslationX = 0;
        deleteIconStack.Opacity = 0;

        _isAnimating = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        priceCalculationBorder.Margin = new Thickness(0, 10, 0, 70);
        backBtn.IsVisible = false;
        var sourcePage = Preferences.Get("SourcePage", string.Empty);
        if (sourcePage == "RestaurantDetailPage")
        {
            backBtn.IsVisible = true;
            priceCalculationBorder.Margin = new Thickness(0, 10, 0, 0);
        }
        else if (sourcePage == "PopularMenuPage")
        {
            backBtn.IsVisible = true;
            priceCalculationBorder.Margin = new Thickness(0, 10, 0, 0);
        }
        else if (sourcePage != "RestaurantDetailPage" && sourcePage != "PopularMenuPage")
        {
            backBtn.IsVisible = false;
            priceCalculationBorder.Margin = new Thickness(0, 10, 0, 70);
        }
        if (BindingContext is CartViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            viewModel.IsLoading = true;
            await viewModel.GetCartdata();
            viewModel.IsLoading = false;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CartViewModel.IsLoading))
        {
            var viewModel = sender as CartViewModel;
            if (viewModel != null && !viewModel.IsLoading)
            {
                priceCalculationBorder.Opacity = 0;
                priceCalculationBorder.FadeTo(1, 300, Easing.SpringIn);
            }
        }
    }
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        if (_currentItemStack != null && _currentDeleteIconStack != null)
        {
            _currentItemStack.TranslationX = 0;
            _currentDeleteIconStack.Opacity = 0;
            _currentItemStack.Margin = new Thickness(0);
        }
        if (BindingContext is CartViewModel viewModel)
        {
            await viewModel.GetCartdata();
        }
        Preferences.Remove("SourcePage");
    }
    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            //await this.ShowPopupAsync(new ShowExitConfirmationPopup());
        });
        return true;
    }
}