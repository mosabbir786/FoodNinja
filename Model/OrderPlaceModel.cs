using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Model
{
    public class OrderPlacedModel
    {
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public int CartId { get; set; }

        public string UserName { get; set; }
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
        public int FoodQuantity { get; set; }
        public string UserAddress { get; set; }
        public double UserLatitude { get; set; }
        public double UserLongitude { get; set; }
        public double TotalPrice { get; set; }
        public double DeliveryCharge { get; set; }
        public double RestaurantLat { get; set; }
        public double RestaurantLong { get; set; }
        public bool IsCODSelected { get; set; }
        public string OrderStatus { get; set; }
        public PaymentModel UserPaymentMethod { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
