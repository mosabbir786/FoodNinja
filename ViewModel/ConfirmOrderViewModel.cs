using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseAdmin.Messaging;
using FoodNinja.Model;
using FoodNinja.Pages;
using FoodNinja.Pages.CartTabScreen;
using FoodNinja.Pages.Popups;
using FoodNinja.Services;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ConfirmOrderViewModel> _logger;
        private readonly INotificationPermissionManager _permissionHelper;

        [ObservableProperty]
        private string firebaseToken = string.Empty;

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
        private string address;

        [ObservableProperty]
        private ObservableCollection<AddFoodToCart> orderFoodItem = new ObservableCollection<AddFoodToCart>();

        [ObservableProperty]
        private ObservableCollection<PaymentModel> paymentMethodList = new ObservableCollection<PaymentModel>();

        [ObservableProperty]
        private PaymentModel selectedPaymentMethod;


        [ObservableProperty]
        private string returnFromPage;

        [ObservableProperty]
        private bool addressControlVisiblity;

        [ObservableProperty]
        private bool addAddressBtnVisiblity;

        [ObservableProperty]
        private string addOrEditBtnText;
        public ICommand PaymentMethodSelectedCommand { get; }

        #endregion

        #region Constructor
        public ConfirmOrderViewModel(FirebaseManager _firebaseManager, INavigation navigation, ObservableCollection<AddFoodToCart> cartDataList, double subTotal, double totalPrice,INotificationPermissionManager permissionHelper)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            _permissionHelper = permissionHelper;
            _logger = factory.CreateLogger<ConfirmOrderViewModel>();
            this.firebaseManager = _firebaseManager;
            this.Navigation = navigation;
            this.OrderFoodItem = cartDataList;
            this.SubTotal = subTotal;
            this.TotalPrice = totalPrice;
            PaymentMethodSelectedCommand = new Command<PaymentModel>(async (selectedPaymentMethod) => await OnPaymentMethodSelectedAsync(selectedPaymentMethod));
            ReturnFromPage = string.Empty;
        }
        #endregion

        #region Methods
        public async Task FetchFirebaseToken()
        {
            await Task.Delay(1);
            try
            {
#if ANDROID
                if (Preferences.ContainsKey("DeviceToken"))
                {
                    FirebaseToken = Preferences.Get("DeviceToken", string.Empty);
                }

                if (!string.IsNullOrEmpty(FirebaseToken))
                {
                    _logger.LogInformation("Firenase token {FirebaseToken}", FirebaseToken);
                    FirebaseToken = FirebaseToken;
                    Preferences.Set("DeviceToken", FirebaseToken);
                }
                else
                {
                    throw new InvalidOperationException("Firebase token could not be retrieved");
                }
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching firebase token: {ex.Message}");
            }
        }
        public async Task FetchUserDataAsync()
        {
            UserId = Preferences.Get("LocalId", string.Empty);
            var response = await firebaseManager.GetUserDataAsync(UserId);
            if (response != null)
            {
                UserData = response;
                HouseFlatBlockNo = UserData.HouseOrFlatOrBlockName;
                CityArea = UserData.AreaOrCity;
                State = UserData.State;
                Pincode = UserData.Pincode;
                Address = $"{UserData.HouseOrFlatOrBlockName}, {UserData.AreaOrCity}, {UserData.State}, {UserData.Pincode}";
                bool isAddressEmpty = string.IsNullOrEmpty(UserData.HouseOrFlatOrBlockName) &&
                                      string.IsNullOrEmpty(UserData.AreaOrCity) &&
                                      string.IsNullOrEmpty(UserData.State) &&
                                      string.IsNullOrEmpty(UserData.Pincode);
                if(isAddressEmpty)
                {
                    Address = null;
                    AddressControlVisiblity = false;
                    AddAddressBtnVisiblity = true;
                    AddOrEditBtnText = "Add Address";
                }
                else
                {
                    AddressControlVisiblity = true;
                    AddAddressBtnVisiblity = false;
                    AddOrEditBtnText = "Edit Address";
                }
                                         
                PaymentMethodList = new ObservableCollection<PaymentModel>(UserData.PaymentMethod.Values);
            }
        }
        public async Task UpdateUserAddress()
        {
            UserId = Preferences.Get("LocalId", string.Empty);
            bool isAddressEdited = await firebaseManager.EditAddressAsync(UserId, HouseFlatBlockNo, CityArea, State, Pincode);
        }
        private async Task OnPaymentMethodSelectedAsync(PaymentModel selectedPaymentMethod)
        {
            await Task.Delay(1);
            if (selectedPaymentMethod == null)
            {
                return;
            }
            selectedPaymentMethod.IsSelectedPayment = true;
            CodBorderColor = Colors.Transparent;
            SelectedPaymentMethod = selectedPaymentMethod;
            PaymentMethodSelected = true;
        }
        private async Task Send(string title, string body)
        {
            if (Preferences.ContainsKey("DeviceToken"))
            {
                FirebaseToken = Preferences.Get("DeviceToken", "");
            }
            if (string.IsNullOrEmpty(FirebaseToken))
            {
                _logger.LogWarning("No device token found. Unable to send notification.");
                return;
            }
            var androidNotificationObject = new Dictionary<string, string>
            {
                { "NavigationID", "2" }
            };
            var iosNotificationObject = new Dictionary<string, object>
            {
                { "NavigationID", "2" }
            };

            var pushNotificationRequest = new PushNotificationRequest
            {
                notification = new NotificationMessageBody
                {
                    title = title,
                    body = body,
                },
                data = androidNotificationObject,
                registration_ids = new List<string> { FirebaseToken }
            };
            var messageList = new List<Message>();
            var obj = new Message
            {
                Token = FirebaseToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                },
                Data = androidNotificationObject,
                Apns = new ApnsConfig()
                {
                    Aps = new Aps
                    {
                        Badge = 15,
                        CustomData = iosNotificationObject,
                    }
                }
            };
            try
            {
                messageList.Add(obj);
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(obj);
                _logger.LogInformation("Notification sent successfully: {response}", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending push notification: {message}", ex.Message);
            }
        }

        public async Task HandleNotificationPermission()
        {
            if (OperatingSystem.IsAndroid())
            {
                if (DeviceInfo.Version.Major >= 13)
                {
                    try
                    {
                        var permissionStatus = await _permissionHelper.CheckNotificationPermissionAsync();
                        if (permissionStatus != PermissionStatus.Granted)
                        {
                            int deniedCount = _permissionHelper.GetDeniedCount();
                            bool isPermissionGranted = permissionStatus == PermissionStatus.Granted;
                            if (deniedCount >= 2)
                            {
                                await App.Current.MainPage.ShowPopupAsync(new NotificationPermissionPopup());
                                return;
                            }
                            else
                            {
                                await _permissionHelper.RequestNotificationPermissionAsync();
                            }
                        }
                        else
                        {
                            _permissionHelper.ResetDeniedCount();
                            Console.WriteLine("Notification Permission  Granted");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else if (OperatingSystem.IsIOS())
            {
                //Commming Soon
            }
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
            await Task.Delay(1);
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
            await ExecutePlacedOrderAsync();
        }

        private async Task ExecutePlacedOrderAsync()
        {
            bool isAddressEmpty = string.IsNullOrEmpty(UserData.HouseOrFlatOrBlockName) &&
                                 string.IsNullOrEmpty(UserData.AreaOrCity) &&
                                 string.IsNullOrEmpty(UserData.State) &&
                                 string.IsNullOrEmpty(UserData.Pincode);
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
            else if (ReturnFromPage == "SuccessfullOrderPlacedPage")
            {
                await Toast.Make("Order has already been placed.").Show();
                return;
            }
            if (!PaymentMethodSelected)
            {
                await Toast.Make("Please select at least one payment method").Show();
                return;
            }
            else if (isAddressEmpty)
            {
                await Toast.Make("Please add address to place order.").Show();
                return;
            }
            else
            {
                var random = new Random();
                var statusOptions = new List<string> { "Processing", "Preparing", "Rider Assign", "Complete" };
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
                        UserAddress = UserData.HouseOrFlatOrBlockName + "," + " " + UserData.AreaOrCity + "," + " " + UserData.State + "," + " " + UserData.Pincode,
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
                        var permissionStatus = await _permissionHelper.CheckNotificationPermissionAsync();
                        if (permissionStatus == PermissionStatus.Granted)
                        {
                           await Send("🍕 Order Placed Successfully!", "Thank you for your order! Your food is being prepared and will be on its way shortly. 🍔🍟");
                        }
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
