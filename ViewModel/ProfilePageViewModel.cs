using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using FoodNinja.Model;
using FoodNinja.Pages;
using FoodNinja.Pages.Popups;
using FoodNinja.Pages.ProfileTabScreen;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class ProfilePageViewModel:ObservableObject
    {
        #region Fields
        private FirebaseManager firebaseManager = new FirebaseManager();
        FirebaseClient firebaseClient;
        private const string DatabaseUrl = "https://fir-maui-491c3-default-rtdb.firebaseio.com/";

        public INavigation Navigation { get; }

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private UserDataModel userData;

        [ObservableProperty]
        private ObservableCollection<PaymentModel> paymentMethodsList;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string fullName;

        [ObservableProperty]
        private bool isUpdated = false;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string phoneNumber;
        #endregion


        #region Constructor
        public ProfilePageViewModel(INavigation navigation, FirebaseManager _firebaseManager)
        {
            firebaseClient = new FirebaseClient(DatabaseUrl);
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            PaymentMethodsList = new ObservableCollection<PaymentModel>();
            UserId = Preferences.Get("LocalId", string.Empty);
        }
        #endregion

        #region Methods
        public async Task FetchUserDataAsync()
        {
            IsLoading = true;
            var response = await firebaseManager.GetUserDataAsync(UserId);
            if (response != null)
            {
                UserData = response;
                FullName = UserData.FirstName + " " + UserData.LastName;
                if (UserData.PaymentMethod == null)
                {
                    UserData.PaymentMethod = new Dictionary<int, PaymentModel>();
                }
                var paymentMethods = new ObservableCollection<PaymentModel>(UserData.PaymentMethod.Values);
                PaymentMethodsList = paymentMethods;
            }
            IsLoading = false;

            /* Stopwatch stopwatch = new Stopwatch();
             stopwatch.Start();

             var observable = firebaseClient
             .Child("UserData")
             .Child(UserId)
             .AsObservable<UserDataModel>();

             stopwatch.Stop();
             Console.WriteLine($"Time taken to set up observable: {stopwatch.Elapsed.TotalSeconds} seconds");
             stopwatch.Restart();

             observable.Subscribe(async user =>
             {
                 await Task.Run(() =>
                 {
                     var dataStopwatch = new Stopwatch();
                     dataStopwatch.Start();
                     UserData = user.Object;
                     dataStopwatch.Stop();
                     Console.WriteLine($"Time taken to assign UserData: {dataStopwatch.Elapsed.TotalSeconds} seconds");

                     dataStopwatch.Restart(); // Restart for FullName
                     FullName = UserData.FirstName + " " + UserData.LastName;
                     dataStopwatch.Stop();
                     Console.WriteLine($"Time taken to construct FullName: {dataStopwatch.Elapsed.TotalSeconds} seconds");
                 });               
                 stopwatch.Stop();
                 Console.WriteLine($"Time taken to display user data: {stopwatch.Elapsed.TotalSeconds} second");

             }, error =>
             {
                 Console.WriteLine("Error while fetching user data: " + error.Message);
             });*/
        }
        public async Task LoadPaymentMethod()
        {
            UserId = Preferences.Get("LocalId", string.Empty);
            var userNode = await firebaseClient
                .Child("UserData")
                .Child(UserId)
                .OnceAsync<object>();

            var users = userNode.FirstOrDefault();
            var firebaseKey = users.Key;
            if (firebaseKey != null)
            {
                MainThread.BeginInvokeOnMainThread(() => PaymentMethodsList.Clear());
                firebaseClient
               .Child("UserData")
               .Child(UserId)
               .Child(firebaseKey)
               .Child("PaymentMethod")
               .AsObservable<PaymentModel>()
               .Subscribe(payment =>
               {
                   MainThread.BeginInvokeOnMainThread(() =>
                   {
                       PaymentMethodsList.Add(payment.Object);
                   });
               }, error =>
               {
                   Application.Current.MainPage.DisplayAlert("Error", "Failed to fetch payment methods.", "OK");
               });
            }
        }

        #endregion

        #region Commands
        [RelayCommand]
        private async Task DeleteSelectedPayment(PaymentModel selectedPaymentModel)
        {
            if(selectedPaymentModel != null)
            {
                PaymentMethodsList.Remove(selectedPaymentModel);
                var isDeleted = await firebaseManager.DeletePaymentMethodAsync(UserId, selectedPaymentModel.Id);
                if(isDeleted)
                {
                    await Toast.Make("Success: Payment method has been deleted.").Show();
                }
            }
        }

        [RelayCommand]
        private async Task GoToOrderDetailPage()
        {
            await Navigation.PushAsync(new PreviousOrderDetailPage());
        }

        [RelayCommand]
        private async Task GoToEditProfilePage()
        {
            await Navigation.PushAsync(new EditProfilePage());
        }

        [RelayCommand]
        private async Task Logout()
        {
           await Application.Current.MainPage.ShowPopupAsync(new LogoutPopup());
        }

        [RelayCommand]
        private async Task AddPaymentMethod()
        {
            await Navigation.PushAsync(new AddPaymentMethodPage());
        }
        #endregion
    }
}
