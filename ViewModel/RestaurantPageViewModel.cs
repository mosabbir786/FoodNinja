using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Pages.HomeTabScreen;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class RestaurantPageViewModel:ObservableObject
    {
        #region Fields
        public INavigation Navigation { get; }

        [ObservableProperty]
        private ObservableCollection<NearestRestaurantModel> restaurantList = new ObservableCollection<NearestRestaurantModel>();

        public IAsyncRelayCommand RestaurantFrameTappedCommand { get; }
        #endregion

        #region Constructor
        public RestaurantPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            var restaurantService = DependencyService.Get<RestaurantService>();
            var restaurantList = restaurantService.Restaurant;
            RestaurantList = restaurantList;
            RestaurantFrameTappedCommand = new AsyncRelayCommand<NearestRestaurantModel>(async (selectedRestaurant) => await OnRequestFrameTapped(selectedRestaurant));
        }
        #endregion

        private async Task OnRequestFrameTapped(NearestRestaurantModel selectedRestaurant)
        {
            var restaurantService = DependencyService.Get<RestaurantService>();
            restaurantService.SelectedRestaurant = selectedRestaurant;
            await Navigation.PushAsync(new RestaurantDetailPage());
        }
    }
}
