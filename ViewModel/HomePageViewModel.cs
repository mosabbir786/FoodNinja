using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Pages.HomeTabScreen;
using FoodNinja.Pages.Popups;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoodNinja.ViewModel
{
    public partial class HomePageViewModel:ObservableObject
    {
        #region Field
        [ObservableProperty]
        private ObservableCollection<Item> items;

        [ObservableProperty]
        private ObservableCollection<Item> singleCollection;


        private FirebaseManager firebaseManager = new FirebaseManager();
        public INavigation Navigation { get; }

        private TypeOfPage _currentPage;
        public TypeOfPage CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        [ObservableProperty]
        private Color borderBackground;

        [ObservableProperty]
        private double borderOpacity;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isViewMoreLbl1;

        [ObservableProperty]
        private ObservableCollection<NearestRestaurantModel> restaurantList = new ObservableCollection<NearestRestaurantModel>();

        [ObservableProperty]
        private ObservableCollection<NearestRestaurantModel> restaurant = new ObservableCollection<NearestRestaurantModel>();

        [ObservableProperty]
        private ObservableCollection<PopularMenuModel> popularMenueList = new ObservableCollection<PopularMenuModel>();

        [ObservableProperty]
        private ObservableCollection<PopularMenuModel> popularMenu = new ObservableCollection<PopularMenuModel>();

        public RelayCommand<NearestRestaurantModel> RestaurantFrameTappedCommand { get; }

        [ObservableProperty]
        private string currentViewModel = "HomePage";
        public ICommand AddFoodToCartCommand { get; }
        #endregion

        #region Constructon
        public HomePageViewModel(INavigation navigation, FirebaseManager _firebaseManager)
        {
            Navigation = navigation;
            firebaseManager = _firebaseManager;
            Items = new ObservableCollection<Item>
            {
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" },
                new Item { Name = "Name", Description = "Description", Image = "rs1.png" }
            };
            SingleCollection = new ObservableCollection<Item> { Items.FirstOrDefault() };
            LoadData();
            RestaurantFrameTappedCommand = new RelayCommand<NearestRestaurantModel>(async (selectedRestaurant) => await OnRequestFrameTapped(selectedRestaurant));
            AddFoodToCartCommand = new Command<PopularMenuModel>(async (selectedFood) => await OnAddingFoodToCartAsync(selectedFood));
        }
        #endregion

        #region Methods
        private async void LoadData()
        {
            IsLoading = true;
            await LoadRestaurantsAsync();
            await LoadPopularMenuAsync();
            IsLoading = false;
        }
        private async Task OnAddingFoodToCartAsync(PopularMenuModel? selectedFood)
        {
            if (selectedFood == null)
            {
                await Toast.Make("No food selected").Show();
                return;
            }
            string userId = Preferences.Get("LocalId", string.Empty);
            bool isDifferentRestaurantPresent = await firebaseManager.IsDifferentRestaurantPresentAsync(userId, selectedFood.RestaurantId);

            if (isDifferentRestaurantPresent)
            {
                #region Popup Code for replace cart item 
                var currentPage = Application.Current.MainPage;
                Func<Task> updateCartCallback = async () =>
                {
                    await Toast.Make("Food added to cart").Show();
                };
                var replaceCartItemPopup = new ReplaceCartItemPopup(firebaseManager, updateCartCallback)
                {
                    SelectedMenuFood = selectedFood,
                    CurrentPage = CurrentViewModel,
                };
                await currentPage.ShowPopupAsync(replaceCartItemPopup);
                return;
                #endregion
            }
            else
            {
                #region For Normal Add Item In Cart If there are no item of different restaurant
                var random = new Random();
                int id = random.Next(1000, 9999);
                var addFoodToCart = new AddFoodToCartByPopularMenu
                {
                    FoodId = selectedFood.Id,
                    CartId = id,
                    FoodImage = selectedFood.FoodImage,
                    FoodName = selectedFood.DishName,
                    RestaurantName = selectedFood.RestaurantName,
                    RestaurantId = selectedFood.RestaurantId,
                    FoodPrice = selectedFood.Price,
                    FoodRating = selectedFood.FoodRating,
                    FoodDescription = selectedFood.FoodDescription,
                    RestaurantDescription = selectedFood.RestaurantDescription,
                    RestaurantAddress = selectedFood.RestaurantAddress,
                    RestaurantLat = selectedFood.RestaurantLat,
                    RestaurantLong = selectedFood.RestaurantLong,

                };
                var response = await firebaseManager.AddFoodToCartFromPopularMenuAsync(addFoodToCart);
                if (response)
                {
                    await Toast.Make("Food added to cart").Show();
                }
                else
                {
                    await Toast.Make("Unable to add food in cart").Show();
                }
                #endregion
            }
        }
        private async Task OnRequestFrameTapped(NearestRestaurantModel? selectedRestaurant)
        {
            var restaurantService = DependencyService.Get<RestaurantService>();
            restaurantService.SelectedRestaurant = selectedRestaurant;
            await Navigation.PushAsync(new RestaurantDetailPage());
        }
        public async Task LoadRestaurantsAsync()
        {
            try
            {
                var restaurants = await firebaseManager.GetNearestRestaurantAsync();
                if (restaurants != null)
                {
                    RestaurantList.Clear();
                    Restaurant = new ObservableCollection<NearestRestaurantModel>(restaurants);
                    if (CurrentPage == TypeOfPage.HomePage)
                    {
                        RestaurantList = new ObservableCollection<NearestRestaurantModel>(restaurants.Take(3));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error ************************: {ex.Message}");
            }
        }
        public async Task LoadPopularMenuAsync()
        {
            var popularMenues = await firebaseManager.GetPopularMenuListAync();
            if (popularMenues != null)
            {
                PopularMenu = new ObservableCollection<PopularMenuModel>(popularMenues);
                if (CurrentPage == TypeOfPage.HomePage)
                {
                    PopularMenueList = new ObservableCollection<PopularMenuModel>(popularMenues.Take(1));
                }
            }
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task NavigateToNextPage()
        {
            var restaurantService = DependencyService.Get<RestaurantService>();
            restaurantService.Restaurant = Restaurant;
            await Navigation.PushAsync(new RestaurantPage());
        }

        [RelayCommand]
        private async Task NavigateToPopularMenu()
        {
            var restaurantService = DependencyService.Get<RestaurantService>();
            restaurantService.PopularMenu = PopularMenu;
            await Navigation.PushAsync(new PopularMenuPage());
        }

        [RelayCommand]
        private async Task NavigateToFilterPage()
        {
            //await Navigation.PushAsync(new FilterPage());
        }
        #endregion
    }
}

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public int Id { get; set; }
}
public enum TypeOfPage
{
    HomePage,
    RestaurantPage,
    PopularMenuPage,
}
