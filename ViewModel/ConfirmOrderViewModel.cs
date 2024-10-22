using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Pages;
using FoodNinja.Pages.CartTabScreen;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoodNinja.ViewModel
{
    public partial class ConfirmOrderViewModel : ObservableObject
    {
        #region Fields
        private FirebaseManager firebaseManager = new FirebaseManager();
        public INavigation Navigation { get; }

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool paymentMethodSelected;

        [ObservableProperty]
        private bool isCodSelected;

        [ObservableProperty]
        private Color codBorderColor;

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private double subTotal;

        [ObservableProperty]
        private double totalPrice;

        [ObservableProperty]
        private double deliveryCharge = 2;

        [ObservableProperty]
        private string houseFlatBlockNo;

        [ObservableProperty]
        private string cityArea;

        [ObservableProperty]
        private string state;

        [ObservableProperty]
        private string pincode;

        [ObservableProperty]
        private ImageSource paymentMethodImage;

        [ObservableProperty]
        private UserDataModel userData;

        [ObservableProperty]
        private string newAddress;

        [ObservableProperty]
        private ObservableCollection<AddFoodToCart> orderFoodItem = new ObservableCollection<AddFoodToCart>();

        [ObservableProperty]
        private ObservableCollection<PaymentModel> paymentMethodList = new ObservableCollection<PaymentModel>();

        [ObservableProperty]
        private PaymentModel selectedPaymentMethod;


        [ObservableProperty]
        private string returnFromPage;
        public ICommand PaymentMethodSelectedCommand { get; }

        #endregion
        #region Constructor
        public ConfirmOrderViewModel(FirebaseManager _firebaseManager, INavigation navigation, ObservableCollection<AddFoodToCart> cartDataList, double subTotal, double totalPrice)
        {
            firebaseManager = _firebaseManager;
            Navigation = navigation;
            OrderFoodItem = cartDataList;
            SubTotal = subTotal;
            TotalPrice = totalPrice;
            PaymentMethodSelectedCommand = new Command<PaymentModel>(async (selectedPaymentMethod) => await OnPaymentMethodSelectedAsync(selectedPaymentMethod));
            ReturnFromPage = string.Empty;
        }
        #endregion

        #region Methods
        public async Task FetchUserDataAsync()
        {
            UserId = Preferences.Get("LocalId", string.Empty);
            var response = await firebaseManager.GetUserDataAsync(UserId);
            if (response != null)
            {
                UserData = response;
                PaymentMethodList = new ObservableCollection<PaymentModel>(UserData.PaymentMethod.Values);
            }
        }
        public async Task UpdateUserAddress()
        {
            NewAddress = $"{HouseFlatBlockNo}, {CityArea}, {State}, {Pincode}";
            UserId = Preferences.Get("LocalId", string.Empty);
            await firebaseManager.UpdateUserAddressAysnc(UserId, NewAddress);
            await FetchUserDataAsync();
        }
        private async Task OnPaymentMethodSelectedAsync(PaymentModel selectedPaymentMethod)
        {
            if (selectedPaymentMethod == null)
            {
                return;
            }
            selectedPaymentMethod.IsSelectedPayment = true;
            CodBorderColor = Colors.Transparent;
            SelectedPaymentMethod = selectedPaymentMethod;
            PaymentMethodSelected = true;
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }
        [RelayCommand]
        private async Task CodSelected()
        {
            PaymentModel.DeselectCurrentPayment();
            CodBorderColor = Color.FromArgb("#53E88B");
            PaymentMethodSelected = true;
            IsCodSelected = true;
        }

        [RelayCommand]
        private async Task AddMorePayment()
        {
            await Navigation.PushAsync(new AddPaymentMethodPage());
        }

        [RelayCommand]
        private async Task PlacedOrder()
        {

            if (!PaymentMethodSelected)
            {
                if (ReturnFromPage == "SuccessfullOrderPlacedPage")
                {
                    if (OrderFoodItem.Count == 0)
                    {
                        await Toast.Make("Order has already been placed.").Show();
                        return;
                    }
                }
            }
            else if(ReturnFromPage == "SuccessfullOrderPlacedPage")
            {
                await Toast.Make("Order has already been placed.").Show();
                return;
            }
            else if (!PaymentMethodSelected || string.IsNullOrWhiteSpace(UserData?.Address))
            if (!PaymentMethodSelected || string.IsNullOrWhiteSpace(UserData?.Address))
            {
                await Toast.Make("Please select at least one payment method").Show();
                return;
            }
            else
            {
                var random = new Random();
                var statusOptions = new List<string> { "Processing", "Preparing", "Rider Assign" };
                var placedOrders = new List<OrderPlacedModel>();
                foreach (var foodItem in OrderFoodItem)
                {
                    int orderId = random.Next(1000, 9999);
                    var placedOrder = new OrderPlacedModel
                    {
                        OrderId = orderId,
                        UserId = UserId,
                        CartId = foodItem.CartId,
                        UserName = UserData.FirstName + " " + UserData.LastName,
                        RestaurantName = foodItem.RestaurantName,
                        RestaurantId = foodItem.RestaurantId,
                        FoodId = foodItem.FoodId,
                        FoodName = foodItem.FoodName,
                        FoodImage = foodItem.FoodImage,
                        FoodDescription = foodItem.FoodDescription,
                        FoodPrice = foodItem.FoodPrice,
                        FoodRating = foodItem.FoodRating,
                        RestaurantDescription = foodItem.RestaurantDescription,
                        RestaurantAddress = foodItem.RestaurantAddress,
                        RestaurantRating = foodItem.RestaurantRating,
                        RestaurantDistance = foodItem.RestaurantDistance,
                        FoodQuantity = foodItem.Quantity,
                        RestaurantLat = foodItem.RestaurantLat,
                        RestaurantLong = foodItem.RestaurantLong,
                        UserAddress = UserData.Address,
                        UserLatitude = UserData.Latitude,
                        UserLongitude = UserData.Longitude,
                        TotalPrice = TotalPrice,
                        DeliveryCharge = DeliveryCharge,
                        IsCODSelected = IsCodSelected,
                        UserPaymentMethod = SelectedPaymentMethod,
                        OrderStatus = statusOptions[random.Next(statusOptions.Count)],
                        TimeStamp = DateTime.UtcNow
                    };
                    placedOrders.Add(placedOrder);
                }

                IsLoading = true;
                bool success = await firebaseManager.PlacedOrderAsync(UserId, placedOrders);
                if (success)
                {
                    var restaurantId = OrderFoodItem.FirstOrDefault()?.RestaurantId;
                    if (restaurantId != null)
                    {
                        var placedOrderItem = placedOrders.FirstOrDefault();
                        await firebaseManager.DeleteCartByIdAsync(UserId, restaurantId);
                        await Toast.Make("Order Placed").Show();
                        await Navigation.PushAsync(new SuccessfullOrderPlacedPage(placedOrderItem));
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to place the order.", "OK");
                }
                IsLoading = false;
            }
        }
        #endregion
    }
}
