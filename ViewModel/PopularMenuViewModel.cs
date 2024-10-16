using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Pages;
using FoodNinja.Pages.Popups;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;


namespace FoodNinja.ViewModel
{
    public partial class PopularMenuViewModel:ObservableObject
    {
        #region Fields
        private FirebaseManager firebaseManager = new FirebaseManager();
        public INavigation Navigation { get; }

        [ObservableProperty]
        private string currentViewModel = "PopularMenuPage";

        [ObservableProperty]
        private string cartValueText;

        [ObservableProperty]
        private int numberOfItemInCart = 0;

        [ObservableProperty]
        private ObservableCollection<PopularMenuModel> popularMenueList;
        public ICommand AddFoodToCartCommand { get; }

        public event Action FoodAddedToCart;
        #endregion

        #region Constructor
        public PopularMenuViewModel(INavigation navigation,FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            var restaurantService = DependencyService.Get<RestaurantService>();
            PopularMenueList = restaurantService.PopularMenu;
            AddFoodToCartCommand = new Command<PopularMenuModel>(async (selectedFood) => await OnAddingFoodToCartAsync(selectedFood));
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task Navigate()
        {
            Preferences.Set("SourcePage", "PopularMenuPage");
            await Navigation.PushAsync(new CartPage());
        }
        #endregion

        #region Method
        private async Task OnAddingFoodToCartAsync(PopularMenuModel? selectedFood)
        {
            if (selectedFood == null)
            {
                await Toast.Make("No food selected").Show();
                return;
            }

            string userId = Preferences.Get("LocalId", string.Empty);
            bool isDifferentRestaurantPresent = await firebaseManager.IsDifferentRestaurantPresentAsync(userId, selectedFood.RestaurantId);

            if (isDifferentRestaurantPresent)
            {
                #region Popup Code For Replace Cart Item
                var currentPage = Application.Current.MainPage;
                Func<Task> updateCartCallback = async () =>
                {
                    NumberOfItemInCart = 0;
                    NumberOfItemInCart++;
                    CartValueText = NumberOfItemInCart == 1 ? "item added" : "items added";
                    await Toast.Make("Food replaced and added to the cart").Show();
                };
                var replaceCartItemPopup = new ReplaceCartItemPopup(firebaseManager, updateCartCallback)
                {
                    SelectedMenuFood = selectedFood,
                    CurrentPage = CurrentViewModel,
                };
                await currentPage.ShowPopupAsync(replaceCartItemPopup);
                return;
                #endregion
            }
            else
            {
                #region For Normal Add Item In Cart If there are no item of different restaurant
                FoodAddedToCart?.Invoke();
                var previousNumberOfItemInCart = NumberOfItemInCart;
                CartValueText = NumberOfItemInCart == 0 ? "item added" : "items added";

                var random = new Random();
                int id = random.Next(1000, 9999);
                var addFoodToCart = new AddFoodToCartByPopularMenu
                {
                    FoodId = selectedFood.Id,
                    CartId = id,
                    FoodImage = selectedFood.FoodImage,
                    FoodName = selectedFood.DishName,
                    RestaurantName = selectedFood.RestaurantName,
                    RestaurantId = selectedFood.RestaurantId,
                    FoodPrice = selectedFood.Price,
                    FoodRating = selectedFood.FoodRating,
                };
                bool response = false;
                try
                {
                    response = await firebaseManager.AddFoodToCartFromPopularMenuAsync(addFoodToCart);
                    if (response)
                    {
                        NumberOfItemInCart++;
                        CartValueText = NumberOfItemInCart == 1 ? "item added" : "items added";
                    }
                    else
                    {
                        NumberOfItemInCart = previousNumberOfItemInCart;
                        CartValueText = NumberOfItemInCart == 1 ? "item added" : "items added";
                        await Toast.Make("Unable To Add Food in Cart").Show();
                    }
                }
                catch (Exception ex)
                {
                    NumberOfItemInCart = previousNumberOfItemInCart;
                    CartValueText = NumberOfItemInCart == 1 ? "item added" : "items added";
                    await Toast.Make("Unable To Add Food in Cart").Show();
                }
                #endregion
            }
        }
        #endregion
    }
}
