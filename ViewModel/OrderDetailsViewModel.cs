using CommunityToolkit.Mvvm.ComponentModel;
using FoodNinja.Model;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.ViewModel
{
    public partial class OrderDetailsViewModel:ObservableObject
    {
        #region Properties
        [ObservableProperty]
        private ObservableCollection<Items> dummyCollection;

        private readonly FirebaseManager firebaseManager;
        public INavigation Navigation { get; }

        [ObservableProperty]
        private ObservableCollection<OrderPlacedModel> allOrderList;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private bool noPreviousOrderAnimation;
        #endregion

        #region Constructor
        public OrderDetailsViewModel(INavigation navigation, FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            DummyCollection = new ObservableCollection<Items>
            {
                new Items { FoodName = "Name", RestaurantName = "Description", Image = "rs1.png", Price = 7.00 , Status = "Processing"},
                new Items { FoodName = "Name", RestaurantName = "Description", Image = "rs1.png", Price = 7.00 , Status = "Processing"},
                new Items { FoodName = "Name", RestaurantName = "Description", Image = "rs1.png", Price = 7.00 , Status = "Processing"},
                new Items { FoodName = "Name", RestaurantName = "Description", Image = "rs1.png", Price = 7.00 , Status = "Processing"},
                new Items { FoodName = "Name", RestaurantName = "Description", Image = "rs1.png", Price = 7.00 , Status = "Processing"},
                new Items { FoodName = "Name", RestaurantName = "Description", Image = "rs1.png", Price = 7.00 , Status = "Processing"},
            };
            AllOrderList = new ObservableCollection<OrderPlacedModel>();
            UserId = Preferences.Get("LocalId", string.Empty);
            FetchAllOrderAsync();
        }
        #endregion

        #region Methods
        private async void FetchAllOrderAsync()
        {
            IsLoading = true;
            var response = await firebaseManager.GetAllPlacedOrderAsync(UserId);
            if(response != null)
            {
                AllOrderList = new ObservableCollection<OrderPlacedModel>(response);
            }
            IsLoading = false;
            Console.WriteLine("*******************" + AllOrderList.Count());
        }
        #endregion
    }

    public class Items
    {
        public string FoodName { get; set; }
        public string RestaurantName { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
    }
}
