using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FoodNinja.Model;
using FoodNinja.Pages;
using FoodNinja.Pages.Popups;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoodNinja.ViewModel
{
    public partial class RestaurantDetailViewModel: ObservableObject
    {
        #region Fields
        [ObservableProperty]
        private string currentViewModel = "RestaurantDetailPage";

        private FirebaseManager firebaseManager = new FirebaseManager();
        public INavigation Navigation { get; }

        [ObservableProperty]
        private int numberOfItemInCart = 0;

        [ObservableProperty]
        private string cartValueText;

        [ObservableProperty]
        private NearestRestaurantModel selectedRestaurant;

        [ObservableProperty]
        private string shortDescription;

        [ObservableProperty]
        private bool isFullDescriptionVisible;

        public string FullDescription => SelectedRestaurant.Description;
        public string DisplayedDescription => IsFullDescriptionVisible ? FullDescription : ShortDescription;
        public string MoreText => IsFullDescriptionVisible ? "Show Less" : "Show More";
        public ICommand AddFoodToCartCommand { get; }

        public event Action FoodAddedToCart;

        #endregion

        #region Constructor
        public RestaurantDetailViewModel(INavigation navigation,FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            var restaurantService = DependencyService.Get<RestaurantService>();
            SelectedRestaurant = restaurantService.SelectedRestaurant;
            AddFoodToCartCommand = new Command<FoodItemModel>(async (selectedFood) => await OnAddingFoodToCartAsync(selectedFood));

        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task GotoCart()
        {
            WeakReferenceMessenger.Default.Send("RestaurantDetailPage");
            Preferences.Set("SourcePage", "RestaurantDetailPage");
            await Navigation.PushAsync(new CartPage());
        }
        #endregion

        #region Methods
        private async Task OnAddingFoodToCartAsync(FoodItemModel? selectedFood)
        {
            if (selectedFood == null)
            {
                await Toast.Make("No food selected").Show();
                return;
            }
            string userId = Preferences.Get("LocalId", string.Empty);
            bool isDifferentRestaurantPresent = await firebaseManager.IsDifferentRestaurantPresentAsync(userId, SelectedRestaurant.Id);

            if (isDifferentRestaurantPresent)
            {
                #region Popup Code for replace cart item 
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
                    SelectedRestaurant = SelectedRestaurant,
                    SelectedFood = selectedFood,
                    CurrentPage = CurrentViewModel
                };
                await currentPage.ShowPopupAsync(replaceCartItemPopup);
                if (replaceCartItemPopup.IsItemReplaced())
                {
                    OnPropertyChanged(nameof(NumberOfItemInCart));
                    OnPropertyChanged(nameof(CartValueText));
                    FoodAddedToCart?.Invoke();
                }
                return;
                #endregion

            }
            else
            {
                #region For Normal Add Item In Cart If there are no item of different restaurant
                FoodAddedToCart?.Invoke();
                var previousNumberOfItemCart = NumberOfItemInCart;
                CartValueText = NumberOfItemInCart == 0 ? "item added" : "items added";

                var random = new Random();
                int id = random.Next(1000, 9999);
                var addFoodToCart = new AddFoodToCart
                {
                    CartId = id,
                    RestaurantId = SelectedRestaurant.Id,
                    RestaurantName = SelectedRestaurant.RestaurantName,
                    RestaurantDescription = SelectedRestaurant.Description,
                    RestaurantAddress = SelectedRestaurant.RestroAddress,
                    RestaurantRating = SelectedRestaurant.RestaurantRating,
                    RestaurantDistance = SelectedRestaurant.RestrauntDistance,
                    FoodId = selectedFood.FoodId,
                    FoodImage = selectedFood.FoodImage,
                    FoodDescription = selectedFood.FoodDescription,
                    FoodPrice = selectedFood.FoodPrice,
                    FoodRating = selectedFood.FoodRating,
                    FoodName = selectedFood.FoodName,
                    RestaurantLat = selectedFood.RestaurantLat,
                    RestaurantLong = selectedFood.RestaurantLong
                };
                bool response = false;
                try
                {
                    response = await firebaseManager.AddFoodToCartAsync(addFoodToCart);
                    if (response)
                    {
                        NumberOfItemInCart++;
                        CartValueText = NumberOfItemInCart == 1 ? "item added" : "items added";
                    }
                    else
                    {
                        NumberOfItemInCart = previousNumberOfItemCart;
                        CartValueText = NumberOfItemInCart == 1 ? "item added" : "items added";
                        await Toast.Make("Unable To Add Food in Cart").Show();
                    }
                }
                catch (Exception ex)
                {
                    NumberOfItemInCart = previousNumberOfItemCart;
                    CartValueText = NumberOfItemInCart == 1 ? "item added" : "items added";
                    await Toast.Make("Unable To Add Food in Cart").Show();
                }
                #endregion
            }
        }
        private string GetShortDescription(string fullDescription)
        {
            return fullDescription.Length > 50 ? fullDescription.Substring(0, 130) + "..." : fullDescription;
        }
        #endregion
    }

    public partial class NavigationMessage : ObservableObject
    {
        [ObservableProperty]
        private string sourcePage;
    }
}
