using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodNinja.Model;
using FoodNinja.Pages.Popups;
using FoodNinja.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
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

        [ObservableProperty]
        private bool noData;

        [ObservableProperty]
        private bool comeFromAfterDelete;
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

        #region Commands
        [RelayCommand]
        private async Task DeleteOrder(OrderPlacedModel selectedOrder)
        {
            if(selectedOrder != null)
            {
                if(selectedOrder.OrderStatus == "Complete")
                {
                    var popup = new DeletePreviousOrderPopup(selectedOrder.OrderId, RemoveOrderFromCollectionView);
                    await App.Current.MainPage.ShowPopupAsync(popup);
                }
                else
                {
                    await Toast.Make("The order is not yet complete.").Show();
                }
            }
        }

        [RelayCommand]
        private async Task Back()
        {
            await Navigation.PopAsync();
        }
        #endregion

        #region Methods
        private async void RemoveOrderFromCollectionView(int orderId)
        {
            var orderToRemove = AllOrderList.FirstOrDefault(o => o.OrderId == orderId);
            if(orderToRemove != null)
            {
                AllOrderList.Remove(orderToRemove);
                ComeFromAfterDelete = true;
                FetchAllOrderAsync();
            }
        }
        private async void FetchAllOrderAsync()
        {
            if(!ComeFromAfterDelete)
            {
                IsLoading = true;
            }
            var response = await firebaseManager.GetAllPlacedOrderAsync(UserId);
            if(response != null)
            {
                foreach(var order in response)
                {
                    if(order.OrderStatus == "Complete")
                    {
                        order.FoodImage = ConvertToGrayscale(order.FoodImage);
                    }
                }
                AllOrderList = new ObservableCollection<OrderPlacedModel>(response);
                if (AllOrderList.Count == 0)
                {
                    NoData = true;
                }
                else
                {
                    NoData = false;
                }
            }
            IsLoading = false;
            ComeFromAfterDelete = false;
        }

        private string? ConvertToGrayscale(string? base64Image)
        {
            var byteArray = Convert.FromBase64String(base64Image);

            using var originalBitmap = SKBitmap.Decode(byteArray);
            using var grayscaleBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);

            using(var canvas = new SKCanvas(grayscaleBitmap))
            {
                var paint = new SKPaint
                {
                    ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
                    {
                        0.33f, 0.33f, 0.33f, 0, 0,
                        0.33f, 0.33f, 0.33f, 0, 0,
                        0.33f, 0.33f, 0.33f, 0, 0,
                        0, 0, 0, 1, 0
                    })
                };
                canvas.DrawBitmap(originalBitmap, 0, 0, paint);
            }

            using var image = SKImage.FromBitmap(grayscaleBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return Convert.ToBase64String(data.ToArray());
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
