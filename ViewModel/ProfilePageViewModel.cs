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
        private ObservableCollection<PaymentModel> paymentMethods;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string fullName;
        #endregion


        #region Constructor
        public ProfilePageViewModel(INavigation navigation, FirebaseManager _firebaseManager)
        {
            firebaseClient = new FirebaseClient(DatabaseUrl);
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            PaymentMethods = new ObservableCollection<PaymentModel>();
            LoadUserData();
        }
        #endregion

        #region Methods
        private async void LoadUserData()
        {
            await FetchUserDataAsync();
            await LoadPaymentMethod();
        }
        public async Task FetchUserDataAsync()
        {
            UserId = Preferences.Get("LocalId", string.Empty);
            IsLoading = true;
            var response = await firebaseManager.GetUserDataAsync(UserId);
            if (response != null)
            {
                UserData = response;
                FullName = UserData.FirstName + " " + UserData.LastName;

                if (UserData.PaymentMethod == null)
                {
                    UserData.PaymentMethod = new ObservableCollection<PaymentModel>();
                }
            }
            IsLoading = false;

            /* try
             {
                 var userDataObservable = firebaseClient
                     .Child("UserData")
                     .Child(UserId)
                     .AsObservable<UserDataModel>();
                      userDataObservable.Subscribe(userNode =>
                     {
                         if(userNode.Object != null)
                         {
                             UserDataCollection.Clear();
                             UserDataCollection.Add(userNode.Object);

                         }
                         else
                         {
                             Console.WriteLine("No user data found for this UserId.");
                         }
                     });
                 IsLoading = false;
             }
             catch (Exception ex)
             {
                 Console.WriteLine("Error while fetching user data " + ex.Message);
             }*/
        }

        public async Task LoadPaymentMethod()
        {
            UserId = Preferences.Get("LocalId", string.Empty);
            firebaseClient.Child("UserData").Child(UserId).AsObservable<PaymentModel>().Subscribe(item =>
            {
                if(item != null)
                {
                    PaymentMethods.Add(item.Object);
                }
            });
        }

        public void UpdateUserData(PaymentModel newPaymentMethod)
        {
            if(UserData?.PaymentMethod == null)
            {
                UserData.PaymentMethod = new ObservableCollection<PaymentModel>();
            }
            UserData.PaymentMethod.Add(newPaymentMethod);
            OnPropertyChanged(nameof(UserData));
        }
        #endregion

        #region Commands
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
