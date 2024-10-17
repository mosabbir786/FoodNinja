using CommunityToolkit.Mvvm.ComponentModel;
using Firebase.Auth;
using FoodNinja.Model;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class ProfilePageViewModel:ObservableObject
    {
        #region Fields
        public List<string> Sections => new List<string> { "Profile", "Order Detail" };
        private FirebaseManager firebaseManager = new FirebaseManager();
        public INavigation Navigation { get; }

        [ObservableProperty]
        private bool isOpenCustom;

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private UserDataModel userData;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string fullName;

        public Action TriggerAnimation { get; set; }
        #endregion

        public ProfilePageViewModel(INavigation navigation, FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            LoadUserData();
        }
        private async void LoadUserData()
        {
            await FetchUserDataAsync();
        }

        #region Methods
        public async Task FetchUserDataAsync()
        {
            UserId = Preferences.Get("LocalId", string.Empty);
            IsLoading = true;
            var response = await firebaseManager.GetUserDataAsync(UserId);
            if (response != null)
            {
                UserData = response;
                FullName = UserData.FirstName + " " + UserData.LastName;
            }
            IsLoading = false;

            TriggerAnimation?.Invoke();
        }
        #endregion
    }
}
