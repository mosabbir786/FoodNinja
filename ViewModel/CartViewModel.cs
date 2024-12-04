using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Pages.CartTabScreen;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class CartViewModel:ObservableObject
    {
        #region Fields
        private FirebaseManager firebaseManager = new FirebaseManager();
        public INavigation Navigation { get; }

        [ObservableProperty]
        private bool isPriceStackVisiblity;

        [ObservableProperty]
        private double deliveryCharge = 2;

        [ObservableProperty]
        private double subTotal;
        [ObservableProperty]
        private double totalPrice;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool noItemInCartAnimation;

        [ObservableProperty]
        private ObservableCollection<AddFoodToCart> cartDataList = new ObservableCollection<AddFoodToCart>();

        [ObservableProperty]
        private string pageType;
        public IAsyncRelayCommand DeleteCartItemCommand { get; }
        #endregion

        #region Constructor
        public CartViewModel(INavigation navigation, FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            DeleteCartItemCommand = new AsyncRelayCommand<AddFoodToCart>(async (selectedCartItem) => await OnCartItemDelete(selectedCartItem));
        }
        #endregion

        #region Command

        [RelayCommand]
        private async Task Increment(AddFoodToCart cartItem)
        {
            var item = CartDataList.FirstOrDefault(x => x.FoodId == cartItem.FoodId);
            if (item != null)
            {
                item.Quantity++;
                OnPropertyChanged(nameof(item.Quantity));
                UpdateTotalPrice();
                _ = Task.Run(async () =>
                {
                    await firebaseManager.AddFoodToCartAsync(item);
                });
            }
        }

        [RelayCommand]
        private async Task Decrement(AddFoodToCart cartItem)
        {
            var item = CartDataList.FirstOrDefault(x => x.FoodId == cartItem.FoodId);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    OnPropertyChanged(nameof(item.Quantity));
                    OnPropertyChanged(nameof(CartDataList));
                    if (CartDataList.Count == 0)
                    {
                        NoItemInCartAnimation = true;
                        IsPriceStackVisiblity = false;
                    }
                    UpdateTotalPrice();
                    _ = Task.Run(async () =>
                    {
                        await firebaseManager.DecrementFoodQuantityAsync(item);
                    });
                }
                else
                {
                    CartDataList.Remove(item);
                    OnPropertyChanged(nameof(item.Quantity));
                    OnPropertyChanged(nameof(CartDataList));
                    if (CartDataList.Count == 0)
                    {
                        NoItemInCartAnimation = true;
                        IsPriceStackVisiblity = false;
                    }
                    UpdateTotalPrice();
                    _ = Task.Run(async () =>
                    {
                        await firebaseManager.DecrementFoodQuantityAsync(item);
                    });
                }
            }
        }

        [RelayCommand]
        private async void Back()
        {
            await Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task PlaceOrder()
        {
             await Navigation.PushAsync(new ConfirmOrderPages(CartDataList, SubTotal, TotalPrice));
        }
        #endregion

        #region Methods
        private void UpdateTotalPrice()
        {
            SubTotal = CartDataList.Sum(item => item.FoodPrice * item.Quantity);
            TotalPrice = CartDataList.Sum(item => item.FoodPrice * item.Quantity) + DeliveryCharge;
        }
        private async Task OnCartItemDelete(AddFoodToCart? selectedCartItem)
        {
            bool response = await firebaseManager.DeleteCartItemAsync(selectedCartItem);
            if (response)
            {
                CartDataList.Remove(selectedCartItem);
                UpdateTotalPrice();
                if (CartDataList.Count == 0)
                {
                    NoItemInCartAnimation = true;
                    IsPriceStackVisiblity = false;
                }
                else
                {
                    NoItemInCartAnimation = false;
                    IsPriceStackVisiblity = true;
                }
                await Toast.Make("This cart item are deleted").Show();
            }
            else
            {
                await Toast.Make("Some thing went wrong.Try again later").Show();
            }
        }
        public async Task GetCartdata()
        {
            var response = await firebaseManager.GetCartDataAsync();
            if (response != null)
            {
                CartDataList = new ObservableCollection<AddFoodToCart>(response);
                UpdateTotalPrice();
                if (CartDataList.Count == 0)
                {
                    NoItemInCartAnimation = true;
                    IsPriceStackVisiblity = false;
                }
                else
                {
                    NoItemInCartAnimation = false;
                    IsPriceStackVisiblity = true;
                }
            }
        }
        private async Task RefreshCartDataAsync()
        {
            var response = await firebaseManager.GetCartDataAsync();
            if (response != null)
            {
                CartDataList = new ObservableCollection<AddFoodToCart>(response);
            }
        }
        #endregion
    }
}
