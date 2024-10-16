using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Model
{
    public class AddFoodToCart:INotifyPropertyChanged
    {
        public string? UserId { get; set; }
        public int CartId { get; set; }
        public string? RestaurantName { get; set; }
        public int RestaurantId { get; set; }
        public int FoodId { get; set; }
        public string? FoodName { get; set; }
        public string? FoodImage { get; set; }
        public string? FoodDescription { get; set; }
        public double FoodPrice { get; set; }
        public double FoodRating { get; set; }
        public string? RestaurantDescription { get; set; }
        public string? RestaurantAddress { get; set; }
        public double RestaurantRating { get; set; }
        public double RestaurantDistance { get; set; }

        private int quantity = 1;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
        public double RestaurantLat { get; set; }
        public double RestaurantLong { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AddFoodToCartByPopularMenu : INotifyPropertyChanged
    {
        public string? UserId { get; set; }
        public int CartId { get; set; }
        public int FoodId { get; set; }
        public string? FoodImage { get; set; }
        public string? FoodName { get; set; }
        public string? RestaurantName { get; set; }
        public int RestaurantId { get; set; }
        public double FoodPrice { get; set; }
        private int quantity = 1;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
        public double FoodRating { get; set; }
        public string RestaurantAddress { get; set; }
        public string FoodDescription { get; set; }
        public string RestaurantDescription { get; set; }
        public double RestaurantLat { get; set; }
        public double RestaurantLong { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
