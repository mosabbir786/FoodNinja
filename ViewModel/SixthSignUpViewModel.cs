using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Pages.Signup_Screen;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class SixthSignUpViewModel:ObservableObject
    {
        #region Fields
        public INavigation Navigation { get; }
        [ObservableProperty]
        private double latitude;

        [ObservableProperty]
        private double longitude;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string area = string.Empty;

        [ObservableProperty]
        private string state = string.Empty;

        [ObservableProperty]
        private string pincode = string.Empty;

        [ObservableProperty]
        private string fullAddress = "Set Location";

        [ObservableProperty]
        private UserDataModel userData;

        [ObservableProperty]
        private string houseFlatBlockNo;

        [ObservableProperty]
        private string cityArea;

        [ObservableProperty]
        private string deliveryState;

        [ObservableProperty]
        private string deliveryPincode;

        [ObservableProperty]
        private string address;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string mobileNumber;

        [ObservableProperty]
        private string finalImage;

        private readonly FirebaseManager firebaseManager = new FirebaseManager();
        #endregion

        #region Constructor
        public SixthSignUpViewModel(INavigation navigation, string email, string firstName, string lastName, string mobileNumber, string finalImage, FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            MobileNumber = mobileNumber;
            FinalImage = finalImage;
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task FetchLocation()
        {
            await GetCurrentLocationAsync();
        }

        [RelayCommand]
        private async Task Next()
        {
            Console.WriteLine("**********************" + Latitude);
            Console.WriteLine("**********************" + Longitude);
            Address = $"{HouseFlatBlockNo}, {CityArea}, {DeliveryState}, {DeliveryPincode}";
            UserData = new UserDataModel()
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                MobileNumber = MobileNumber,
                Image = FinalImage,
                Address = Address,
                Latitude = Latitude,
                Longitude = Longitude,
                PaymentMethod = new ObservableCollection<PaymentModel>()
            };
            IsLoading = true;
            await Task.Delay(50);
            bool response = await firebaseManager.UploadUserDataAsync(UserData);
            if (response)
            {
                await Navigation.PushAsync(new FinalSignUpPage());
            }
            IsLoading = false;
        }
        #endregion

        #region Methods
        private async Task GetCurrentLocationAsync()
        {
            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Best,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }
                else
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        Area = placemark.SubLocality;
                        State = placemark.AdminArea;
                        Pincode = placemark.PostalCode;
                        FullAddress = area + " " + state + "" + Pincode;
                        Latitude = location.Latitude;
                        Longitude = location.Longitude;
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await CatchException(fnsEx, "Location Services not supported!", "Location Services are not supported on this device");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await CatchException(fneEx, "Location Services are disabled!", "Location Services are not enabled on this device");
                await Application.Current.MainPage.DisplayAlert("Warning", "Location Services are not enabled on this device please enable location srvice from setting", "Ok");
                FullAddress = "Set Location";
            }
            catch (PermissionException pEx)
            {
                await CatchException(pEx, "Location Services are not permitted!", "Location Services are not permitted on this device");
            }
            catch (Exception ex)
            {
                await CatchException(ex, "Location Services error!", "Location Services are not working on this device");
            }
        }
        private async Task CatchException(Exception ex, string message, string description)
        {
            await Task.Delay(1);
            Console.WriteLine(message);
        }
        #endregion
    }
}
